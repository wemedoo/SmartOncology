using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Helpers;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202501281135_RemovePropertiesFromEntities : MongoMigration
    {
        public override int Version => 20;

        protected override void Up()
        {
            RemoveNotUsedPropertiesInAuditLogs();
            RemoveNotUsedPropertiesInFormEntries().Wait();
        }

        private void RemoveNotUsedPropertiesInAuditLogs()
        {
            var filterDefinition = Builders<AuditLog>.Filter.Empty;
            var update = Builders<AuditLog>.Update
                .Unset("EntryDatetime")
                .Unset("IsDeleted")
                .Unset("LastUpdate");

            IMongoCollection<AuditLog> auditLogCollection = MongoDBInstance.Instance.GetDatabase().GetCollection<AuditLog>(MongoCollectionNames.AuditLog);
            _ = auditLogCollection.UpdateMany(filterDefinition, update).IsAcknowledged;
        }

        private async Task RemoveNotUsedPropertiesInFormEntries()
        {
            try
            {
                var allFormsFilter = Builders<Form>.Filter.Empty;
                var findOptions = new FindOptions<Form, Form>() { };
                var instancesToWrite = new List<WriteModel<Form>>();

                IMongoCollection<Form> formCollection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);

                using (var cursor = await formCollection.FindAsync(allFormsFilter, findOptions).ConfigureAwait(false))
                {
                    while (await cursor.MoveNextAsync().ConfigureAwait(false))
                    {
                        var batch = cursor.Current;

                        foreach (Form form in batch)
                        {
                            var replaceFilter = Builders<Form>.Filter.Eq(x => x.Id, form.Id);
                            instancesToWrite.Add(new ReplaceOneModel<Form>(replaceFilter, form));
                        }

                        if (instancesToWrite.Any())
                        {
                            var result = await formCollection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                            if (!result.IsAcknowledged)
                                throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {instancesToWrite.Count}");

                            instancesToWrite.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Eror while MigrateFieldDependecyNewModel, error: " + ex.Message);
            }
        }

        protected override void Down()
        {
        }
    }
}
