using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_20231228100405_FormMultipleOrganizations : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        public override int Version => 11;

        public M_20231228100405_FormMultipleOrganizations()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            var filterDefinition = Builders<Form>.Filter.Empty;
            var pipeline = new EmptyPipelineDefinition<Form>()
                .AppendStage<Form, Form, Form>("{$set: {'OrganizationIds': ['$OrganizationId']}}")
                .AppendStage<Form, Form, Form>("{$unset: 'OrganizationId'}")
                ;

            var update = Builders<Form>.Update.Pipeline(pipeline);

            _ = Collection.UpdateMany(filterDefinition, update).IsAcknowledged;
        }

        protected override void Down()
        {
            var filterDefinition = Builders<Form>.Filter.Empty;
            var pipeline = new EmptyPipelineDefinition<Form>()
                .AppendStage<Form, Form, Form>("{$set: {'OrganizationId': {$arrayElemAt: ['$OrganizationIds', 0]}}}")
                .AppendStage<Form, Form, Form>("{$unset: 'OrganizationIds'}")
                ;

            var update = Builders<Form>.Update.Pipeline(pipeline);

            _ = Collection.UpdateMany(filterDefinition, update).IsAcknowledged;
        }
    }
}