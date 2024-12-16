using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using System.Threading.Tasks;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.DTOs.CodeEntry.DataOut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Cache.Resources;
using System;
using System.Linq;

namespace sReportsV2.Controllers
{
    public partial class FormController
    {
        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetPredefinedFormElements()
        {
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/DragAndDrop/PredefinedFormElements.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult CreateDragAndDropFormPartial([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            SetFormDragAndDropPartialViewBags(formDataIn.IsReadOnlyViewMode);
            return PartialView("~/Views/Form/DragAndDrop/DragAndDropFormPartial.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult CreateFormTreeNestable([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            return PartialView("~/Views/Form/DragAndDrop/FormTreeNestable.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }
      
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Designer)]
        public ActionResult Create()
        {
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return View("~/Views/Form/DragAndDrop/Create.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public ActionResult Edit(FormRequestDataIn request)
        {
            return GetViewEditResponse(request);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Designer)]
        public ActionResult View(FormRequestDataIn request)
        {
            return GetViewEditResponse(request);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Designer)]
        public ActionResult GetViewEditResponse(FormRequestDataIn request)
        {
            request = Ensure.IsNotNull(request, nameof(request));
            Form form = this.formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(request.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, request.VersionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, request.ThesaurusId.ToString());
            }
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.Consensus = Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(form.Id));
            ViewBag.ActiveTab = request.ActiveTab;
            SetDiagnosisRoleAndEoCTypesViewBag();
            SetViewBagCommentsParameters(request.TaggedCommentId);
            SetViewBagAndMakeResetAndNeSectionHidden();
            return View("~/Views/Form/DragAndDrop/Create.cshtml", GetFormDataOut(form));
        }

        [SReportsAuditLog]
        [Authorize]
        [HttpPost]
        public ActionResult GetFormTree([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            SetViewBagPreviewTypeParameters(formDataIn.IsReadOnlyViewMode);
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/DragAndDrop/FormTreeContainer.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public async Task<ActionResult> GetFormGeneralInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.NullFlavors = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.NullFlavor);

            int formInstancesCount = (dataIn != null && dataIn.Id != "formIdPlaceHolder") ? formInstanceDAL.CountByDefinition(dataIn.Id) : -1;
            ViewBag.NotUpdateableField = (formInstancesCount > 0);
            ViewBag.ClinicalDomains = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain);

            SetViewBagPreviewTypeParameters(dataIn.IsReadOnlyViewMode);
            SetDiagnosisRoleAndEoCTypesViewBag();

