using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;

namespace sReportsV2.DTOs.DTOs.Organization.DataOut
{
    public class OrganizationCommunicationEntityDataOut
    {
        public int OrgCommunicationEntityId { get; set; }
        public string DisplayName { get; set; }
        public int? OrgCommunicationEntityCD { get; set; }
        public int? PrimaryCommunicationSystemCD { get; set; }
        public int? SecondaryCommunicationSystemCD { get; set; }
        public int? OrganizationId { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }

        public string ConvertOrgCommunicationEntityCDToDisplayName(List<CodeDataOut> types, string language)
        {
            return types.Find(x => x.Id == this.OrgCommunicationEntityCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
        }

        public string ConvertPrimaryCommunicationSystemCDToDisplayName(List<CodeDataOut> types, string language)
        {
            return types.Find(x => x.Id == this.PrimaryCommunicationSystemCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
        }

        public bool IsActive()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return this.ActiveFrom <= now && this.ActiveTo >= now;
        }
    }
}
