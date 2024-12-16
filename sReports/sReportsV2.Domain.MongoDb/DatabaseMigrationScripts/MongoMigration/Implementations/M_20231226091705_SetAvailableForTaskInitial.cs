using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_20231226091705_SetAvailableForTaskInitial : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        private readonly ITaskDAL taskDAL;
        public override int Version => 9;

        public M_20231226091705_SetAvailableForTaskInitial(SReportsContext dbContext)
        {
            taskDAL = new TaskDAL(dbContext);
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            UpdateForms();
        }

        protected override void Down()
        {
        }

        // ---------- Helper Methods ----------


        private void UpdateForms()
        {
            foreach (var taskDocument in taskDAL.GetAllTaskDocuments()) 
            {
                var filter = Builders<Form>.Filter.Eq(x => x.Id, taskDocument.FormId)
                  & Builders<Form>.Filter.Eq(x => x.State, Common.Enums.FormDefinitionState.ReadyForDataCapture)
                  & Builders<Form>.Filter.Eq(x => x.IsDeleted, false);

                var update = Builders<Form>.Update.Set(x => x.AvailableForTask, true);

                Collection.UpdateOne(filter, update);
            }
        }
    }
}