using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.UploadPatientData;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class UploadPatientDataDAL : IUploadPatientDataDAL
    {
        private readonly SReportsContext context;
        public UploadPatientDataDAL(SReportsContext context)
        {
            this.context = context;
        }

        public Task<UploadPatientData> GetById(int uploadPatientDataId)
        {
            return context.UploadPatientData.FirstOrDefaultAsync(x => x.UploadPatientDataId == uploadPatientDataId);
        }

        public void InsertOrUpdate(List<UploadPatientData> uploadPatientData)
        {
            foreach (var uploadPatientDataItem in uploadPatientData) {
                if (uploadPatientDataItem.UploadPatientDataId == 0)
                {
                    context.UploadPatientData.Add(uploadPatientDataItem);
                }
                else
                {
                    uploadPatientDataItem.SetLastUpdate();
                }
            }

            context.SaveChanges();
        }

        public async Task<PaginationData<UploadPatientData>> GetAllAndCount(UploadPatientDataFilter filter)
        {
            IQueryable<UploadPatientData> result = GetUploadPatientDataFiltered(filter);

            int count = await result.CountAsync().ConfigureAwait(false);

            result = ApplyOrderByAndPagination(filter, result);

            return new PaginationData<UploadPatientData>(count, await result.ToListAsync().ConfigureAwait(false));
        }

        private IQueryable<UploadPatientData> GetUploadPatientDataFiltered(UploadPatientDataFilter filter)
        {
            IQueryable<UploadPatientData> query = this.context.UploadPatientData
                .Include(x => x.CreatedBy)
                .WhereEntriesAreActive();

            if (!string.IsNullOrEmpty(filter.NameGiven))
            {
                query = query.Where(x => x.CreatedBy.FirstName != null && x.CreatedBy.FirstName.Contains(filter.NameGiven));
            }

            if (!string.IsNullOrEmpty(filter.NameFamily))
            {
                query = query.Where(x => x.CreatedBy.LastName != null && x.CreatedBy.LastName.Contains(filter.NameFamily));
            }

            if (filter.DateTimeFrom.HasValue)
            {
                query = query.Where(q => q.EntryDatetime >= filter.DateTimeFrom.Value);
            }

            if (filter.DateTimeTo.HasValue)
            {
                query = query.Where(q => q.EntryDatetime <= filter.DateTimeTo.Value);
            }

            return query;
        }

        private IQueryable<UploadPatientData> ApplyOrderByAndPagination(UploadPatientDataFilter filter, IQueryable<UploadPatientData> query)
        {
            if (filter.ColumnName != null)
                query = SortByField(query, filter);
            else
                query = query.OrderByDescending(x => x.UploadPatientDataId)
                    .Skip(filter.GetHowManyElementsToSkip())
                    .Take(filter.PageSize);
            return query;
        }

        private IQueryable<UploadPatientData> SortByField(IQueryable<UploadPatientData> result, UploadPatientDataFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.NameGiven:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.CreatedBy.FirstName)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => x.CreatedBy.FirstName)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                case AttributeNames.NameFamily:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.CreatedBy.LastName)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => x.CreatedBy.LastName)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
            }
        }

    }
}
