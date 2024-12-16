using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ApiRequest;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class AdministrationApiDAL : IAdministrationApiDAL
    {
        private readonly SReportsContext context;
        public AdministrationApiDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task<List<ApiRequestLog>> GetAll(AdministrationApiFilter administrationApiFilter)
        {
            IQueryable<ApiRequestLog> result = GetApiRequestFiltered(administrationApiFilter);

            if (administrationApiFilter.ColumnName != null)
            {
                result = SortTableHelper.OrderByField(result, administrationApiFilter.ColumnName, administrationApiFilter.IsAscending)
                     .Skip((administrationApiFilter.Page - 1) * administrationApiFilter.PageSize)
                     .Take(administrationApiFilter.PageSize);
            }
            else
            {
                result = result.OrderByDescending(x => x.ResponseTimestamp)
                     .Skip((administrationApiFilter.Page - 1) * administrationApiFilter.PageSize)
                     .Take(administrationApiFilter.PageSize);
            }

            return await result.ToListAsync();
        }

        public async Task<long> GetAllFilteredCount(AdministrationApiFilter administrationApiFilter)
        {
            return await GetApiRequestFiltered(administrationApiFilter).CountAsync();
        }

        public async Task<ApiRequestLog> ViewLog(int apiRequestLogId)
        {
            return await context.ApiRequestLogs.FirstOrDefaultAsync(a => a.ApiRequestLogId == apiRequestLogId);
        }

        private IQueryable<ApiRequestLog> GetApiRequestFiltered(AdministrationApiFilter filterData)
        {
            IQueryable<ApiRequestLog> query = context.ApiRequestLogs
                .Where(apiRequestLog => (filterData.ApiRequestDirection == null || filterData.ApiRequestDirection == apiRequestLog.ApiRequestDirection)
                    && (filterData.HttpStatusCode == null || filterData.HttpStatusCode == apiRequestLog.HttpStatusCode)
                    && (filterData.RequestTimestampFrom == null || filterData.RequestTimestampFrom <= apiRequestLog.RequestTimestamp)
                    && (filterData.RequestTimestampTo == null || apiRequestLog.RequestTimestamp <= filterData.RequestTimestampTo)
                    && (string.IsNullOrEmpty(filterData.ApiName) || apiRequestLog.ApiName == filterData.ApiName)
                    && (string.IsNullOrEmpty(filterData.RequestContains) || apiRequestLog.RequestPayload.Contains(filterData.RequestContains) || apiRequestLog.ResponsePayload.Contains(filterData.RequestContains))
                )
            ;

            if (filterData.ShowOnlyUnsuccessful)
            {
                query = query.Where(apiRequestLog => apiRequestLog.HttpStatusCode > 201);
            }

            return query;
        }
    }
}
