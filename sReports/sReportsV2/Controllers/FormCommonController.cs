using AutoMapper;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using sReportsV2.DTOs.Common;
using sReportsV2.Cache.Singleton;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController : BaseController
    {
        protected readonly IUserBLL userBLL;
        protected readonly IOrganizationBLL organizationBLL;
        protected readonly ICodeBLL codeBLL;
        protected readonly IFormInstanceBLL formInstanceBLL;
        protected readonly IFormBLL formBLL;
        protected readonly IProjectManagementBLL projectManagementBLL;
        private readonly IPdfBLL pdfBLL;
        protected readonly ICodeAssociationBLL codeAssociationBLL;
        protected readonly IMapper mapper;
        protected readonly IHttpContextAccessor httpContextAccessor;

        public FormCommonController(
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IAsyncRunner asyncRunner, 
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService,
            ICodeAssociationBLL codeAssociationBLL = null, 
            IProjectManagementBLL projectManagementBLL = null) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
            this.formInstanceBLL = formInstanceBLL;
            this.formBLL = formBLL;
            this.codeBLL = codeBLL;
            this.projectManagementBLL = projectManagementBLL;
            this.mapper = mapper;
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

        public string RenderPartialViewToString(string viewName, object model, bool isChapterReadonly, string fieldSetId, bool showResetAndNeSection = true, FormLayoutStyleDataOut layoutStyle = null, bool formInstanceMode = false, bool readOnly = false)
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
            ViewBag.ReadOnly = readOnly;
            ViewBag.Statuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.QueryStatus);

            return this.RenderPartialView(httpContextAccessor, viewName, model, isChapterReadonly, fieldSetId);
        }

        protected void SetViewBagAndMakeResetAndNeSectionHidden()
        {
            ViewBag.DefaultLink = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            ViewBag.ShowResetAndNeSection = false;
        }

        protected ActionResult NotFound(string errorTemplate, string resourceId)
        {
            string errorMessage = string.Format(errorTemplate, resourceId);
            Log.Warning(errorMessage);
            return NotFound(new ErrorDTO(errorMessage));
        }
    }
}