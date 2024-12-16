using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using sReportsV2.Cache.Resources;

namespace sReportsV2.Controllers
{
    public class CustomController : DiagnosticReportCommonController
    {
        public CustomController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL,
            IEncounterDAL encounterDAL, 
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IThesaurusDAL thesaurusDAL, 
            IDiagnosticReportBLL diagnosticReportBLL, 
            IAsyncRunner asyncRunner, 
            IPdfBLL pdfBLL,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration) : 
            base(patientDAL, episodeOfCareDAL, encounterDAL ,userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, diagnosticReportBLL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration) { }
        // GET: Custom
        public async Task<ActionResult> CTCAE(int episodeOfCareId, FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            Form form = this.formDAL.GetForm(formInstanceDataIn.FormDefinitionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn.FormDefinitionId);
            }
            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);
            if (episodeOfCareId != 0)
            {
                formInstance.EncounterRef = GetEncounterFromRequestOrCreateDefault(episodeOfCareId, formInstanceDataIn.EncounterId);
                formInstance.EpisodeOfCareRef = episodeOfCareId;
                formInstance.PatientId = episodeOfCareDAL.GetById(episodeOfCareId).PatientId;
            }

            await formInstanceBLL.InsertOrUpdateAsync(
                formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id),
                userCookieData
                ).ConfigureAwait(false);

            return Redirect($"{Configuration["ctcaeUrl"]}?patientId={formInstance.PatientId}&organizationId={userCookieData.ActiveLanguage}&formInstanceId={formInstance.Id}");
        }

    }
}