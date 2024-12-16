using sReportsV2.Domain.Sql.Entities.ApiRequest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IAdministrationApiDAL
    {
        Task<List<ApiRequestLog>> GetAll(AdministrationApiFilter administrationApiFilter);
        Task<long> GetAllFilteredCount(AdministrationApiFilter administrationApiFilter);
        Task<ApiRequestLog> ViewLog(int apiRequestLogId);
    }
}
