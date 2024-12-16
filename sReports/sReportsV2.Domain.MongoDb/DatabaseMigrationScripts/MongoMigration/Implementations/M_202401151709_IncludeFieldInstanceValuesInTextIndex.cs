using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202401151709_IncludeFieldInstanceValuesInTextIndex: MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;
        public override int Version => 12;

        public M_202401151709_IncludeFieldInstanceValuesInTextIndex()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
        }

        protected override void Up()
        {
            var indexName = "Fields_ValueLabel";
            var indexExists = IndexExists(Collection, indexName);

            if (indexExists)
            {
                Collection.Indexes.DropOne("Fields_ValueLabel");
            }

            var keys = Builders<FormInstance>.IndexKeys.Combine(
                Builders<FormInstance>.IndexKeys.Text("Fields.ValueLabel"),
                Builders<FormInstance>.IndexKeys.Text("FieldInstances.FieldInstanceValues.ValueLabel")
            );


            var fieldValueLabelIndex = "Combined_Text_Index";
            var fieldValueLabelIndexExists = IndexExists(Collection, fieldValueLabelIndex);
            if (fieldValueLabelIndexExists)
            {
                Collection.Indexes.DropOne("Combined_Text_Index");
            }

            var options = new CreateIndexOptions { Name = "Combined_Text_Index" };

            Collection.Indexes.CreateOne(new CreateIndexModel<FormInstance>(keys, options));
        }

        protected override void Down()
        {
            Collection.Indexes.DropOne("Combined_Text_Index");

            IndexKeysDefinition<FormInstance> key = Builders<FormInstance>.IndexKeys.Text("Fields.ValueLabel");
            Collection.Indexes.CreateOne(new CreateIndexModel<FormInstance>(key, new CreateIndexOptions() { Name = "Fields_ValueLabel" }));
        }

        private bool IndexExists(IMongoCollection<FormInstance> collection, string indexName)
        {
            var indexes = collection.Indexes.List().ToList();
            foreach (var index in indexes)
            {
                if (index.Contains("name") && index["name"].AsString == indexName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
