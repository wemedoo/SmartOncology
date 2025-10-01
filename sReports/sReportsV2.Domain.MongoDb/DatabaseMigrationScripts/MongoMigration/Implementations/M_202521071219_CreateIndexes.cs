using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202521071219_CreateIndexes : MongoMigration
    {
        public override int Version => 21;

        private readonly IMongoCollection<FormInstance> FormInstanceCollection;
        private readonly IMongoCollection<FieldInstanceHistory> FieldInstanceHistoryCollection;
        private readonly IMongoCollection<AuditLog> AuditLogCollection;

        public M_202521071219_CreateIndexes()
        {
            var database = MongoDBInstance.Instance.GetDatabase();
            FormInstanceCollection = database.GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
            FieldInstanceHistoryCollection = database.GetCollection<FieldInstanceHistory>(MongoCollectionNames.FieldInstanceHistory);
            AuditLogCollection = database.GetCollection<AuditLog>(MongoCollectionNames.AuditLog);
        }

        protected override void Up()
        {
            // Indexes for FormInstance
            var formInstanceIndexes = new List<CreateIndexModel<FormInstance>>
            {
                new(CreateIndexKeys.FormDefinitionId, new CreateIndexOptions { Name = "FormDefinitionId_1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("EncounterRef")
                    .Ascending("OrganizationId")
                    .Ascending("IsDeleted"),
                    new CreateIndexOptions { Name = "EncounterRef_1_OrganizationId_1_IsDeleted_1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("PatientId")
                    .Ascending("Title"),
                    new CreateIndexOptions { Name = "PatientId_1_Title_1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Descending("UserId"),
                    new CreateIndexOptions { Name = "UserId_-1" }),

                new(
                    Builders<FormInstance>.IndexKeys.Text("FieldInstances.FieldInstanceValues.ValueLabel")
                                             .Text("Fields.ValueLabel"),
                    new CreateIndexOptions
                    {
                        Name = "Combined_Text_Index",
                        Weights = new BsonDocument
                        {
                            { "FieldInstances.FieldInstanceValues.ValueLabel", 1 },
                            { "Fields.ValueLabel", 1 }
                        },
                        DefaultLanguage = "english",
                        LanguageOverride = "language",
                        TextIndexVersion = 3
                    }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("IsDeleted")
                    .Ascending("ThesaurusId")
                    .Ascending("Version._id"),
                    new CreateIndexOptions { Name = "By_FormDefinition" }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("ThesaurusId")
                    .Ascending("Version._id")
                    .Ascending("IsDeleted")
                    .Descending("EntryDatetime"),
                    new CreateIndexOptions { Name = "ThesaurusId_1_Version._id_1_IsDeleted_1_EntryDatetime_-1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("Title"),
                    new CreateIndexOptions { Name = "Title_1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Ascending("EpisodeOfCareRef")
                    .Ascending("OrganizationId"),
                    new CreateIndexOptions { Name = "EpisodeOfCareRef_1_OrganizationId_1" }),

                new(Builders<FormInstance>.IndexKeys
                    .Descending("IsDeleted")
                    .Descending("FormDefinitionId"),
                    new CreateIndexOptions { Name = "IsDeleted_-1_FormDefinitionId_-1" })
            };

            FormInstanceCollection.Indexes.CreateMany(formInstanceIndexes);

            // Indexes for FieldInstanceHistory
            var fieldInstanceIndexes = new List<CreateIndexModel<FieldInstanceHistory>>
            {
                new(CreateIndexKeys.FieldInstanceRepetitionIdFormInstanceIdActiveTo,
                    new CreateIndexOptions { Name = "FieldInstanceRepetitionId_1_FormInstanceId_1_ActiveTo_1" }),

                new(Builders<FieldInstanceHistory>.IndexKeys
                    .Ascending("FormInstanceId")
                    .Ascending("IsDeleted"),
                    new CreateIndexOptions { Name = "FormInstanceId_1_IsDeleted_1" })
            };

            FieldInstanceHistoryCollection.Indexes.CreateMany(fieldInstanceIndexes);

            // Indexes for AuditLog
            var auditIndexKeys = Builders<AuditLog>.IndexKeys.Descending(x => x.Time);
            var indexModel = new CreateIndexModel<AuditLog>(auditIndexKeys);

            AuditLogCollection.Indexes.CreateOne(indexModel);
        }

        protected override void Down()
        {
            // Drop FormInstance indexes
            var formInstanceIndexNames = new[]
                {
                "FormDefinitionId_1",
                "EncounterRef_1_OrganizationId_1_IsDeleted_1",
                "PatientId_1_Title_1",
                "UserId_-1",
                "Combined_Text_Index",
                "By_FormDefinition",
                "ThesaurusId_1_Version._id_1_IsDeleted_1_EntryDatetime_-1",
                "Title_1",
                "EpisodeOfCareRef_1_OrganizationId_1",
                "IsDeleted_-1_FormDefinitionId_-1"
            };

            foreach (var indexName in formInstanceIndexNames)
            {
                FormInstanceCollection.Indexes.DropOne(indexName);
            }

            // Drop FieldInstanceHistory indexes
            var fieldInstanceIndexNames = new[]
            {
                "FieldInstanceRepetitionId_1_FormInstanceId_1_ActiveTo_1",
                "FormInstanceId_1_IsDeleted_1"
            };

            foreach (var indexName in fieldInstanceIndexNames)
            {
                FieldInstanceHistoryCollection.Indexes.DropOne(indexName);
            }

            // Drop AuditLog indexes
            AuditLogCollection.Indexes.DropOne("Time_-1");
        }

        private static class CreateIndexKeys
        {
            public static IndexKeysDefinition<FormInstance> FormDefinitionId =>
                Builders<FormInstance>.IndexKeys.Ascending(x => x.FormDefinitionId);

            public static IndexKeysDefinition<FieldInstanceHistory> FieldInstanceRepetitionIdFormInstanceIdActiveTo =>
                Builders<FieldInstanceHistory>.IndexKeys
                    .Ascending("FieldInstanceRepetitionId")
                    .Ascending("FormInstanceId")
                    .Ascending("ActiveTo");
        }
    }
}