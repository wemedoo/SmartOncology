using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202312271038_RenameOmniaId : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        public override int Version => 10;

        public M_202312271038_RenameOmniaId()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            var filterDefinition = Builders<Form>.Filter.Empty;
            var update = Builders<Form>.Update.Rename("OmniaId", "OomniaId");

            _ = Collection.UpdateMany(filterDefinition, update).IsAcknowledged;
        }

        protected override void Down()
        {

        }
    }
}
