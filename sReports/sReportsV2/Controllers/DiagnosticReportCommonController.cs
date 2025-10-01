using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.Cache.Resources;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportCommonController : FormCommonController
    {
        private readonly IDiagnosticReportBLL diagnosticReportBLL;

        public DiagnosticReportCommonController(IUserBLL userBLL, 
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
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService) 
        {
            this.diagnosticReportBLL = diagnosticReportBLL;
        }

        protected async Task<ActionResult> GetEditFormInstanceFromPatient(FormInstanceReloadDataIn reloadDataIn, string partialViewName, bool formInstanceLoaded)
        {
            var diagnosticReportCreateFromPatientDataOut = await diagnosticReportBLL.GetReportAsync(reloadDataIn, userCookieData)
               .ConfigureAwait(false);
            var formInstance = await formInstanceBLL.GetByIdAsync(reloadDataIn.FormInstanceId).ConfigureAwait(false);

            ViewBag.FormInstanceId = reloadDataIn.FormInstanceId;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.Referrals = formInstance.Referrals;
            ViewBag.FormInstanceWorkflowHistory = formInstanceBLL.GetWorkflowHistory(formInstance.WorkflowHistory);

            ViewBag.CannotUpdateDocument = !userCookieData.UserHasPermission(PermissionNames.UpdateDocument, ModuleNames.Patients);
            ViewBag.NotApplicableId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.NullFlavor, ResourceTypes.NotApplicable);
            ViewBag.Action = GetSubmitActionEndpoint(EndpointConstants.Edit, diagnosticReportCreateFromPatientDataOut.Encounter.EpisodeOfCareId, diagnosticReportCreateFromPatientDataOut.Encounter.PatientId);
            SetReadOnlyAndDisabledViewBag(reloadDataIn.IsReadOnlyViewMode);
            SetPatientViewBags();
            HttpContext.Session?.UpdateUserCookieDataInSession(formInstanceLoaded);
            return PartialView(partialViewName, diagnosticReportCreateFromPatientDataOut.CurrentForm);
        }

        protected async Task<ActionResult> CreateOrEditFromPatient(int episodeOfCareId, int patientId, FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            Form form = formBLL.GetFormById(formInstanceDataIn.FormDefinitionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn?.FormDefinitionId);
            }
            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);
            formInstance.EncounterRef = formInstanceBLL.GetEncounterFromRequestOrCreateDefault(episodeOfCareId, formInstanceDataIn.EncounterId);
            formInstance.EpisodeOfCareRef = episodeOfCareId;
            formInstance.PatientId = patientId;

            await formInstanceBLL.InsertOrUpdateAsync(
                formInstance,
                formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id),
                userCookieData
                ).ConfigureAwait(false);

            return GetCreateFormInstanceResponseResult(formInstance.Id, form.Version.Id, form.Title);
        }

        protected string GetSubmitActionEndpoint(string action, int episodeOfCareId, int patientId)
        {
            return $"/DiagnosticReport/{action}FromPatient?episodeOfCareId={episodeOfCareId}&patientId={patientId}";
        }

        protected void SetPatientViewBags()
        {
            ViewBag.IsEngineModule = false;
            ViewBag.IsPatientModule = true;
            ViewBag.CollapseChapters = true;
        }
    }
}