using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.EpisodeOfCare;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.DTOs.Patient.DataOut;
using sReportsV2.DTOs.Common.DataOut;

namespace sReportsV2.DTOs.Patient
{
    public class PatientDataOut
    {
        public List<IdentifierDataOut> Identifiers { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string GenderName { get; set; }
        public CodeDataOut Gender { get; set; }
        public int? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public string ContactName { get; set; }
        public string Relationship { get; set; }
        public string Language { get; set; }
        public int? CitizenshipId { get; set; }
        public int? ReligionId { get; set; }
        public int? MaritalStatusId { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public List<AddressDTO> Addresses { get; set; }
        public List<ContactDTO> Contacts { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int? LanguageCD { get; set; }
        public string CityNames
        {
            get
            {
                return string.Join(", ", Addresses.Select(a => a.City).Where(c => !string.IsNullOrEmpty(c)).Distinct());
            }
        }

        public List<EpisodeOfCareDataOut> EpisodeOfCares { get; set; }
        public List<ClinicalTrialDataOut> ClinicalTrials { get; set; }
        public List<ProjectDataOut> Projects { get; set; }

        public PatientChemotherapyDataDataOut PatientChemotherapyData { get; set; }
        public string RowVersion { get; set; }

        public EpisodeOfCareDataOut GetEpisodeOfCare(int id)
        {
            return EpisodeOfCares.Find(eoc => eoc.Id == id);
        }

        public void ReplaceEpisodeOfCare(int id, EpisodeOfCareDataOut eocReplacement)
        {
            EpisodeOfCareDataOut eocToReplace = GetEpisodeOfCare(id);
            int eocIndex = EpisodeOfCares.IndexOf(eocToReplace);
            if (eocIndex != -1)
            {
                EpisodeOfCares[eocIndex] = eocReplacement;
            }
        }

        public string GetPatientBasicInfo(string name, string familyName, DateTime? birthDate)
        {
            var date = birthDate != null ? ", " + birthDate.Value.ToString("dd-MM-yyyy") : "";

            return $"{name} {familyName}{date}";
        }

        public string ConvertEOCAndEncounterTypeCDToDisplayName(int episodeOfCareId, List<CodeDataOut> episodeOfCaresTypes, List<CodeDataOut> encounterTypes, string language, int? encounterId = null)
        {
            var eoc = EpisodeOfCares.Find(x => x.Id == episodeOfCareId);
            int eocTypeCD = eoc.Type;
            var encounter = GetEncounter(eoc, encounterId);
            string encounterTypeName = String.Empty;
            if (encounter != null) 
            {
                encounterTypeName = " - " + encounterTypes.Find(x => x.Id == encounter.TypeId)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language);
            }
            return episodeOfCaresTypes.Find(x => x.Id == eocTypeCD)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language) + encounterTypeName;
        }

        public string GetName()
        {
            return $"{Name} {FamilyName}";
        }

        private EncounterDataOut GetEncounter(EpisodeOfCareDataOut eoc, int? encounterId) 
        {
            if (encounterId == 0 || encounterId == null)
                return eoc.Encounters.OrderByDescending(x => x.EntryDatetime).FirstOrDefault();
            else
                return eoc.Encounters.Find(x => x.Id == encounterId);
        }
    }
}