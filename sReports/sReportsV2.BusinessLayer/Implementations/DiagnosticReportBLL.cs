using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.DiagnosticReport.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class DiagnosticReportBLL : IDiagnosticReportBLL
    {
        private readonly IFormInstanceBLL formInstanceBLL;
        private readonly IFormBLL formBLL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IMapper Mapper;

        public DiagnosticReportBLL(IEncounterDAL encounterDAL, IFormBLL formBLL, IFormInstanceDAL formInstanceDAL, IFormInstanceBLL formInstanceBLL, IMapper mapper)
        {
            this.encounterDAL = encounterDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.formInstanceBLL = formInstanceBLL;
            this.formBLL = formBLL;
            Mapper = mapper;
        }

        public async Task<DiagnosticReportCreateFromPatientDataOut> GetReportAsync(FormInstanceReloadDataIn dataIn, UserCookieData userCookieData)
        {
            string formInstanceId = Ensure.IsNotNull(dataIn.FormInstanceId, nameof(dataIn.FormInstanceId));

            var formInstance = await formInstanceDAL.GetByIdAsync(formInstanceId);
            var referrals = await formInstanceBLL.GetByIdsAsync(formInstance.Referrals);
            var data = formBLL.GetFormDataOut(formInstance, referrals, userCookieData, dataIn);
            var encounter = encounterDAL.GetByIdAsync(formInstance.EncounterRef);

            DiagnosticReportCreateFromPatientDataOut diagnosticReportCreateFromPatientDataOut = new DiagnosticReportCreateFromPatientDataOut()
            {
                Encounter = Mapper.Map<EncounterDataOut>(await encounter),
                CurrentForm = data,
            };

            return diagnosticReportCreateFromPatientDataOut;
        }
    }
}
