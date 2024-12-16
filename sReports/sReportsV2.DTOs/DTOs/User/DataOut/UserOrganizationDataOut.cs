using sReportsV2.Common.Enums;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Organization;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.User.DataOut
{
    public class UserOrganizationDataOut
    {
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public int OrganizationId { get; set; }
        public int? StateCD { get; set; }

        public OrganizationDataOut Organization { get; set; }

        public UserOrganizationDataOut() { }

        public UserOrganizationDataOut(OrganizationDataOut organization)
        {
            this.Organization = organization;
            this.OrganizationId = organization.Id;
        }

        public string ConvertStateCDToDisplayName(List<CodeDataOut> states, string language)
        {
            if (this.StateCD != null && this.StateCD.HasValue)
                return states.Find(x => x.Id == this.StateCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}