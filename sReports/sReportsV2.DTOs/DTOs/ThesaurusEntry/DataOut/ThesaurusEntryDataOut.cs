using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.CodeEntry.DataOut;

namespace sReportsV2.DTOs.ThesaurusEntry.DataOut
{
    public class ThesaurusEntryDataOut
    {
        public int Id { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public string UmlsDefinitions { get; set; }
        public AdministrativeDataDataOut AdministrativeData { get; set; }
        public List<ThesaurusEntryTranslationDTO> Translations { get; set; }
        public List<ThesaurusEntryCodingSystemDTO> CodingSystems { get; set; }
        public List<O4CodeableConceptDataOut> Codes { get; set; }
        public int? StateCD { get; set; }
        public string PreferredLanguage { get; set; }

        public string UriClassLink { get; set; }
        public string UriClassGUI { get; set; }
        public string UriSourceLink { get; set; }
        public string UriSourceGUI { get; set; }

        public ThesaurusEntryTranslationDTO GetTranslation(string language)
        {
            return GetTranslationByLanguage(language) ?? GetTranslationByLanguage(LanguageConstants.EN) ?? Translations.Find(x => x.Language != null) ?? new ThesaurusEntryTranslationDTO() { Abbreviations = new List<string>(), Synonyms = new List<string>()};
        }

        public string GetAbbreaviations(string language)
        {
            return JoinList(GetTranslationByLanguage(language)?.Abbreviations);
        }

        public string GetSynonyms(string language)
        {
            return JoinList(GetTranslationByLanguage(language)?.Synonyms);
        }

        public List<string> GetSynonymsListByLanguage(string language)
        {
            return GetTranslationByLanguage(language)?.Synonyms;
        }

        public List<string> GetAbbreviationListByLanguage(string language)
        {
            return GetTranslationByLanguage(language)?.Abbreviations;
        }

        public string GetDefinitionByTranslationOrDefault(string language)
        {
            string result = string.Empty;
            if (GetTranslationByLanguage(language) != null)
            {
                result = GetTranslationByLanguage(language).Definition;
            }

            if (string.IsNullOrEmpty(result))
            {
                if (GetTranslationByLanguage(LanguageConstants.EN) != null)
                    result = GetTranslationByLanguage(LanguageConstants.EN).Definition;
                else
                    result = Translations.Find(x => x.Language != null).Definition;
            }

            return result;
        }

        public string GetPreferredTermByTranslationOrDefault(string language)
        {
            string result = string.Empty;
            if (GetTranslationByLanguage(language) != null)
            {
                result = GetTranslationByLanguage(language).PreferredTerm;
            }

            if (string.IsNullOrEmpty(result))
            {
                if (GetTranslationByLanguage(LanguageConstants.EN) != null)
                    result = GetTranslationByLanguage(LanguageConstants.EN).PreferredTerm;
                else
                    result = Translations?.Find(x => x.Language != null).PreferredTerm;
            }

            return result;
        }

        public string GetFilteredTerm(GlobalThesaurusFilterDataIn filterData)
        {
            string result = string.Empty;
            string language = filterData?.Language ?? (filterData?.Term == null ? this.PreferredLanguage : null);
            if (language != null)
            {
                result = GetPreferredTermByTranslationOrDefault(language);
            }
            else
            {
                if (filterData != null && !string.IsNullOrWhiteSpace(filterData.Term))
                {
                    if (!string.IsNullOrWhiteSpace(filterData.TermIndicator) && filterData.TermIndicator.Equals("exact"))
                    {
                        result = Translations.Find(t => !string.IsNullOrEmpty(t.PreferredTerm) && t.PreferredTerm.ToLower().Equals(filterData.Term.ToLower()))?.PreferredTerm;
                    }
                    else
                    {
                        result = Translations.Find(t => !string.IsNullOrEmpty(t.PreferredTerm) && t.PreferredTerm.ToLower().Contains(filterData.Term.ToLower()))?.PreferredTerm;
                    }
                }
                else
                {
                    result = Translations.Find(x => !string.IsNullOrWhiteSpace(x.PreferredTerm))?.PreferredTerm;
                }
            }

            return result ?? "No preferred term";
        }

        public string GetUserWhoChangedThesaurus(string timeOfChange)
        {
            string user = string.Empty;

            if(AdministrativeData != null && AdministrativeData.VersionHistory != null)
            {
                VersionDataOut change = null;
                if(timeOfChange == "Last")
                {
                    change = AdministrativeData.VersionHistory.LastOrDefault();
                }
                else if (timeOfChange == "First")
                {
                    change = AdministrativeData.VersionHistory.FirstOrDefault();

                }

                if (change != null && change.User != null)
                {
                    user = change.User.Name;
                }
            }

            return user;
        }

        private string JoinList(List<string> list)
        {
            string result = "";
            if (list != null)
            {
                result = string.Join(",", list);
            }
            return result;
        }

        private ThesaurusEntryTranslationDTO GetTranslationByLanguage(string language)
        {
            return this.Translations?.Find(x => x.Language.Equals(language));
        }

        public string ConvertStateCDToDisplayName(List<CodeDataOut> states, string language)
        {
            if (this.StateCD != null && this.StateCD.HasValue)
                return states.Find(x => x.Id == this.StateCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}