using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using System.Linq;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareDataOut
    {
        public string Description { get; set; }
        public List<GuidelineInstanceDataOut> ListGuidelines { get; set; } = new List<GuidelineInstanceDataOut>();
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string OrganizationRef { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int? DiagnosisConditionId { get; set; }
        public PeriodDTO Period { get; set; }
        public List<DiagnosticReportDataOut> DiagnosticReports { get; set; }
        public PatientDataOut Patient { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public List<EncounterDataOut> Encounters { get; set; }
        public PersonnelTeamDataOut PersonnelTeam { get; set; }
        public ThesaurusEntryDataOut DiagnosisCondition { get; set; }
        public int NumOfDocuments { get; set; }
        public int NumOfEncounters { get; set; }
        public bool UseSkosData { get; set; }
        public string ConvertTypeCDToDisplayName(List<CodeDataOut> episodeOfCaresTypes, string language)
        {
            return episodeOfCaresTypes.Find(x => x.Id == this.Type)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language) ?? String.Empty;
        }

        public string ConvertEOCAndEncounterTypeCDToDisplayName(List<CodeDataOut> episodeOfCaresTypes, List<CodeDataOut> encounterTypes, string language, int? encounterId = null)
        {
            int eocTypeCD = this.Type;
            var encounter = GetEncounter(encounterId);
            string encounterTypeName = String.Empty;
            if (encounter != null)
            {
                encounterTypeName = " - " + encounterTypes.Find(x => x.Id == encounter.TypeId)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language);
            }
            return episodeOfCaresTypes.Find(x => x.Id == eocTypeCD)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language) + encounterTypeName;
        }

        private EncounterDataOut GetEncounter(int? encounterId)
        {
            if (encounterId == 0 || encounterId == null)
                return this.Encounters.OrderByDescending(x => x.EntryDatetime).FirstOrDefault();
            else
                return this.Encounters.Find(x => x.Id == encounterId);
        }
    }
}