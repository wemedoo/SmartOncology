using MongoDB.Driver;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using sReportsV2.SqlDomain.Implementations;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202405231138_DeleteFromInstanceForInactiveEncounters : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly SReportsContext dbContext;

        public override int Version => 18;

        public M_202405231138_DeleteFromInstanceForInactiveEncounters(SReportsContext dbContext)
        {
            this.dbContext = dbContext;
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>("forminstance");
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
                EncounterDAL encounterDAL = new EncounterDAL(dbContext);
                var encounters = encounterDAL.GetAllInactive();
                var tasks = encounters.Select(async encounter =>
                {
                    var filter = Builders<FormInstance>.Filter.Eq(x => x.EncounterRef, encounter.EncounterId)
                                & Builders<FormInstance>.Filter.Eq(x => x.IsDeleted, false);

                    var matchingFormInstancesCount = await Collection.CountDocumentsAsync(filter);

                    if (matchingFormInstancesCount > 0)
                    {
                        var update = Builders<FormInstance>.Update.Set(x => x.IsDeleted, true);
                        await Collection.UpdateManyAsync(filter, update);
                    }
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error while MigrateFormInstancesToNewModel, error: {ex.Message}, stack trace: {ex.StackTrace}");
            }
        }
    }
}
