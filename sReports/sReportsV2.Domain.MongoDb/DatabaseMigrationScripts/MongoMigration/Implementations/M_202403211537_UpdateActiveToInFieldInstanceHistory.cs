using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202403211537_UpdateActiveToInFieldInstanceHistory : MongoMigration
    {
        private readonly IMongoCollection<FieldInstanceHistory> CollectionFieldInstance;
        public override int Version => 15;

        public M_202403211537_UpdateActiveToInFieldInstanceHistory()
        {
            CollectionFieldInstance = MongoDBInstance.Instance.GetDatabase().GetCollection<FieldInstanceHistory>(MongoCollectionNames.FieldInstanceHistory);
        }

        protected override void Up()
        {
            UpdateActiveTo().Wait();
        }

        protected override void Down()
        {

        }

        private async Task UpdateActiveTo()
        {
            var allFormInstancesWithValuesFilter = Builders<FieldInstanceHistory>.Filter.Eq(x => x.ActiveTo, null);
            var instancesToWrite = new List<WriteModel<FieldInstanceHistory>>();

            var documents = await CollectionFieldInstance
                .Find(allFormInstancesWithValuesFilter)
                .SortBy(x => x.FieldInstanceRepetitionId)
                .ThenBy(x => x.EntryDatetime)
                .ToListAsync()
                .ConfigureAwait(false);

            var groupedRecords = documents.GroupBy(x => x.FieldInstanceRepetitionId);

            foreach (var group in groupedRecords)
            {
                var records = group.ToList();

                for (int i = 0; i < records.Count - 1; i++)
                {
                    records[i].ActiveTo = records[i + 1].ActiveFrom;
                    records[i].LastUpdate = records[i + 1].ActiveFrom;
                }
                records.Last().ActiveTo = null;
                records.Last().LastUpdate = null;

                instancesToWrite.AddRange(records.Select(record =>
                    new ReplaceOneModel<FieldInstanceHistory>(
                        Builders<FieldInstanceHistory>.Filter.Eq(x => x.Id, record.Id),
                        record
                    )
                ));
            }

            if (instancesToWrite.Any())
            {
                var result = await CollectionFieldInstance.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                if (!result.IsAcknowledged)
                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {instancesToWrite.Count}");

            }
        }
    }
}