            return PartialView("~/Views/Form/DragAndDrop/FormGeneralInfoTabs.cshtml", await formBLL.GetFormForGeneralInfoAsync(dataIn).ConfigureAwait(false));
        }

        // This endpoint is NOT in use
        /*[SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFormPreview([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn dataIn)
        {
            return PartialView("~/Views/Form/DragAndDrop/FormPartialPreview.cshtml", Mapper.Map<FormDataOut>(dataIn));
        }*/

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetFormAdministrativeData(string formId)
        {
            Form form = formBLL.GetFormById(formId);

            return PartialView("~/Views/Form/DragAndDrop/CustomFields/FormAdministrativeData.cshtml", GetFormDataOut(form).GetWorkflowHistory());
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetChapterInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FormChapterDataIn chapter)
        {
            SetViewBagPreviewTypeParameters(chapter.IsReadOnlyViewMode);
            return PartialView("~/Views/Form/DragAndDrop/ChapterInfoForm.cshtml", Mapper.Map<FormChapterDataOut>(chapter));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetPageInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FormPageDataIn page)
        {
            SetViewBagPreviewTypeParameters(page.IsReadOnlyViewMode);
            return PartialView("~/Views/Form/DragAndDrop/PageInfoForm.cshtml", Mapper.Map<FormPageDataOut>(page));
        }


        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldSetInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FormFieldSetDataIn fieldset)
        {
            SetViewBagPreviewTypeParameters(fieldset.IsReadOnlyViewMode);
            return PartialView("~/Views/Form/DragAndDrop/FieldSetInfoForm.cshtml", Mapper.Map<FormFieldSetDataOut>(fieldset));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FieldDataIn fieldDataIn)
        {
            SetViewBagPreviewTypeParameters(fieldDataIn.IsReadOnlyViewMode);
            SetNullFlavorsViewBag(fieldDataIn.FormId);
            FormDataOut form = Mapper.Map<FormDataOut>(formBLL.GetFormById(fieldDataIn.FormId));
            FieldDataOut model = Mapper.Map<FieldDataOut>(fieldDataIn);
            model.AddMissingPropertiesInDependency(form);
            ViewBag.FieldSet = form.GetFieldSet(fieldDataIn.FieldSetId);
            ViewBag.Form = form;
            return PartialView("~/Views/Form/DragAndDrop/FieldInfoTabs.cshtml", model);
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldInfoCustomForm([ModelBinder(typeof(JsonNetModelBinder))] FieldDataIn fieldDataIn)
        {
            fieldDataIn = Ensure.IsNotNull(fieldDataIn, nameof(fieldDataIn));
            SetViewBagPreviewTypeParameters(fieldDataIn.IsReadOnlyViewMode);
            var dataOut = Mapper.Map<FieldDataOut>(fieldDataIn);
            SetNullFlavorsViewBag(fieldDataIn.FormId);

            switch (fieldDataIn.Type)
            {
                case FieldTypes.Calculative:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CalculativeFieldForm.cshtml", dataOut);
                case FieldTypes.Checkbox:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CheckboxFieldForm.cshtml", dataOut);
                case FieldTypes.CustomButton:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CustomFieldButtonForm.cshtml", dataOut);
                case FieldTypes.Date:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/DateFieldForm.cshtml", dataOut);
                case FieldTypes.Datetime:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/DatetimeFieldForm.cshtml", dataOut);
                case FieldTypes.Email:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/EmailFieldForm.cshtml", dataOut);
                case FieldTypes.File:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/FileFieldForm.cshtml", dataOut);
                case FieldTypes.LongText:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/LongTextFieldForm.cshtml", dataOut);
                case FieldTypes.Number:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/NumberFieldForm.cshtml", dataOut);
                case FieldTypes.Radio:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/RadioFieldForm.cshtml", dataOut);
                case FieldTypes.Regex:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/RegexFieldForm.cshtml", dataOut);
                case FieldTypes.Select:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/SelectFieldForm.cshtml", dataOut);
                case FieldTypes.Text:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/TextFieldForm.cshtml", dataOut);
                case FieldTypes.Coded:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CodedFieldForm.cshtml", dataOut);
                case FieldTypes.Paragraph:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/ParagraphFieldForm.cshtml", dataOut);
                case FieldTypes.Link:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/LinkFieldForm.cshtml", dataOut);
                case FieldTypes.Audio:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/AudioFieldForm.cshtml", dataOut);
                default: return PartialView("~/Views/Form/DragAndDrop/CustomFields/TextFieldForm.cshtml", dataOut);
            }
            
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public ActionResult IsNullFlavorUsedInAnyField(string formId, int nullFlavorId)
        {
            return Json(formBLL.IsNullFlavorUsedInAnyField(formId, nullFlavorId));
        }

        public ActionResult GetFieldValueInfoForm([ModelBinder(typeof(JsonNetModelBinder))] FormFieldValueDataIn fieldValueDataIn)
        {
            SetViewBagPreviewTypeParameters(fieldValueDataIn.IsReadOnlyViewMode);
            return PartialView("~/Views/Form/DragAndDrop/FieldValueInfoForm.cshtml", Mapper.Map<FormFieldValueDataOut>(fieldValueDataIn));
        }

        [HttpPost]
        public ActionResult GetCalculativeTree([ModelBinder(typeof(JsonNetModelBinder))] CalculativeTreeDataIn dataIn)
        {
            SetViewBagPreviewTypeParameters(dataIn.IsReadOnlyViewMode);
            return PartialView(dataIn.Data);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public ActionResult AddDependentOnField(DependentOnFieldInfoDataIn dataIn)
        {
            SetReadOnlyAndDisabledViewBag(false);
            return PartialView("~/Views/Form/DragAndDrop/Dependency/DependentOnFieldInfo.cshtml", Mapper.Map<DependentOnFieldInfoDataOut>(dataIn));
        }

        private void SetViewBagPreviewTypeParameters(bool readOnly)
        {
            SetReadOnlyAndDisabledViewBag(readOnly);
        }

        private void SetDiagnosisRoleAndEoCTypesViewBag()
        {
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.DiagnosisRole);
            SetEpisodeOfCareViewBags();
        }

        private void SetNullFlavorsViewBag(string formId)
        {
            List<CodeDataOut> codeDataOuts = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.NullFlavor);
            List<int> formNullFlavors = formBLL.GetFormNullFlavors(formId);
            if (formNullFlavors != null)
                codeDataOuts.RemoveAll(code => !formNullFlavors.Contains(code.Id));
            ViewBag.NullFlavors = codeDataOuts;
        }

        private void SetFormDragAndDropPartialViewBags(bool isReadOnly)
        {
            SetFormStateViewBag();
            SetViewBagPreviewTypeParameters(isReadOnly);
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
        }
    }
}