using sReportsV2.DTOs.AdministrationApi.DataIn;
using sReportsV2.DTOs.AdministrationApi.DataOut;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IAdministrationApiBLL
    {
        Task<PaginationDataOut<AdministrationApiDataOut, DataIn>> ReloadTable(AdministrationApiFilterDataIn dataIn);
        Task<AdministrationApiDataOut> ViewLog(int apiRequestLogId);
    }
}
