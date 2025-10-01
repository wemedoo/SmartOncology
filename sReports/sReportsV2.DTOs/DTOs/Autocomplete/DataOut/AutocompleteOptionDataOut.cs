using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.DTOs.Autocomplete;
using System.Collections.Generic;
using System.Text;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.DTOs.DTOs.Autocomplete.DataOut
{
    public class AutocompleteOptionDataOut : AutocompleteDataOut
    {
        public Dictionary<string, string> DataAttributes { get; set; }
        public bool FormAddition { get; set; }
        public bool PinnedItem { get; set; }

        private AutocompleteOptionDataOut(string text)
        {
            this.text = text;
        }

        public AutocompleteOptionDataOut(GuidelineEdgeElementData entityModel) : this(entityModel.Condition)
        {
            DataAttributes = new Dictionary<string, string>()
            {
                { "condition", entityModel.Condition }
            };
        }

        public AutocompleteOptionDataOut(Domain.Entities.FormInstance.FormInstance formInstance, UserCookieData userCookieData) : this($"{formInstance.Title} - {formInstance.EntryDatetime.ToTimeZoned(userCookieData.TimeZoneOffset, DateTimeConstants.DateFormat)}")
        {
            DataAttributes = new Dictionary<string, string>()
            {
                { "forminstanceid", formInstance.Id },
            };
        }

        public AutocompleteOptionDataOut(Guideline guideline, UserCookieData userCookieData) : this($"{guideline.Title} - {guideline.EntryDatetime.ToTimeZoned(userCookieData.TimeZoneOffset, DateTimeConstants.DateFormat)}")
        {
            DataAttributes = new Dictionary<string, string>()
            {
                { "guidelineid", guideline.Id },
                { "guidelinetitle", guideline.Title },
            };
        }

        public AutocompleteOptionDataOut(Domain.Entities.Form.Form form, UserCookieData userCookieData) : this($"{form.GetTitleWithVersion()} - {form.EntryDatetime.ToTimeZoned(userCookieData.TimeZoneOffset, DateTimeConstants.DateFormat)}")
        {
            FormAddition = true;
            PinnedItem = userCookieData.SuggestedForms.Contains(form.Id);
            DataAttributes = new Dictionary<string, string>()
            {
                { "id", form.Id }
            };
        }

        public string RenderDataAttributes()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> attribute in DataAttributes) {
                stringBuilder.AppendLine($@"data-{attribute.Key.ToLower()}=""{attribute.Value}"" ");
            }
            return stringBuilder.ToString();
        }
    }
}
