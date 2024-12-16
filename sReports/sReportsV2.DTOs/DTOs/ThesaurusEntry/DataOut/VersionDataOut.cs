using sReportsV2.Common.Constants;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.ThesaurusEntry.DataOut
{
    public class VersionDataOut
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? RevokedOn { get; set; }
        public UserShortInfoDataOut User {get;set;}
        public int? TypeCD { get; set; }
        public OrganizationDataOut Organization { get; set; }
        public int? StateCD { get; set; }

        public string GetStateColor(List<CodeDataOut> states, string language)
        {
            var thesaurusState = ConvertStateCDToDisplayName(states, language);
            string color;

            if (thesaurusState == CodeAttributeNames.Production)
                color = "production-state";
            else if (thesaurusState == CodeAttributeNames.Deprecated)
                color = "depracated-state";
            else if (thesaurusState == CodeAttributeNames.Disabled)
                color = "disabled-state";
            else
                color = "administrative-state";

            return color;
        }

        public string ConvertTypeCDToDisplayName(List<CodeDataOut> types, string language)
        {
            if (this.TypeCD != null && this.TypeCD.HasValue)
                return types.Find(x => x.Id == this.TypeCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertStateCDToDisplayName(List<CodeDataOut> states, string language)
        {
            if (this.StateCD != null && this.StateCD.HasValue)
                return states.Find(x => x.Id == this.StateCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}