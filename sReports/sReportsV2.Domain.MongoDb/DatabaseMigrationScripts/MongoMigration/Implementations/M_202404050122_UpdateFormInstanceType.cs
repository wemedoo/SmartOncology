using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202404050122_UpdateFormInstanceType : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly IMongoCollection<Form> FormCollection;

        public override int Version => 16;

        public M_202404050122_UpdateFormInstanceType()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
            FormCollection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            MigrateFormInstancesToNewModel().Wait();
        }

        protected override void Down()
        {

        }

        private async Task MigrateFormInstancesToNewModel()
        {
            try
            {
                var instancesToWrite = new List<WriteModel<FormInstance>>();
                var findOptions = new FindOptions<FormInstance, FormInstance>() { };
                var nullTypeFilter = Builders<FormInstance>.Filter.ElemMatch(
                    x => x.FieldInstances,
                    Builders<FieldInstance>.Filter.Eq(fi => fi.Type, null)
                );

                var matchingForms = await FormCollection.Find(Builders<Form>.Filter.Empty).ToListAsync().ConfigureAwait(false);

                using (var cursor = await Collection.FindAsync(nullTypeFilter, findOptions).ConfigureAwait(false))
                {
                    while (await cursor.MoveNextAsync().ConfigureAwait(false))
                    {
                        var batch = cursor.Current;

                        foreach (var formInstance in batch)
                        {
                            bool deleteDocument = false;
                            var matchingForm = matchingForms?.Find(x => x.Id == formInstance.FormDefinitionId);
                            foreach (var fieldInstance in formInstance.FieldInstances)
                            {
                                if (fieldInstance.Type == null)
                                {
                                    fieldInstance.Type = matchingForm?.GetAllFields()?.Find(f => f.Id == fieldInstance.FieldId)?.Type; 

                                    if (fieldInstance.Type == null)
                                    {
                                        deleteDocument = true;
                                        break;
                                    }

                                    var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                                    instancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance));
                                }
                            }

                            if (deleteDocument)
                            {
                                var deleteFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                                instancesToWrite.Add(new DeleteOneModel<FormInstance>(deleteFilter));
                            }

                            if (instancesToWrite.Any())
                            {
                                var result = await Collection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                                if (!result.IsAcknowledged)
                                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {instancesToWrite.Count}");

                                instancesToWrite.Clear();
                            }
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                LogHelper.Error("Eror while MigrateFormInstancesToNewModel, error: " + ex.Message);
            }
        }
    }
}
