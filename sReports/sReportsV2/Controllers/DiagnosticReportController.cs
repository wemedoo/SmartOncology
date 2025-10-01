using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.Cache.Resources;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.DiagnosticReport.DataIn;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportController : DiagnosticReportCommonController
    {
        public DiagnosticReportController(IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL,
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IDiagnosticReportBLL diagnosticReportBLL, 
            IAsyncRunner asyncRunner, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService) : 
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, diagnosticReportBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService) 
        {
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        public ActionResult CreateFromPatient(CreateFromPatientDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            Form form = formBLL.GetFormById(dataIn.FormId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, dataIn.FormId);
            }
            
            FormDataOut formOut = GetDataOutForCreatingNewFormInstance(form);

            ViewBag.EncounterId = dataIn.EncounterId;
            ViewBag.Action = GetSubmitActionEndpoint(EndpointConstants.Create, dataIn.EpisodeOfCareId, dataIn.PatientId);
            ViewBag.Referrals = dataIn.Referrals;
            ViewBag.NotApplicableId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.NullFlavor, ResourceTypes.NotApplicable);
            SetPatientViewBags();
            HttpContext.Session?.UpdateUserCookieDataInSession(false);
            return PartialView("~/Views/FormInstance/FormInstancePartial.cshtml", formOut);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        [HttpPost]
        public async Task<ActionResult> CreateFromPatient(int episodeOfCareId, int patientId, [ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn = null)
        {
            return await CreateOrEdit(episodeOfCareId, patientId, formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.UpdateDocument, Module = ModuleNames.Patients)]
        [HttpPost]
        public async Task<ActionResult> EditFromPatient(int episodeOfCareId, int patientId, [ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn)
        {
            return await CreateOrEdit(episodeOfCareId, patientId, formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.ViewDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ShowFormInstanceDetails(FormInstanceReloadDataIn dataIn)
        {
            return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/FormInstancePartial.cshtml", false).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetFormInstanceContent(FormInstanceReloadDataIn dataIn)
        {
            switch (dataIn.ViewMode)
            {
                case FormInstanceViewMode.SynopticView:
                    return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/SynopticView.cshtml", true).ConfigureAwait(false);
                default:
                    SetIsHiddenFieldsShown(dataIn.HiddenFieldsShown);
                    return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/FormInstanceContent.cshtml", true).ConfigureAwait(false);
            }
        }

        [HttpDelete]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.DeleteDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> Delete(string formInstanceId, DateTime lastUpdate)
        {
            return await DeleteFormInstance(formInstanceId, lastUpdate).ConfigureAwait(false);
        }

        private async Task<ActionResult> CreateOrEdit(int episodeOfCareId, int patientId, FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn.EpisodeOfCareId = episodeOfCareId;
            formInstanceDataIn.PatientId = patientId;
            return await CreateOrEdit(formInstanceDataIn);
        }
    }
}