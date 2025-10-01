using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using System;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.QueryManagement;
using sReportsV2.DTOs.DTOs.QueryManagement.DataOut;
using sReportsV2.DTOs.DTOs.QueryManagement.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Common;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.Common.Constants;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class QueryManagementBLL : IQueryManagementBLL
    {
        private readonly IMapper mapper;
        private readonly IQueryManagementDAL queryManagementDAL;
        private readonly IFormInstanceDAL formInstanceDAL;

        public QueryManagementBLL(IMapper mapper, IQueryManagementDAL queryManagementDAL, IFormInstanceDAL formInstanceDAL)
        {
            this.mapper = mapper;
            this.queryManagementDAL = queryManagementDAL;
            this.formInstanceDAL = formInstanceDAL;
        }

        public async Task<QueryDataOut> GetById(int id)
        {
            Query query = await queryManagementDAL.GetById(id).ConfigureAwait(false);
            if (query == null) throw new ArgumentNullException(nameof(id), "Query does not exist");

            return mapper.Map<QueryDataOut>(query);
        }

        public async Task<List<QueryDataOut>> GetListById(int id)
        {
            Query query = await queryManagementDAL.GetById(id).ConfigureAwait(false);
            if (query == null) throw new ArgumentNullException(nameof(id), "Query does not exist");

            var result = new List<QueryDataOut>
            {
                mapper.Map<QueryDataOut>(query)
            };

            var field = GetFieldData(result.First());

            foreach (var item in result)
            {
                item.Field = field;
            }

            return result;
        }

        public async Task<int> Create(QueryDataIn dataIn, int userId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            Query query = mapper.Map<Query>(dataIn);

            return await queryManagementDAL.Create(query, userId).ConfigureAwait(false);
        }

        public async Task<List<int>> Update(List<QueryDataIn> dataInList, int userId)
        {
            Ensure.IsNotNull(dataInList, nameof(dataInList));

            var updatedIds = new List<int>();

            foreach (var dataIn in dataInList)
            {
                var query = mapper.Map<Query>(dataIn);

                var queryDB = await queryManagementDAL.GetById(query.QueryId).ConfigureAwait(false);
                if (queryDB == null) continue;

                queryDB.StatusCD = query.StatusCD;
                queryDB.Comment = string.IsNullOrWhiteSpace(query.Comment) ? null : query.Comment;

                int updatedId = await queryManagementDAL.Update(queryDB, userId, mapper.Map<List<QueryHistory>>(dataIn.History)).ConfigureAwait(false);
                updatedIds.Add(updatedId);
            }

            return updatedIds;
        }

        public async Task Delete(int id)
        {
            await queryManagementDAL.Delete(id);
        }

        public async Task<PaginationDataOut<QueryDataOut, DataIn>> GetAllFiltered(QueryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            QueryFilter filter = mapper.Map<QueryFilter>(dataIn);

            var dataTask = await queryManagementDAL.GetAll(filter);
            var countTask = await queryManagementDAL.GetAllEntriesCount(filter);

            var dataOut = mapper.Map<List<QueryDataOut>>(dataTask);

            foreach (var queryDataOut in dataOut)
            {
                var field = GetFieldData(queryDataOut);
                queryDataOut.Field = field;
            }

            (dataOut, countTask) = ApplyFilteringSortingAndPagination(dataOut, filter, countTask);

            PaginationDataOut<QueryDataOut, DataIn> result = new PaginationDataOut<QueryDataOut, DataIn>()
            {
                Count = countTask,
                Data = dataOut,
                DataIn = dataIn
            };

            return result;
        }

        public async Task<List<QueryDataOut>> GetByFieldId(QueryFilterDataIn dataIn)
        {
            QueryFilter filter = mapper.Map<QueryFilter>(dataIn);
            var queries = await queryManagementDAL.GetByFieldId(filter).ConfigureAwait(false);

            if (queries == null || queries.Count == 0)
                return new List<QueryDataOut>();

            var result = queries.Select(q => mapper.Map<QueryDataOut>(q)).ToList();
            var field = GetFieldData(result.First());

            foreach (var item in result)
            {
                item.Field = field;
            }

            return result;
        }

        private FieldDataOut GetFieldData(QueryDataOut item)
        {
            var formInstance = formInstanceDAL.GetById(item.FormInstanceId);

            var thesaurusId = formInstance.FieldInstances
                .FirstOrDefault(f => f.FieldInstanceValues
                    .Any(v => v.FieldInstanceRepetitionId == item.FieldId))?.ThesaurusId;

            if (!thesaurusId.HasValue)
                return null;

            return mapper.Map<FieldDataOut>(
                formInstanceDAL.GetFieldByThesaurus(formInstance, thesaurusId.Value)
            );
        }

        private (List<QueryDataOut> dataOut, int count) ApplyFilteringSortingAndPagination(List<QueryDataOut> dataOut, QueryFilter filter, int countTask)
        {
            bool needsPagination = !string.IsNullOrEmpty(filter.FieldLabel) ||
                                   (!string.IsNullOrEmpty(filter.ColumnName) && filter.ColumnName == AttributeNames.FieldId);

            if (!string.IsNullOrEmpty(filter.FieldLabel))
            {
                dataOut = dataOut
                    .Where(x => x.Field?.Label != null && x.Field.Label.Contains(filter.FieldLabel, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(filter.ColumnName) && filter.ColumnName == AttributeNames.FieldId)
            {
                dataOut = filter.IsAscending
                    ? dataOut.OrderBy(x => x.Field?.Label ?? string.Empty).ToList()
                    : dataOut.OrderByDescending(x => x.Field?.Label ?? string.Empty).ToList();
            }

            if (needsPagination)
            {
                countTask = dataOut.Count;

                dataOut = dataOut
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();
            }

            return (dataOut, countTask);
        }

    }
}
