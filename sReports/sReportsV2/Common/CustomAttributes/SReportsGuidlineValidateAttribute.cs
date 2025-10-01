using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using System;
using System.Collections.Generic;
using System.Text;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class SReportsGuidlineValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));

            GuidelineDataIn guidelineDataIn = filterContext.ActionArguments["dataIn"] as GuidelineDataIn;

            string inputValidationSummary = ValidationSummary(guidelineDataIn);
            if (!string.IsNullOrWhiteSpace(inputValidationSummary))
            {
                FormatValidationError(inputValidationSummary);
            }

            if (!IsVersionValid(guidelineDataIn))
            {
                FormatValidationError($"New version of digital guidline should be greater than {GetGretestVersion(guidelineDataIn)}!");
            }
        }

        private void FormatValidationError(string errorMessage)
        {
            throw new UserAdministrationException(StatusCodes.Status409Conflict, errorMessage);
        }

        private string GetGretestVersion(GuidelineDataIn guidelineDataIn)
        {
            DigitalGuidelineDAL guidlineService = new DigitalGuidelineDAL();
            Guideline form = guidlineService.GetGuidelineWithGreatestVersion(guidelineDataIn.ThesaurusId);

            return $"{form.Version.Major}.{form.Version.Minor}";
        }

        private bool IsVersionValid(GuidelineDataIn guidelineDataIn)
        {
            DigitalGuidelineDAL guidlineService = new DigitalGuidelineDAL();
            Guideline guidlineWithGreatestVersion = guidlineService.GetGuidelineWithGreatestVersion(guidelineDataIn.ThesaurusId);

            if (guidlineWithGreatestVersion == null || (guidlineWithGreatestVersion.Id == guidelineDataIn.Id && guidelineDataIn.Version.Major == guidlineWithGreatestVersion.Version.Major && guidelineDataIn.Version.Minor == guidlineWithGreatestVersion.Version.Minor))
            {
                return true;
            }
            return guidelineDataIn.Version.IsVersionGreater(guidlineWithGreatestVersion.Version);
        }

        private string ValidationSummary(GuidelineDataIn guidelineDataIn)
        {
            StringBuilder textBuilder = new StringBuilder();
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(guidelineDataIn.Title))
            {
                errors.Add("Clinical pathway has no title!");
            }

            if (guidelineDataIn.ThesaurusId <= 0)
            {
                errors.Add("Clinical pathway has no thesaurus!");
            }

            if (errors.Count > 0)
            {
                textBuilder.Append("Validation errors:</br>");
                foreach (var error in errors)
                {
                    textBuilder.Append($"- {error}</br>");
                }
            }

            return textBuilder.ToString();
        }
    }
}
