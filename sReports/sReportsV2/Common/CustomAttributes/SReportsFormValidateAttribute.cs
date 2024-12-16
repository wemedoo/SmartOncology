using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using sReportsV2.Common.Exceptions;
using sReportsV2.Cache.Resources;
using sReportsV2.Common.Enums;
using System;
using System.Text;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class SReportsFormValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));

            FormDataIn formDataIn = filterContext.ActionArguments["formDataIn"] as FormDataIn;

            string inputValidationSummary = ValidationSummary(formDataIn);
            if (!string.IsNullOrWhiteSpace(inputValidationSummary))
            {
                FormatValidationError(inputValidationSummary);
            }

            UserCookieData userCookieData = filterContext.HttpContext.Session.GetUserFromSession();

            if (!IsFormValid(formDataIn, userCookieData))
            {
                FormatValidationError("Form with same thesaurus, language, organization and version alredy exist!");
            }

            if (!IsVersionValid(formDataIn, userCookieData)) 
            {
                FormatValidationError($"New version of document should be greater than {GetGretestVersion(formDataIn, userCookieData)}!");
            }

            List<string> listDuplicateIds = formDataIn.ValidateFieldsIds();
            if (listDuplicateIds.Count != 0)
            {
                FormatValidationError(TextLanguage.DuplicateFieldIds + ": " + string.Join(", ", listDuplicateIds));
            }
        }

        private void FormatValidationError(string errorMessage)
        {
            throw new UserAdministrationException(StatusCodes.Status409Conflict, errorMessage);
        }

        private bool IsFormValid(FormDataIn formDataIn, UserCookieData userCookieData)
        {
            FormDAL formService = new FormDAL();
            long formCount = formService.GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(formDataIn.ThesaurusId, userCookieData.ActiveOrganization, formDataIn.Language, formDataIn.Version);
            
            return formCount <= 0;
        }

        private bool IsVersionValid(FormDataIn formDataIn, UserCookieData userCookieData)
        {
            FormDAL formService = new FormDAL();
            Form formWithGreatestVersion = formService.GetFormWithGreatestVersion(formDataIn.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);

            if (formWithGreatestVersion == null || (formWithGreatestVersion.Id == formDataIn.Id && formDataIn.Version.Major == formWithGreatestVersion.Version.Major && formDataIn.Version.Minor == formWithGreatestVersion.Version.Minor)) 
            {
                return true;
            }
            return formDataIn.Version.IsVersionGreater(formWithGreatestVersion.Version);
        }

        private string GetGretestVersion(FormDataIn formDataIn, UserCookieData userCookieData)
        {
            FormDAL formService = new FormDAL();
            Form form = formService.GetFormWithGreatestVersion(formDataIn.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);

            return $"{form.Version.Major}.{form.Version.Minor}";
        }

        private string ValidationSummary(FormDataIn formDataIn)
        {
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.Append(formDataIn.ThesaurusId <= 0 ? "Form has no thesaurus!" : string.Empty);
            textBuilder.Append(ValidateChapters(formDataIn.Chapters));

            return textBuilder.ToString();
        }

        private string ValidateChapters(List<FormChapterDataIn> chapters)
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (FormChapterDataIn chapter in chapters) 
            {
                textBuilder.Append(string.IsNullOrWhiteSpace(chapter.ThesaurusId) ? $"Chapter({chapter.Title}) has no thesaurus!</br>" : string.Empty);
                textBuilder.Append(ValidatePages(chapter.Pages));
            }

            return textBuilder.ToString();
        }
        private string ValidatePages(List<FormPageDataIn> pages)
        {
            StringBuilder textBuilder = new StringBuilder();
            int i = 0;
            foreach (FormPageDataIn page in pages)
            {
                i++;
                textBuilder.Append(string.IsNullOrWhiteSpace(page.ThesaurusId) ? $"Page({page.Title}) has no thesaurus!</br>" : string.Empty);
                textBuilder.Append(ValidateFieldSets(page.ListOfFieldSets));

            }

            return textBuilder.ToString();
        }

        private string ValidateFieldSets(List<List<FormFieldSetDataIn>> listOfFs)
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (FormFieldSetDataIn fieldset in listOfFs.SelectMany(x => x).Select(y => y).ToList())
            {
                textBuilder.Append(string.IsNullOrWhiteSpace(fieldset.ThesaurusId) ? $"FieldSet({fieldset.Label}) has no thesaurus!</br>" : string.Empty);
                textBuilder.Append(ValidateFields(fieldset.Fields, fieldset.LayoutStyle?.LayoutType));

            }

            return textBuilder.ToString();
        }

        private string ValidateFields(List<FieldDataIn> fields, LayoutType? layoutType)
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (FieldDataIn field in fields) 
            {
                textBuilder.Append(string.IsNullOrWhiteSpace(field.ThesaurusId) ? $"Field ({field.Label}) has no thesaurus! </br>" : string.Empty);
                if (field is FieldSelectableDataIn fieldSelectable) 
                {
                    textBuilder.Append(ValidateFieldValues(fieldSelectable.Values, layoutType != null && layoutType == LayoutType.Matrix));
                }
            }

            return textBuilder.ToString();
        }

        private string ValidateFieldValues(List<FormFieldValueDataIn> values, bool isMatrixFieldSet)
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (FormFieldValueDataIn value in values)
            {
                textBuilder.Append(value.ThesaurusId == null && !isMatrixFieldSet ? $"Field value ({value.Label}) has no thesaurus!</br>" : string.Empty);
            }

            return textBuilder.ToString();
        }

    }
}