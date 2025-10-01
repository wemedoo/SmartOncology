using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.MongoMigrationVersion;
using sReportsV2.Domain.Mongo;
using System;
using System.Configuration;
using System.IO;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class MongoMigrator
    {
        private readonly IMongoCollection<MongoMigrationVersion> Collection;
        private readonly IConfiguration configuration;
        private readonly SReportsContext dbContext;

        public MongoMigrator(IConfiguration configuration, SReportsContext dbContext)
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<MongoMigrationVersion>(MongoCollectionNames.MigrationVersion);
            // Creating a Unique Index for Version Field (if doesn't already exists)
            IndexKeysDefinition<MongoMigrationVersion> key = Builders<MongoMigrationVersion>.IndexKeys.Ascending("Version");
            Collection.Indexes.CreateOne(new CreateIndexModel<MongoMigrationVersion>(key, new CreateIndexOptions() { Name = "VersionUniqueIndex", Unique = true }));
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        /// <summary>
        ///     Maps Each Migration Version with correspective object.
        ///     Note: Every time a new Migration is created, MUST be added in a new case and in the LastVersion case.
        /// </summary>
        private static MongoMigration VersionToMigration(int version, IConfiguration configuration, SReportsContext sReportsContext)
        {
            switch (version)
            {
                case 1:
                    return new M_202211221536_ValueLabel(configuration, sReportsContext);
                case 2:
                    return new M_202212081410_DateFormat();
                case 3:
                    return new M_202302161420_FormInstance_Index();
                case 4:
                    return new M_202211221888_AddDataToTaskDocument(configuration, sReportsContext);
                case 5:
                    return new M_202308231355_FieldInstanceValue_Property();
                case 6:
                    return new M_202310091205_ExtendDateTimeWithOffset();
                case 7:
                    return new M_20231124091405_PopulateFormCodeRelation(configuration, sReportsContext);
                case 8:
                    return new M_20231129091705_SetValueForRenamedFormProperty();
                case 9:
                    return new M_20231226091705_SetAvailableForTaskInitial(sReportsContext);
                case 10:
                    return new M_202312271038_RenameOmniaId();
                case 11:
                    return new M_20231228100405_FormMultipleOrganizations();
                case 12:
                    return new M_202401151709_IncludeFieldInstanceValuesInTextIndex();
                case 13:
                    return new M_20240131_UpdateClinicalDomainToCodeValues(configuration);
                case 14:
                    return new M_202402141255_FieldInstanceValue_Modification(configuration, sReportsContext);
                case 15:
                    return new M_202403211537_UpdateActiveToInFieldInstanceHistory();
                case 16:
                    return new M_202404050122_UpdateFormInstanceType();
                case 17:
                    return new M_202404091622_MigrateFormDependencyModel();
                case 18:
                    return new M_202405231138_DeleteFromInstanceForInactiveEncounters(sReportsContext);
                case 19:
                    return new M_202405281348_MigrateStorageReferences(configuration, sReportsContext);
                case 20:
                    return new M_202501281135_RemovePropertiesFromEntities();
                case 21:
                    return new M_202521071219_CreateIndexes();
                case MongoMigrationsConstants.LastVersion:
                    return new M_202521071219_CreateIndexes();
                default:
                    return null ;
            }
        }

        public void SetToLatestVersion()
        {
            SaveMigrationsVersionFromJsonToDB();

            int currentVersion = LoadLastSavedMigrationVersion();
            int lastSavedVersion = VersionToMigration(MongoMigrationsConstants.LastVersion, configuration, dbContext).Version;

            if (currentVersion < lastSavedVersion)
                UpgradeMigrationVersion(currentVersion, lastSavedVersion);
        }

        public void SetToVersion(int desiredVersion)
        {
            int currentVersion = LoadLastSavedMigrationVersion();

            if (desiredVersion >= currentVersion)
                UpgradeMigrationVersion(currentVersion, desiredVersion);
            else
                DowngradeMigrationVersion(currentVersion, desiredVersion);
        }

        private void UpgradeMigrationVersion(int currentVersion, int desiredVersion)
        {
            for (int i = currentVersion +1; i <= desiredVersion; i++)
            {
                MongoMigration migration = VersionToMigration(i, configuration, dbContext) ?? throw new ArgumentNullException($"Migration version {i} is null");
                migration.ExecuteUp();
                SaveMigrationVersion(migration.Version, migration.GetType().Name);
            }
        }

        private void DowngradeMigrationVersion(int currentVersion, int desiredVersion)
        {
            for (int i = currentVersion; i > desiredVersion; i--)
            {
                MongoMigration migration = VersionToMigration(i, configuration, dbContext) ?? throw new ArgumentNullException($"Migration version {i} is null");
                migration.ExecuteDown();
                SaveMigrationVersion(migration.Version - 1, migration.GetType().Name);
            }
        }

        public void SaveMigrationVersion(int migrationVersion, string name)
        {
            MongoMigrationVersion migrationToSave = new MongoMigrationVersion() { 
                Version = migrationVersion, 
                Name = name,
                EntryDatetime = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            Collection.InsertOne(migrationToSave);
        }

        public int LoadLastSavedMigrationVersion()
        {
            MongoMigrationVersion lastSavedMigration = Collection.Aggregate().SortByDescending(x => x.Version).Limit(1).FirstOrDefault();

            if (lastSavedMigration != null && lastSavedMigration.Version > 0)
                return lastSavedMigration.Version;

            return 0;
        }

        public void SaveMigrationsVersionFromJsonToDB()
        {
            string pathAndFilename = Path.Combine(DirectoryHelper.AppDataFolder, @"MigrationStatus.json");

            if (File.Exists(pathAndFilename))
            {
                string json = File.ReadAllText(pathAndFilename);
                int version = JsonConvert.DeserializeObject<int>(json);

                // If a Migration with the same Version already exists, MongoWriteException is thrown (because Version is a Unique index)
                try
                {
                    if (version >= 1)
                        SaveMigrationVersion(1, "M_202211221536_ValueLabel");
                }
                catch(MongoWriteException ex) 
                {
                    Console.WriteLine($"MongoWriteException occurred: {ex.Message}");
                }

                try
                {
                    if (version == 2)
                        SaveMigrationVersion(2, "M_202212081410_DateFormat"); 
                }
                catch (MongoWriteException ex) 
                {
                    Console.WriteLine($"MongoWriteException occurred: {ex.Message}");
                }
            }
        }
    }
}
