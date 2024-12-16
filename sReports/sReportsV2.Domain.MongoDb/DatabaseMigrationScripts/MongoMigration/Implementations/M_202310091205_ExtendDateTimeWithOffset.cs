using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202310091205_ExtendDateTimeWithOffset : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;
        public override int Version => 6;

        public M_202310091205_ExtendDateTimeWithOffset()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
        }

        protected override void Up()
        {
            MigrateDateTimeToDateTimeWithOffset().Wait();
        }

        protected override void Down()
        {

        }

        private async Task MigrateDateTimeToDateTimeWithOffset() 
        {
            var fieldFilters = Builders<FormInstance>.Filter.Empty;
            var findOptions = new FindOptions<FormInstance, FormInstance>() { };
            var instancesToWrite = new List<WriteModel<FormInstance>>();

            using (var cursor = await Collection.FindAsync(fieldFilters, findOptions).ConfigureAwait(false))
            {
                while (await cursor.MoveNextAsync().ConfigureAwait(false))
                {
                    var batch = cursor.Current;

                    batch.Where(forminstance => forminstance.FieldInstances != null)
                        .SelectMany(forminstance => forminstance.FieldInstances)
                        .Where(fieldValue => fieldValue.Type == FieldTypes.Datetime).ToList()
                        .ForEach(fieldValue =>
                        {
                            if (fieldValue.FieldInstanceValues != null)
                            {
                                for (int i = 0; i < fieldValue.FieldInstanceValues.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(fieldValue.FieldInstanceValues[i].Value))
                                    {
                                        fieldValue.FieldInstanceValues[i].ValueLabel = fieldValue.FieldInstanceValues[i].ValueLabel + "+01:00";
                                        fieldValue.FieldInstanceValues[i].Value = fieldValue.FieldInstanceValues[i].Value + "+01:00";
                                    }
                                }
                            }
                        });

                    //Replacing the modified FormInstances into MongoDB Collection
                    foreach (FormInstance formInstance in batch)
                    {
                        var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                        instancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance));
                    }

                    if (instancesToWrite.Any())
                    {
                        var result = await Collection.BulkWriteAsync(instancesToWrite);
                        if (!result.IsAcknowledged)
                            throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {batch.Count()}");
                    }
                    

                    instancesToWrite.Clear();
                }
            }
        }
    }
}
