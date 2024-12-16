using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Encounter.DataOut
{
    public class EncounterViewDataOut
    {
        public int EncounterId { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public int? GenderCD { get; set; }
        public DateTime? BirthDate { get; set; }
        public int PatientId { get; set; }
        public DateTimeOffset? AdmissionDate { get; set; }
        public DateTimeOffset? DischargeDate { get; set; }
        public int EpisodeOfCareId { get; set; }
        public int? EpisodeOfCareTypeCD { get; set; }
        public int? StatusCD { get; set; }
        public int? TypeCD { get; set; }

        public string ConvertGenderCDToDisplayName(List<CodeDataOut> genders, string language) 
        {
            if (this.GenderCD != null)
                return genders.Find(x => x.Id == this.GenderCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertStatusCDToDisplayName(List<CodeDataOut> statuses, string language)
        {
            if (this.StatusCD != null)
                return statuses.Find(x => x.Id == this.StatusCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertTypeCDToDisplayName(List<CodeDataOut> types, string language)
        {
            if (this.TypeCD != null)
                return types.Find(x => x.Id == this.TypeCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertEOCTypeCDToDisplayName(List<CodeDataOut> eocTypes, string language)
        {
            if (this.EpisodeOfCareTypeCD != null)
                return eocTypes.Find(x => x.Id == this.EpisodeOfCareTypeCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}
