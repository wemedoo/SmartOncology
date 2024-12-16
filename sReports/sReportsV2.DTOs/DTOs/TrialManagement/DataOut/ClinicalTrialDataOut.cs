using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.TrialManagement
{
    public class ClinicalTrialDataOut
    {
        public int ClinicalTrialId { get; set; }
        public string ClinicalTrialTitle { get; set; }
        public string ClinicalTrialAcronym { get; set; }
        public string ClinicalTrialSponsorIdentifier { get; set; }
        public string ClinicalTrialDataProviderIdentifier { get; set; }
        public bool? IsArchived { get; set; }
        public int? ClinicalTrialRecruitmentStatusCD { get; set; }
        public List<UserDataOut> Personnels { get; set; } = new List<UserDataOut>();

        public string ClinicalTrialIdentifier { get; set; }
        public string ClinicalTrialSponsorName { get; set; }
        public string ClinicalTrialDataManagementProvider { get; set; }

        public int? ClinicalTrialIdentifierTypeCD { get; set; }
        public int? ClinicalTrialSponsorIdentifierTypeCD { get; set; }

        public string GetClinicalTrialRecruitmentStatus(string activeLanguage, List<CodeDataOut> clinicalTrialRecruitmentStatuses)
        {
            CodeDataOut recruitmentStatusCode = clinicalTrialRecruitmentStatuses.Find(x => x.Id == ClinicalTrialRecruitmentStatusCD);
            return recruitmentStatusCode != null ? recruitmentStatusCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage) : string.Empty;
        }
    }
}
