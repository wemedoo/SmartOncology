using sReportsV2.DTOs.DiagnosticReport.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IDiagnosticReportBLL
    {
        Task<DiagnosticReportCreateFromPatientDataOut> GetReportAsync(FormInstanceReloadDataIn dataIn, UserCookieData userCookieData);
    }
}
