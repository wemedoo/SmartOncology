using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.Form.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.BusinessLayer.Interfaces;
using Generator;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using System.Threading.Tasks;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;

namespace sReportsV2.Controllers
{
    [SReportsAuthorize]
    public partial class FormController : FormCommonController
    {
        private readonly ICommentBLL commentBLL;
        private readonly IConsensusBLL consensusBLL;
        private readonly SReportsContext dbContext;

        public FormController(
            IConsensusBLL consensusBLL, 
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL,
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL,
            IFormBLL formBLL, 
            ICommentBLL commentBLL,
            IAsyncRunner asyncRunner, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService,
            SReportsContext dbContext) :
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService)
        {
            this.commentBLL = commentBLL;
            this.consensusBLL = consensusBLL;
            this.dbContext = dbContext;
        }

        [SReportsAuthorize(Module = ModuleNames.Designer, Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetAll(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.ClinicalDomains = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain);
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Module = ModuleNames.Designer, Permission = PermissionNames.View)]
        public ActionResult ReloadTable(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            ViewBag.DocumentPropertiesEnums = mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            return PartialView("FormsTable", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAuthorize(Module = ModuleNames.Engine, Permission = PermissionNames.View)]
        public ActionResult ReloadByFormThesaurusTable(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.WorkOnlyWithValidForms();
            ViewBag.DocumentPropertiesEnums = mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            return PartialView("~/Views/FormInstance/FormsTable.cshtml", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.ChangeState, Module = ModuleNames.Designer)]
        [HttpPut]
        public ActionResult UpdateFormState(UpdateFormStateDataIn updateFormStateDataIn)
        {
            var result = formBLL.UpdateFormState(updateFormStateDataIn, userCookieData);
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.ShowJson, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetFormJson([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            ViewBag.IsReadOnlyViewMode = formDataIn.IsReadOnlyViewMode;
            return PartialView("~/Views/Form/DragAndDrop/FormJson.cshtml", formBLL.GetFormJson(formDataIn));
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewComments, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetAllCommentsByForm(string formId, string taggedCommentId)
        {
            Form form = formBLL.GetFormById(formId);
            List<string> formItemsOrderIds = form.GetIdsFromObject();
            List<FormCommentDataOut> commentsDataOut = commentBLL.GetComentsDataOut(formId, formItemsOrderIds);
            SetViewBagCommentsParameters(taggedCommentId);
            return PartialView("~/Views/Form/DragAndDrop/FormAllComments.cshtml", commentsDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddComment, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult AddCommentSection(string fieldId)
        {
            ViewBag.ItemRef = fieldId;
            ViewBag.CommentStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.CommentState);

            return PartialView("~/Views/Form/DragAndDrop/FormCommentSection.cshtml");
        }

        [SReportsAuthorize(Permission = PermissionNames.AddComment, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult AddComment(FormCommentDataIn commentDataIn)
        {
            commentDataIn = Ensure.IsNotNull(commentDataIn, nameof(commentDataIn));
            commentDataIn.UserId = userCookieData.Id;
            commentBLL.InsertOrUpdate(commentDataIn);

            return GetAllCommentsByForm(commentDataIn.FormRef, null);
        }

        [SReportsAuthorize(Permission = PermissionNames.ChangeCommentStatus, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult SendCommentStatus(int commentId, int? stateCD)
        {
            string formRef = commentBLL.UpdateState(commentId, stateCD);

            return GetAllCommentsByForm(formRef, null);
        }

        [SReportsAuthorize]
        public ActionResult GetDocumentsByThesaurusId(int o4MtId, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, string.Empty);
            ViewBag.TotalAppeareance = formBLL.GetThesaurusAppereanceCount(o4MtId, string.Empty);

            if (thesaurusPageNum != 0)
                return PartialView("Thesaurus/FormThesaurusTreePartial", result);

            return PartialView("Thesaurus/FormThesaurusTree", result);
        }

        [SReportsAuthorize]
        public ActionResult ReloadClinicalDomain(string name)
        {
            List<ClinicalDomainDTO> options = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain)
                .Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToLower().Contains(name.ToLower()))
                .Select(x => new ClinicalDomainDTO()
                {
                    Id = x.Id,
                    Translation = x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage)
                })
                .OrderBy(enm => enm.ToString())
                .ToList();

            return PartialView(
                "~/Views/Form/DragAndDrop/CustomFields/AutocompleteValues.cshtml", 
                new CustomAutocompleteDataOut()
                {
                    ComponentName = "clinicalDomain",
                    Options = options.OrderBy(x => x.Translation).ToDictionary(x => x.Id.ToString(), x => x.Translation),
                }
            );
        }

        [SReportsAuthorize]
        public ActionResult FilterThesaurusTree(int o4MtId, string searchTerm, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, searchTerm, userCookieData);
            ViewBag.TotalAppeareance = formBLL.GetThesaurusAppereanceCount(o4MtId, searchTerm, userCookieData.ActiveOrganization);

            if (thesaurusPageNum != 0)
                return PartialView("Thesaurus/FormThesaurusAppearanceTreePartial", result);

            return PartialView("Thesaurus/FormThesaurusAppearanceTree", result);
        }

        [SReportsAuthorize]
        public ActionResult GetDocumentProperties(string id)
        {
            DocumentProperties result = this.formBLL.GetDocumentProperties(id);
            return PartialView("Thesaurus/DocumentProperties", mapper.Map<DocumentPropertiesDataOut>(result));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Designer)]
        [HttpPost]
        [SReportsFormValidate]
        public ActionResult Create([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn, string formId)
        {
            return CreateOrEdit(formDataIn, formId);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpPost]
        [SReportsFormValidate]
        public ActionResult Edit([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn, string formId)
        {
            return CreateOrEdit(formDataIn, formId);
        }

        [SReportsAuditLog]
        [HttpDelete]
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Designer)]
        public async Task<ActionResult> Delete(string formId, DateTime lastUpdate)
        {
            await formBLL.Delete(formId, lastUpdate, userCookieData.OrganizationTimeZone);
            return NoContent();
        }


        public ActionResult ExportCTCAEForm()
        {
            FormGenerator generator = new FormGenerator(mapper.Map<UserData>(userCookieData));
            Form form = generator.GetFormFromCsv("CTCAE ");
            formBLL.InsertOrUpdate(form, userCookieData, executeBackgroundTask: false);

            return Json(true);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpGet]
        public async Task<ActionResult> GetGenerateNewLanguage(string formId)
        {
            return PartialView("GenerateLanguageModal", await formBLL.GetGenerateNewLanguage(formId, userCookieData.ActiveOrganization).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult GenerateNewLanguage(string formid, string language)
        {
            bool success = formBLL.TryGenerateNewLanguage(formid, language, userCookieData);
            if (success)
                return Json(new { message = TextLanguage.GenerateFormTranslationMessage });
            else
                return NotFound(TextLanguage.FormNotExists, formid);
        }

        public ActionResult GenerateThesauruses(string formId)
        {
            Form form = formBLL.GetFormById(formId);
            ThesaurusGenerator generator = new ThesaurusGenerator(Configuration, dbContext);
            generator.GenerateThesauruses(form);
            formBLL.InsertOrUpdate(form, userCookieData, executeBackgroundTask: false);

            return RedirectToAction(EndpointConstants.Edit, "Form", new { formId });
        }

        [SReportsAuthorize]
        public ActionResult RetrieveUser(string searchWord, string commentId)
        {
            List<UserData> userData = userBLL.GetUsersForCommentTag(searchWord);
            ViewBag.CommentId = commentId;
            return PartialView("~/Views/Form/DragAndDrop/CustomFields/UserValues.cshtml", userData);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        public async Task<ActionResult> ResetCustomHeadersView(string formId)
        {
            SetReadOnlyAndDisabledViewBag(!userCookieData.UserHasPermission(PermissionNames.Update, ModuleNames.Engine));
            FormDataOut formDataOut = (!string.IsNullOrWhiteSpace(formId) && formId != "formIdPlaceHolder") ? mapper.Map<FormDataOut>(await formBLL.GetFormByIdAsync(formId).ConfigureAwait(false)) : new FormDataOut();
            formDataOut.CustomHeaderFields = CustomHeaderFieldDataOut.GetDefaultHeaders();
            return PartialView("~/Views/Form/DragAndDrop/CustomHeaderFields.cshtml", formDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> InsertOrUpdateCustomHeaders(string formId, List<CustomHeaderFieldDataIn> customHeaders)
        {
            await formBLL.InsertOrUpdateCustomHeaderFieldsAsync(formId, customHeaders, userCookieData).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public ActionResult CheckFormula(DependentOnInfoDataIn dataIn)
        {
            return Json(formBLL.ValidateFormula(dataIn));
        }

        [SReportsAuthorize(Module = ModuleNames.Designer, Permission = PermissionNames.GenerateReports)]
        public async Task<ActionResult> GenerateReport(string formId)
        {
            await formBLL.GenerateReport(formId, userCookieData);
            return Json("Success");
        }

        private void SetViewBagCommentsParameters(string taggedCommentId = "")
        {
            ViewBag.TaggedCommentId = taggedCommentId;
            ViewBag.CanAddComment = ViewBag.UserCookieData.UserHasPermission(PermissionNames.AddComment, ModuleNames.Designer);
            ViewBag.CanChangeCommentStatus = ViewBag.UserCookieData.UserHasPermission(PermissionNames.ChangeCommentStatus, ModuleNames.Designer);
            ViewBag.CommentStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.CommentState);
        }

        private ActionResult CreateOrEdit(FormDataIn formDataIn, string formId)
        {
            formDataIn = Ensure.IsNotNull(formDataIn, nameof(formDataIn));

            if (formId != null && !formBLL.ExistsForm(formId))
            {
                return NotFound(TextLanguage.FormNotExists, formId);
            }

            Form form = mapper.Map<Form>(formDataIn);

            form.UserId = userCookieData.Id;
            form.Language = userCookieData.ActiveLanguage;
            form.SetInitialOrganizationId(userCookieData.ActiveOrganization);

            formBLL.DisableActiveFormsIfNewVersion(form, userCookieData);
            formBLL.InsertOrUpdate(form, userCookieData);

            var result = new
            {
                id = form.Id,
                versionId = form.Version.Id,
                thesaurusId = form.ThesaurusId,
                lastUpdate = System.Net.WebUtility.UrlEncode(form.LastUpdate.Value.ToString("o"))
            };

            return Json(result);
        }
    }
}