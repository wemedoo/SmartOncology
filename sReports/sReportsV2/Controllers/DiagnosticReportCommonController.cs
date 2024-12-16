using AutoMapper;
using Serilog;
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
using System.Linq;
using System.Net;
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
        private readonly IMapper Mapper;

        public DiagnosticReportCommonController(IPatientDAL patientDAL, 
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
            base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration) 
        {
            this.diagnosticReportBLL = diagnosticReportBLL;
            Mapper = mapper;
        }

        protected EpisodeOfCareListFormsDataOut GetEpisodeOfCareListFormsDataOut(int episodeOfCareId, List<string> referrals, string encounterId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareDAL.GetById(episodeOfCareId));

            EpisodeOfCareListFormsDataOut episodeOfCareListFormsDataOut = new EpisodeOfCareListFormsDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Forms = Mapper.Map<List<FormEpisodeOfCareDataOut>>(formDAL.GetAllByOrganizationAndLanguage(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage)),
                Patient = Mapper.Map<PatientDataOut>(patientDAL.GetById(episodeOfCareDataOut.PatientId)),
                Referrals = referrals,
                EncounterId = encounterId

            };

            return episodeOfCareListFormsDataOut;
        }

        protected List<Form> GetRefeerals(List<string> referrals)
        {
            List<FormInstance> formInstancesReferrals = referrals != null ? formInstanceDAL.GetByIds(referrals).ToList() : new List<FormInstance>();
            return formBLL.GetFormsFromReferrals(formInstancesReferrals);
        }

        protected async Task<ActionResult> GetEditFormInstanceFromPatient(FormInstanceReloadDataIn reloadDataIn, string partialViewName)
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
            return PartialView(partialViewName, diagnosticReportCreateFromPatientDataOut.CurrentForm);
        }

        protected async Task<ActionResult> CreateOrEditFromPatient(int episodeOfCareId, int patientId, FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            Form form = formDAL.GetForm(formInstanceDataIn.FormDefinitionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn?.FormDefinitionId);
            }
            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);
            formInstance.EncounterRef = GetEncounterFromRequestOrCreateDefault(episodeOfCareId, formInstanceDataIn.EncounterId);
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