using MongoDB.Driver;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FieldInstanceHistoryDAL : IFieldInstanceHistoryDAL
    {
        private readonly IMongoCollection<FieldInstanceHistory> Collection;

        public FieldInstanceHistoryDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FieldInstanceHistory>(MongoCollectionNames.FieldInstanceHistory);
        }

        public async Task InsertManyAsync(List<FieldInstanceHistory> fieldInstanceHistories)
        {
            if(fieldInstanceHistories!= null && fieldInstanceHistories.Count > 0)
                await Collection.InsertManyAsync(fieldInstanceHistories).ConfigureAwait(false);
        }

        public async Task<int> UpdateManyAsync(List<FieldInstanceHistory> fieldInstanceHistories)
        {
            await UpdateOldHistory(fieldInstanceHistories).ConfigureAwait(false);
            return await InsertBatch(Collection, fieldInstanceHistories).ConfigureAwait(false);
        }

        public async Task<List<FieldInstanceHistory>> GetAllFilteredAsync(FieldInstanceHistoryFilterData filter)
        {
            IMongoQueryable<FieldInstanceHistory> query = Collection.AsQueryable();

            query = FilterFieldInstanceHistories(query, filter);
            query = OrderByFilter(query, filter);
            query = ApplyPagingByFilter(query, filter);

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<int> CountFilteredAsync(FieldInstanceHistoryFilterData filter)
        {
            IMongoQueryable<FieldInstanceHistory> query = Collection.AsQueryable();
            query = FilterFieldInstanceHistories(query, filter);

            return await query.CountAsync().ConfigureAwait(false);
        }

        private async Task<int> UpdateOldHistory(List<FieldInstanceHistory> fieldInstanceHistories)
        {
            int updatedNumber = 0;
            string formInstanceId = fieldInstanceHistories.FirstOrDefault()?.FormInstanceId ?? string.Empty;
            List<WriteModel<FieldInstanceHistory>> instancesToWrite = new List<WriteModel<FieldInstanceHistory>>();

            foreach (FieldInstanceHistory fieldInstanceHistory in fieldInstanceHistories)
            {
                fieldInstanceHistory.SetLastUpdate();
                FilterDefinition<FieldInstanceHistory> filter =
                    Builders<FieldInstanceHistory>.Filter.Eq(x => x.FieldInstanceRepetitionId, fieldInstanceHistory.FieldInstanceRepetitionId)
                    & Builders<FieldInstanceHistory>.Filter.Eq(x => x.FormInstanceId, formInstanceId)
                    & Builders<FieldInstanceHistory>.Filter.Eq(x => x.ActiveTo, null);

                UpdateDefinition<FieldInstanceHistory> update = Builders<FieldInstanceHistory>.Update
                             .Set(x => x.ActiveTo, DateTime.Now)
                             .Set(x => x.LastUpdate, DateTime.Now);
                instancesToWrite.Add(new UpdateManyModel<FieldInstanceHistory>(filter, update));
            }
            if (instancesToWrite.Count > 0)
            {
                BulkWriteResult<FieldInstanceHistory> result = await Collection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                updatedNumber = (int)result.InsertedCount;
                if (!result.IsAcknowledged)
                    throw new InvalidOperationException($"BulkWriteAsync updated {result.InsertedCount} items instead of {fieldInstanceHistories.Count}");
            }
            return updatedNumber;
        }

        private IMongoQueryable<FieldInstanceHistory> FilterFieldInstanceHistories(IMongoQueryable<FieldInstanceHistory> query, FieldInstanceHistoryFilterData filter)
        {
            if (filter.IncludeIsDeletedInQuery)
            {
                query = query.Where(f => !f.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(filter.FormInstanceId))
            {
                query = query.Where(f => f.FormInstanceId == filter.FormInstanceId);
            }

            if (!string.IsNullOrWhiteSpace(filter.FieldInstanceRepetitionId))
            {
                query = query.Where(f => f.FieldInstanceRepetitionId == filter.FieldInstanceRepetitionId);
            }

            if (!string.IsNullOrWhiteSpace(filter.FieldLabel))
            {
                query = query.Where(f => f.FieldLabel.ToLower().Contains(filter.FieldLabel.ToLower()));
            }

            if (filter.UserId != null)
            {
                query = query.Where(f => f.UserId == filter.UserId);
            }

            return query;
        }

        private IMongoQueryable<FieldInstanceHistory> OrderByFilter(IMongoQueryable<FieldInstanceHistory> query, FieldInstanceHistoryFilterData filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.ColumnName))
            {
                IOrderedMongoQueryable<FieldInstanceHistory> orderedQuery = null;
                switch (filter.ColumnName)
                {
                    case FieldInstanceHistoryConstants.FieldLabel:
                        orderedQuery = filter.IsAscending ? query.OrderBy(x => x.FieldLabel) : query.OrderByDescending(x => x.FieldLabel);
                        break;
                    case FieldInstanceHistoryConstants.FieldSetLabel:
                        orderedQuery = filter.IsAscending ? query.OrderBy(x => x.FieldSetLabel) : query.OrderByDescending(x => x.FieldSetLabel);
                        break;
                }
                query = orderedQuery.ThenBy(x => x.FieldLabel);
            }
            return query;
        }

        private IMongoQueryable<FieldInstanceHistory> ApplyPagingByFilter(IMongoQueryable<FieldInstanceHistory> query, FieldInstanceHistoryFilterData filter)
        {
            if (filter.Page > 0 && filter.PageSize > 0)
            {
                query = query.Skip((filter.Page - 1) * filter.PageSize);
                query = query.Take(filter.PageSize);
            }
            return query;
        }

        private async Task<int> InsertBatch(IMongoCollection<FieldInstanceHistory> collection, List<FieldInstanceHistory> fieldInstanceHistories)
        {
            int insertedNumber = 0;
            List<WriteModel<FieldInstanceHistory>> instancesToWrite = new List<WriteModel<FieldInstanceHistory>>();

            fieldInstanceHistories.ForEach(fieldInstanceHistory => instancesToWrite.Add(new InsertOneModel<FieldInstanceHistory>(fieldInstanceHistory)));

            if (instancesToWrite.Count > 0)
            {
                BulkWriteResult<FieldInstanceHistory> result = await collection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                insertedNumber = (int)result.InsertedCount;
                if (!result.IsAcknowledged)
                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {fieldInstanceHistories.Count}");
            }
            return insertedNumber;
        }
    }
}
