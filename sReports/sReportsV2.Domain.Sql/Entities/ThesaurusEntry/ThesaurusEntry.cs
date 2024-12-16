using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    [Table("ThesaurusEntries")]
    public class ThesaurusEntry : Entity, IEditChildEntries<O4CodeableConcept>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ThesaurusEntryId")]
        public int ThesaurusEntryId { get; set; }
        public virtual List<ThesaurusEntryTranslation> Translations { get; set; }
        [ForeignKey("AdmnistrativeDataId")]
        public AdministrativeData AdministrativeData { get; set; }
        [Column("AdministrativeDataId")]
        public int? AdmnistrativeDataId { get; set; }  
        public List<O4CodeableConcept> Codes { get; set; } = new List<O4CodeableConcept>();

        [ForeignKey("StateCD")]
        public virtual Code StateCode { get; set; }
        public int? StateCD { get; set; }

        #region Thesaurus Global specific properties
        public string PreferredLanguage { get; set; }
        public string UriClassLink { get; set; }
        public string UriClassGUI { get; set; }
        public string UriSourceLink { get; set; }
        public string UriSourceGUI { get; set; }
        #endregion /Thesaurus Global specific properties

        public ThesaurusEntry() { }

        public ThesaurusEntry(bool isEntityAddedByMigrations) : base(isEntityAddedByMigrations)
        {
        }

        #region Setter actions
        public void SetTranslation(string definition, string language)
        {
            ThesaurusEntryTranslation translation = GetTranslation(language);

            if (translation == null)
            {
                this.Translations.Add(new ThesaurusEntryTranslation()
                {
                    Language = language,
                    Definition = definition
                });
            }
        }
        public void UpdateTranslation(ThesaurusEntryTranslation thesaurusEntryTranslation)
        {
            SetTranslation(string.Empty, thesaurusEntryTranslation.Language);
            ThesaurusEntryTranslation translationToUpdate = this.Translations.First(tr => tr.Language.Equals(thesaurusEntryTranslation.Language));
            translationToUpdate.Copy(thesaurusEntryTranslation);
        }
        public void SetCode(O4CodeableConcept code)
        {
            this.Codes.Add(code);
        }

        public void SetPrefferedTermAndDescriptionForLang(string language, string preferredTerm, string definition)
        {
            ThesaurusEntryTranslation translation = this.Translations.Find(x => x.Language.Equals(language));
            if (translation == null)
            {
                translation = new ThesaurusEntryTranslation
                {
                    Language = language
                };
                Translations.Add(translation);
            }

            if (!string.IsNullOrWhiteSpace(definition))
            {
                translation.Definition = definition;
            }

            if (!string.IsNullOrWhiteSpace(preferredTerm))
            {
                translation.PreferredTerm = preferredTerm;
            }
        }
        #endregion /Setter actions

        #region Getter actions
        public O4CodeableConcept GetCodeByCodeSystem(int? codeSystemId)
        {
            return Codes.Find(x => !x.IsDeleted && x.CodeSystemId == codeSystemId);
        }

        public string GetDefinitionByTranslationOrDefault(string language, string activeLanguage)
        {
            string result = string.Empty;
            ThesaurusEntryTranslation translation = GetTranslation(language);
            if (translation != null)
            {
                result = translation.Definition;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = GetTranslation(LanguageConstants.EN)?.Definition;
                if (string.IsNullOrEmpty(result))
                {
                    result = GetTranslation(activeLanguage)?.Definition;
                }
            }
            return result;
        }

        public string GetPreferredTermByTranslationOrDefault(string language, string activeLanguage = null)
        {
            string result = GetPreferredTermTranslation(language);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = GetPreferredTermDefaultTranslation(activeLanguage);
            }

            return result;
        }

        public string GetPreferredTermByActiveLanguage(string language)
        {
            return GetTranslation(language)?.PreferredTerm ?? string.Empty;
        }

        private string GetPreferredTermTranslation(string language)
        {
            return GetTranslation(language)?.PreferredTerm;
        }

        private ThesaurusEntryTranslation GetTranslation(string language)
        {
            return Translations?.Find(x => x.Language.Equals(language));
        }

        private string GetPreferredTermDefaultTranslation(string activeLanguage = null)
        {
            string result = GetPreferredTermTranslation(LanguageConstants.EN);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = activeLanguage != null ? GetPreferredTermTranslation(activeLanguage) : result;
                result = string.IsNullOrWhiteSpace(result) ? GetPrefferedTermFirstTranslation() : result;
            }
            return result;
        }

        private string GetPrefferedTermFirstTranslation()
        {
            string result = string.Empty;
            var translation = Translations?.Find(x => !string.IsNullOrEmpty(x.PreferredTerm));
            if (translation != null)
            {
                result = translation.PreferredTerm;
            }

            return result;
        }
        #endregion /Getter actions

        #region Copy actions
        public void Copy(ThesaurusEntry thesaurusEntry, bool copyCodes = true)
        {
            StateCD = thesaurusEntry.StateCD;
            PreferredLanguage = thesaurusEntry.PreferredLanguage;
            CopyTranslations(thesaurusEntry.Translations);
            CopyConnectionWithOntology(thesaurusEntry);
            if (copyCodes)
            {
                CopyEntries(thesaurusEntry.Codes);
            }
        }

        public void CopyConnectionWithOntology(ThesaurusEntry fromThesaurus)
        {
            this.UriClassLink = fromThesaurus.UriClassLink;
            this.UriClassGUI = fromThesaurus.UriClassGUI;
            this.UriSourceLink = fromThesaurus.UriSourceLink;
            this.UriSourceGUI = fromThesaurus.UriSourceGUI;
        }

        private void CopyTranslations(List<ThesaurusEntryTranslation> translations)
        {
            foreach (var translation in this.Translations)
            {
                var t = translations.Find(x => x.Language == translation.Language);
                if (t != null)
                {
                    translation.PreferredTerm = t.PreferredTerm;
                    translation.Definition = t.Definition;
                    translation.Synonyms = t.Synonyms;
                    translation.Abbreviations = t.Abbreviations;
                }
            }

            foreach (var t in translations.Where(x => !Translations.Select(c => c.Language).Contains(x.Language)).ToList())
            {
                Translations.Add(t);
            }
        }

        #endregion /Copy actions

        #region Merge actions
        public void MergeTranslations(ThesaurusEntry sourceThesaurus)
        {
            this.AddMissingTranslations(sourceThesaurus);

            foreach (var sourceTranslation in sourceThesaurus.Translations)
            {
                if (!string.IsNullOrWhiteSpace(sourceTranslation.PreferredTerm))
                {
                    ThesaurusEntryTranslation targetTranslation = GetTranslation(sourceTranslation.Language);
                    targetTranslation.PreferredTerm = sourceTranslation.PreferredTerm;
                    MergeAbbreviations(targetTranslation, sourceTranslation);
                    MergeDefinition(targetTranslation, sourceTranslation);
                    MergeSynonyms(targetTranslation, sourceTranslation);
                }
            }
        }

        public void MergeSynonyms(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeSynonyms(targetTranslation, sourceTranslation);
            }
        }

        public void MergeAbbreviations(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeAbbreviations(targetTranslation, sourceTranslation);
            }
        }

        public void MergeDefinitions(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeDefinition(targetTranslation, sourceTranslation);
            }
        }

        public void MergeCodes(ThesaurusEntry fromThesaurus)
        {
            if (fromThesaurus.Codes != null)
            {
                foreach (var code in fromThesaurus.Codes)
                {
                    if (this.Codes.Find(x => x.System.CodeSystemId == code.System.CodeSystemId && (x.Value == code.Value || x.Value == null)) == null)
                    {
                        this.Codes.Add(code);
                    }
                }
            }
        }

        private void AddMissingTranslations(ThesaurusEntry sourceThesauus)
        {
            foreach (var tr in sourceThesauus.Translations)
            {
                if (this.Translations?.Find(x => x.Language == tr.Language) == null)
                {
                    this.Translations.Add(tr);
                }
            }
        }

        private void MergeDefinition(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && !string.IsNullOrEmpty(sourceTranslation.Definition))
            {
                targetTranslation.Definition = string.IsNullOrEmpty(targetTranslation.Definition) ? sourceTranslation.Definition : targetTranslation.Definition + " " + sourceTranslation.Definition;
            }
        }
        
        private void MergeSynonyms(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && sourceTranslation.Synonyms != null)
            {
                foreach (string sourceSynonym in sourceTranslation.Synonyms.Where(x => !targetTranslation.Synonyms.Contains(x, StringComparer.OrdinalIgnoreCase)))
                {
                    targetTranslation.Synonyms.Add(sourceSynonym);
                }
            }
        }

        private void MergeAbbreviations(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && sourceTranslation.Abbreviations != null)
            {
                foreach (string sourceAbbreviation in sourceTranslation.Abbreviations.Where(x => !targetTranslation.Abbreviations.Contains(x, StringComparer.OrdinalIgnoreCase)))
                {
                    targetTranslation.Abbreviations.Add(sourceAbbreviation);
                }
            }
        }
        #endregion /Merge actions

        #region Edit Child entries actions

        public void CopyEntries(List<O4CodeableConcept> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<O4CodeableConcept> upcomingEntries)
        {
            List<O4CodeableConcept> remainingCodes = new List<O4CodeableConcept>();
            foreach (var code in Codes)
            {
                var remainingCode = upcomingEntries.Exists(x => x.O4CodeableConceptId == code.O4CodeableConceptId);
                if (remainingCode)
                {
                    remainingCodes.Add(code);
                }
            }
            Codes = remainingCodes;
        }

        public void AddNewOrUpdateOldEntries(List<O4CodeableConcept> upcomingEntries)
        {
            foreach (var code in upcomingEntries)
            {
                if (code.O4CodeableConceptId == 0)
                {
                    Codes.Add(code);
                }
                else
                {
                    var dbCode = Codes.Find(x => x.O4CodeableConceptId == code.O4CodeableConceptId);
                    dbCode.Copy(code);
                }
            }
        }

        #endregion /Edit Child entries actions
    }
}
