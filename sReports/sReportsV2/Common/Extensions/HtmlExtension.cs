using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sReportsV2.Cache.Resources;
using sReportsV2.DTOs.Common;
using System;
using System.Linq;
using System.Text;

namespace sReportsV2.Common.Extensions
{
    public static class HtmlExtension
    {
        public static IHtmlContent ReadOnly(this IHtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "readonly" : "");
        }

        public static IHtmlContent Disabled(this IHtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "disabled" : "");
        }

        public static IHtmlContent DateTimeDisabled(this IHtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"openDateTimeDatePicker(event)\"");
        }

        public static IHtmlContent DateDisabled(this IHtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"showDatePicker(event)\"");
        }

        public static IHtmlContent TimeDisabled(this IHtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"openDateTimeTimePicker(event)\"");
        }

        public static IHtmlContent RenderTableCell(this IHtmlHelper helper, string value)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(string.IsNullOrEmpty(value) ? TextLanguage.N_E : value);
        }

        public static string GetVisibilityClass(this bool conditionIsSatisfied)
        {
            return conditionIsSatisfied ? "visible" : "invisible";
        }

        public static bool AnyConditionIsMet(params bool[] arguments)
        {
            return arguments.ToList().Exists(arg => arg);
        }

        public static string GetSortArrowClass(string columnName, string sortingColumnName, bool isAscending)
        {
            return columnName == sortingColumnName ? (isAscending ? "sort-arrow-asc" : "sort-arrow-desc") : "sort-arrow";
        }

        public static IHtmlContent HtmlContainerId(this IHtmlHelper helper, string elementId, bool isContainer)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(elementId + (isContainer ? "-acc" : "-li"));
        }

        public static string OopusDownloadLink(this IUrlHelper urlHelper, string url, string domain)
        {
            urlHelper = Ensure.IsNotNull(urlHelper, nameof(urlHelper));
            return string.IsNullOrEmpty(url) ? string.Empty : urlHelper.Action("Download", "Blob", new BinaryMetadataDataIn { ResourceId = url.GetResourceNameFromUri(), Domain = domain});
        }

        public static IHtmlContent ChapterInstanceHeaderAttributes(this IHtmlHelper helper, bool allowChapterCollapse, bool chapterIsActive, string elementId, string targetId)
        {
            string dataToggle = allowChapterCollapse ? "collapse" : string.Empty;
            string onClick = allowChapterCollapse ? "changeChapterAction(event, this, false)" : "return false;";
            string ariaExpanded = chapterIsActive ? "true" : "false";
            string allowCollapseClass = allowChapterCollapse ? "chapter-hover" : "chapter-non-collapse";
            string chapterActiveClass = chapterIsActive ? "chapter-active" : "";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($@"class=""position-relative enc-form-instance-header display-flex {allowCollapseClass} {chapterActiveClass}""");
            stringBuilder.AppendLine($@"data-link-id=""#{helper.HtmlContainerId(elementId, isContainer: false)}""");
            stringBuilder.AppendLine($@"data-toggle=""{dataToggle}""");
            stringBuilder.AppendLine($@"onclick=""{onClick}""");
            stringBuilder.AppendLine($@"aria-expanded=""{ariaExpanded}""");
            stringBuilder.AppendLine($@"data-target=""#{targetId}""");
            stringBuilder.AppendLine($@"aria-controls=""{targetId}""");
            return helper.Raw(stringBuilder);
        }
    }
}
