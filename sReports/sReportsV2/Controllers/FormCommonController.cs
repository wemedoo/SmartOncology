using AutoMapper;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using sReportsV2.DTOs.Common;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController : BaseController
    {
        protected readonly IFormInstanceDAL formInstanceDAL;
        protected readonly IFormDAL formDAL;
        protected readonly IEncounterDAL encounterDAL;
        protected readonly IUserBLL userBLL;
        protected readonly IOrganizationBLL organizationBLL;
        protected readonly ICodeBLL codeBLL;
        protected readonly IFormInstanceBLL formInstanceBLL;
        protected readonly IFormBLL formBLL;
        public readonly IEpisodeOfCareDAL episodeOfCareDAL;
        public readonly IPatientDAL patientDAL;
        public readonly IThesaurusDAL thesaurusDAL;
        protected readonly IProjectManagementBLL projectManagementBLL;
        private readonly IPdfBLL pdfBLL;
        public readonly ICodeAssociationBLL codeAssociationBLL;
        private readonly IMapper Mapper;
        protected readonly IHttpContextAccessor httpContextAccessor;

        public FormCommonController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL, 
            IEncounterDAL encounterDAL,
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IThesaurusDAL thesaurusDAL, 
            IAsyncRunner asyncRunner, 
            IPdfBLL pdfBLL, 
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICodeAssociationBLL codeAssociationBLL = null, 
            IProjectManagementBLL projectManagementBLL = null) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
            formInstanceDAL = new FormInstanceDAL();
            formDAL = new FormDAL();
            this.encounterDAL = encounterDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.formInstanceBLL = formInstanceBLL;
            this.formBLL = formBLL;
            this.codeBLL = codeBLL;
            this.patientDAL = patientDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.projectManagementBLL = projectManagementBLL;
            this.pdfBLL = pdfBLL;
            Mapper = mapper;
            this.codeAssociationBLL = codeAssociationBLL;
            this.httpContextAccessor = httpContextAccessor;
        }
        public ActionResult GetGuids(int quantity)
        {
            List<string> guids = Enumerable.Range(0, quantity).Select(i => GuidExtension.NewGuidStringWithoutDashes()).ToList();
            return Json(guids);
        }

        protected FormDataOut GetFormDataOut(Form form)
        {
            SetFormStateViewBag();
            return formBLL.GetFormDataOut(form, userCookieData);
        }

        protected void SetFormStateViewBag()
        {
            ViewBag.States = Enum.GetValues(typeof(FormDefinitionState)).Cast<FormDefinitionState>().ToList();
        }

        public string RenderPartialViewToString(string viewName, object model, bool isChapterReadonly, string fieldSetId, bool showResetAndNeSection = true, FormLayoutStyleDataOut layoutStyle = null, bool formInstanceMode = false)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;
            ViewBag.Chapter = isChapterReadonly;
            ViewBag.FieldSetId = fieldSetId;
            ViewBag.ShowResetAndNeSection = showResetAndNeSection;
            ViewBag.UserCookieData = _httpContextAccessor.HttpContext.Session.GetUserFromSession();
            ViewBag.IsMatrixLayout = layoutStyle != null && layoutStyle.LayoutType != null && layoutStyle.LayoutType == LayoutType.Matrix;
            ViewBag.FormInstanceMode = formInstanceMode;

            return this.RenderPartialView(httpContextAccessor, viewName, model, isChapterReadonly, fieldSetId);
        }

        protected void SetViewBagAndMakeResetAndNeSectionHidden()
        {
            ViewBag.DefaultLink = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            ViewBag.ShowResetAndNeSection = false;
        }

        protected int GetOrganizationId(Form form)
        {
            return form.GetActiveOrganizationId(userCookieData.ActiveOrganization);
        }

        protected ActionResult NotFound(string errorTemplate, string resourceId)
        {
            string errorMessage = string.Format(errorTemplate, resourceId);
            Log.Warning(errorMessage);
            return NotFound(new ErrorDTO(errorMessage));
        }
    }
}