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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.Cache.Resources;
using sReportsV2.Common.JsonModelBinder;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportController : DiagnosticReportCommonController
    {
        public DiagnosticReportController(IPatientDAL patientDAL, 
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
            base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, diagnosticReportBLL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration) 
        {
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        public ActionResult CreateFromPatient(int patientId, int encounterId, int episodeOfCareId, string formId, List<string> referrals)
        {
            Form form = formDAL.GetForm(formId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formId);
            }

            List<Form> formReferrals = GetRefeerals(referrals);
            form.SetValuesFromReferrals(formReferrals);
            FormDataOut formOut = GetDataOutForCreatingNewFormInstance(form, formReferrals);

            ViewBag.EncounterId = encounterId;
            ViewBag.Action = GetSubmitActionEndpoint(EndpointConstants.Create, episodeOfCareId, patientId);
            ViewBag.Referrals = referrals;
            ViewBag.NotApplicableId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.NullFlavor, ResourceTypes.NotApplicable);
            SetPatientViewBags();
            return PartialView("~/Views/FormInstance/FormInstancePartial.cshtml", formOut);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        [HttpPost]
        public async Task<ActionResult> CreateFromPatient([ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn = null)
        {
            return await CreateOrEdit(formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.UpdateDocument, Module = ModuleNames.Patients)]
        [HttpPost]
        public async Task<ActionResult> EditFromPatient([ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn)
        {
            return await CreateOrEdit(formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.ViewDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ShowFormInstanceDetails(FormInstanceReloadDataIn dataIn)
        {
            return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/FormInstancePartial.cshtml").ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetFormInstanceContent(FormInstanceReloadDataIn dataIn)
        {
            switch (dataIn.ViewMode)
            {
                case FormInstanceViewMode.SynopticView:
                    return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/SynopticView.cshtml").ConfigureAwait(false);
                default:
                    SetIsHiddenFieldsShown(dataIn.HiddenFieldsShown);
                    return await GetEditFormInstanceFromPatient(dataIn, "~/Views/FormInstance/FormInstanceContent.cshtml").ConfigureAwait(false);
            }
        }

        [HttpDelete]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.DeleteDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> Delete(string formInstanceId, DateTime lastUpdate)
        {
            return await DeleteFormInstance(formInstanceId, lastUpdate).ConfigureAwait(false);
        }
    }
}