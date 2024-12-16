using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_20231129091705_SetValueForRenamedFormProperty : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        public override int Version => 8;

        public M_20231129091705_SetValueForRenamedFormProperty()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            var filterDefinition = Builders<Form>.Filter.Empty;
            var update = Builders<Form>.Update.Rename("AvailableForCode", "AvailableForTask");

            _ = Collection.UpdateMany(filterDefinition, update).IsAcknowledged;
        }

        protected override void Down()
        {

        }
    }
}
