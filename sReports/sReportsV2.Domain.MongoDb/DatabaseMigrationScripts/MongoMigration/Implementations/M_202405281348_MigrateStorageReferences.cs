using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using System;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.IO;
using sReportsV2.Common.Helpers;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202405281348_MigrateStorageReferences : MongoMigration
    {
        private readonly string uploadFileStorageFolderName;
        private readonly IConfiguration configuration;
        private readonly SReportsContext dbContext;

        public override int Version => 19;

        public M_202405281348_MigrateStorageReferences(IConfiguration configuration, SReportsContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext= dbContext;
            this.uploadFileStorageFolderName = Path.Combine(GetUploadedBaseDirectory(), "UploadedFiles");
        }

        protected override void Up()
        {
            try
            {
                Dictionary<string, List<string>> resourcesToMove = new Dictionary<string, List<string>>();

                HandleOrganizationLogs(resourcesToMove);
                HandleImageMaps(resourcesToMove);
                HandleFormInstances(resourcesToMove);
                MoveFiles(resourcesToMove);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error while M_202405281348_MigrateStorageReferences, error: {ex.Message}, stack trace: {ex.StackTrace}");
                throw;
            }
        }


        protected override void Down()
        {
        }

        #region Gether data and update references

        private void HandleOrganizationLogs(Dictionary<string, List<string>> resourcesToMove)
        {
            foreach (Organization organization in dbContext.Organizations.Where(org => org.LogoUrl != null).ToList())
            {
                string generatedResourceName = organization.LogoUrl.GetResourceNameFromUri();
                organization.LogoUrl = generatedResourceName;

                ModifyStructureForUpdate(StorageDirectoryNames.OrganizationLogo, resourcesToMove, generatedResourceName);

            }
            dbContext.SaveChanges();
        }

        private void HandleImageMaps(Dictionary<string, List<string>> resourcesToMove)
        {
            var collectionForm = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);

            var project1 =
            new BsonDocument("Pages",
                    new BsonDocument("$reduce",
                        new BsonDocument
                        {
                            { "input", "$Chapters.Pages" },
                            {
                                "initialValue",
                                new BsonArray()
                            },
                            { "in",
                                new BsonDocument(
                                    "$concatArrays",
                                    new BsonArray
                                    {
                                        "$$value",
                                        "$$this"
                                    })
                            }
                            }
                        )
                );
            var match2 =
                    new BsonDocument
                    {
                        { "Pages.ImageMap",
                            new BsonDocument("$ne", BsonNull.Value) },
                        { "Pages.ImageMap.Url",
                            new BsonDocument("$ne", "") }
                    };
            var project2 =
                    new BsonDocument("PageId", "$Pages._id");
            var group =
                    new BsonDocument
                    {
                        { "_id", "$_id" },
                        { "Pages",
                            new BsonDocument("$push", "$PageId")
                        }
                    };

            var queryFormResults = collectionForm
                .Aggregate()
                .Project(project1)
                .Unwind("Pages")
                .Match(match2)
                .Project(project2)
                .Group(group)
                .ToList();

            Dictionary<string, List<string>> groupedByMatchedForms = queryFormResults.ToDictionary(
                groupedByForm => GetBsonValueHelper(groupedByForm, "_id").ToString(),
                groupedByForm => GetBsonValueHelper(groupedByForm, "Pages").AsBsonArray.Select(pageId => pageId.ToString()).ToList());

            var forms = collectionForm
                .Find(x => groupedByMatchedForms.Keys.Contains(x.Id))
                .ToList();

            var formsToWrite = new List<WriteModel<Form>>();


            foreach (var form in forms)
            {
                List<string> pagesWithImageMap = groupedByMatchedForms[form.Id];

                foreach (FormPage page in form.GetAllPages().Where(p => pagesWithImageMap.Contains(p.Id)))
                {
                    string generatedUniqueResourceName = page.ImageMap.Url.ToString().GetResourceNameFromUri();
                    page.ImageMap.Url = generatedUniqueResourceName;
                    ModifyStructureForUpdate(StorageDirectoryNames.ImageMap, resourcesToMove, generatedUniqueResourceName);

                    var replaceFilter = Builders<Form>.Filter.Eq(x => x.Id, form.Id);
                    formsToWrite.Add(new ReplaceOneModel<Form>(replaceFilter, form));
                }
            }

            if (formsToWrite.Any())
            {
                var result = collectionForm.BulkWrite(formsToWrite);
                if (!result.IsAcknowledged)
                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {formsToWrite.Count}");

                formsToWrite.Clear();
            }
        }

        private void HandleFormInstances(Dictionary<string, List<string>> resourcesToMove)
        {
            var collectionFormInstance = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);

            var project1 =
                new BsonDocument("FieldInstances", 1);


            var match1 =
            new BsonDocument
                {
                        { "$or",
                            new BsonArray
                            {
                                new BsonDocument("FieldInstances.Type", "file"),
                                new BsonDocument("FieldInstances.Type", "audio")
                            }
                        },
                        {
                            "FieldInstances.FieldInstanceValues.0",
                            new BsonDocument("$exists", true)
                        },
                        {
                            "FieldInstances.FieldInstanceValues.Values.0",
                            new BsonDocument("$exists", true)
                        },
                        {
                            "FieldInstances.FieldInstanceValues.Values",
                            new BsonDocument("$nin",
                                new BsonArray
                                {
                                    BsonNull.Value,
                                    ""
                                })
                        },
                        {
                            "FieldInstances.FieldInstanceValues.IsSpecialValue",
                            new BsonDocument("$ne", true)
                        }
                };

            var project2 =
                new BsonDocument("F", "$FieldInstances.FieldInstanceValues");


            var project3 = new BsonDocument("FieldInstance", "$F.FieldInstanceRepetitionId");

            var group1 =
                new BsonDocument
                    {
                        { "_id", "$_id" },
                        { "FieldInstances",
                            new BsonDocument("$push", "$FieldInstance") }
                    }

            ;

            var queryFormResults = collectionFormInstance
                .Aggregate()
                .Project(project1)
                .Unwind("FieldInstances")
                .Match(match1)
                .Project(project2)
                .Unwind("F")
                .Project(project3)
                .Group(group1)
                .ToList();

            var groupedByMatchedFormInstances = queryFormResults.ToDictionary(
                groupedByForm => GetBsonValueHelper(groupedByForm, "_id").ToString(),
                groupedByForm => GetBsonValueHelper(groupedByForm, "FieldInstances")
                                    .AsBsonArray
                                    .Select(fieldInstanceRepetitionId => fieldInstanceRepetitionId.ToString())
                                    .ToList());

            var formInstances = collectionFormInstance
                .Find(x => groupedByMatchedFormInstances.Keys.Contains(x.Id))
                .ToList();

            var formInstancesToWrite = new List<WriteModel<FormInstance>>();

            foreach (FormInstance formInstance in formInstances)
            {
                List<string> fieldInstances = groupedByMatchedFormInstances[formInstance.Id];

                foreach (FieldInstanceValue fieldInstanceValue in formInstance
                    .FieldInstances
                    .SelectMany(fI => fI.FieldInstanceValues)
                    .Where(p => fieldInstances.Contains(p.FieldInstanceRepetitionId)))
                {
                    if (fieldInstanceValue != null)
                    {
                        List<string> modifiedValues = new List<string>();
                        foreach (string url in fieldInstanceValue.Values)
                        {
                            string generatedUniqueResourceName = url.GetResourceNameFromUri();
                            modifiedValues.Add(generatedUniqueResourceName);
                            ModifyStructureForUpdate(
                                generatedUniqueResourceName.EndsWith("_audio.mp3") ? StorageDirectoryNames.Audio : StorageDirectoryNames.File,
                                resourcesToMove,
                                generatedUniqueResourceName
                                );
                        }
                        fieldInstanceValue.Values = modifiedValues;
                    }

                }

                var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                formInstancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance));
            }

            if (formInstancesToWrite.Any())
            {
                var result = collectionFormInstance.BulkWrite(formInstancesToWrite);
                if (!result.IsAcknowledged)
                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {formInstancesToWrite.Count}");

                formInstancesToWrite.Clear();
            }
        }

        private void ModifyStructureForUpdate(string binaryTypeKey, Dictionary<string, List<string>> resourcesToMove, string generatedResourceName)
        {
            if (!string.IsNullOrEmpty(generatedResourceName))
            {
                if (resourcesToMove.TryGetValue(binaryTypeKey, out List<string> resourceNames))
                {
                    resourceNames.Add(generatedResourceName);
                }
                else
                {
                    resourcesToMove.Add(binaryTypeKey, new List<string> { generatedResourceName });
                }
            }
        }

        private BsonValue GetBsonValueHelper(BsonDocument bson, string value)
        {
            bson.TryGetValue(value, out BsonValue bsonValue);
            return !(bsonValue is BsonNull) ? bsonValue : null;
        }

        #endregion Gether data and update references

        #region Update storage

        private void MoveFiles(Dictionary<string, List<string>> resourcesToMove)
        {
            _ = bool.TryParse(configuration["UseFileStorage"], out bool useFileStorage);
            if (useFileStorage)
            {
                MoveFilesInFileStorage(resourcesToMove);
            }
            else
            {
                MoveFilesInCloudStorage(resourcesToMove);
            }
        }


        #region File Storage
        private void MoveFilesInFileStorage(Dictionary<string, List<string>> resourcesToMove)
        {
            foreach (KeyValuePair<string, List<string>> fileNamesPerDomain in resourcesToMove)
            {
                string fullDirectoryName = FormatFullDirectoryName(fileNamesPerDomain.Key);
                CreateDirectoryIfNotExist(fullDirectoryName);
                foreach (string fileName in fileNamesPerDomain.Value)
                {
                    string oldPath = FormatFullFilePath(uploadFileStorageFolderName, fileName);
                    if (File.Exists(oldPath))
                    {
                        string newPath = FormatFullFilePath(fullDirectoryName, fileName);
                        if (!File.Exists(newPath))
                        {
                            File.Move(oldPath, newPath);
                        }
                    }
                }
            }
        }

        private string GetUploadedBaseDirectory()
        {
            string uploadedFilesBaseDirectory = DirectoryHelper.ProjectBaseDirectory;
            if (!string.IsNullOrWhiteSpace(configuration["UploadedFilesBaseDirectory"]))
                uploadedFilesBaseDirectory = configuration["UploadedFilesBaseDirectory"];
            return uploadedFilesBaseDirectory;
        }

        private void CreateDirectoryIfNotExist(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string FormatFullDirectoryName(string subDirectoryDomain)
        {
            return Path.Combine(uploadFileStorageFolderName, subDirectoryDomain);
        }

        private string FormatFullFilePath(string fullDirectoryPath, string generatedResourceName)
        {
            return Path.Combine(fullDirectoryPath, generatedResourceName);
        }

        #endregion /File Storage

        #region Cloud Storage
        public async Task MoveFilesInCloudStorage(Dictionary<string, List<string>> resourcesToMove)
        {
            BlobContainerClient container = await CloudStorageHelper.GetCloudBlobContainer(configuration["AccountStorage"]).ConfigureAwait(false);
            foreach (KeyValuePair<string, List<string>> fileNamesPerDomain in resourcesToMove)
            {
                foreach (string fileName in fileNamesPerDomain.Value)
                {
                    BlobClient oldBlob = container.GetBlobClient(fileName);
                    BlobClient newBlob = container.GetBlobClient(CloudStorageHelper.FormatFullFilePath(fileName, fileNamesPerDomain.Key));
                    bool oldExists = await oldBlob.ExistsAsync().ConfigureAwait(false);
                    if (oldExists)
                    {
                        bool newExists = await newBlob.ExistsAsync().ConfigureAwait(false);
                        if (!newExists)
                        {
                            await newBlob.SyncCopyFromUriAsync(oldBlob.Uri).ConfigureAwait(false);
                        }
                        await oldBlob.DeleteAsync().ConfigureAwait(false);
                    }
                }
            }
        }
        #endregion /Cloud Storage

        #endregion
    }
}
