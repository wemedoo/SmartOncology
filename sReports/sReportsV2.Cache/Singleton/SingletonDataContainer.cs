using AutoMapper;
using ExcelImporter.Importers;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Localization;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.CodeAliases.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Cache.Singleton
{
    public class SingletonDataContainer
    {
        private static SingletonDataContainer instance;

        #region Changing data in runtime
        private List<CodeDataOut> codes = new List<CodeDataOut>();
        private List<CodeAliasViewDataOut> aliases = new List<CodeAliasViewDataOut>();
        #endregion /Changing data in runtime

        #region Pre-defined data
        private readonly List<EnumDTO> languages = new List<EnumDTO>();
        private List<SmartOncologyEnumDataOut> smartOncologyEnums = new List<SmartOncologyEnumDataOut>();
        private List<CodeSystemDataOut> codeSystems = new List<CodeSystemDataOut>();
        private readonly List<EnumDTO> patientLanguages = new List<EnumDTO>();
        #endregion /Pre-defined data

        #region Singleton methods
        private SingletonDataContainer(IServiceProvider serviceProvider)
        {
            PopulateData(serviceProvider);
        }

        public static SingletonDataContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("SingletonDataContainer not initialized.");
                }
                return instance;
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (instance == null)
            {
                instance = new SingletonDataContainer(serviceProvider);
            }
        }

        public void RefreshSingleton(IMapper mapper, ICodeAliasViewDAL codeAliasViewDAL, ICodeDAL codeDAL, int? resourceId, ModifiedResourceType? modifiedResourceType)
        {
            if (resourceId.HasValue && modifiedResourceType.HasValue)
            {
                RefreshSingletonPartially(mapper, codeAliasViewDAL, codeDAL, resourceId, modifiedResourceType);
            }
            else
            {
                PopulateCodes(mapper, codeDAL);
                PopulateAliases(mapper, codeAliasViewDAL);
            }
        }

        private void RefreshSingletonPartially(IMapper mapper, ICodeAliasViewDAL codeAliasViewDAL, ICodeDAL codeDAL, int? resourceId, ModifiedResourceType? modifiedResourceType)
        {
            switch (modifiedResourceType.Value)
            {
                case ModifiedResourceType.Thesaurus:
                    List<int> affectedCachedCodesIds = codes.Where(c => c.Thesaurus.Id == resourceId.Value).Select(x => x.Id).ToList();
                    codes.RemoveAll(x => affectedCachedCodesIds.Contains(x.Id));
                    List<CodeDataOut> affectedCodesFromDb = mapper.Map<List<CodeDataOut>>(codeDAL.GetByIds(affectedCachedCodesIds));
                    codes.AddRange(affectedCodesFromDb);
                    break;
                case ModifiedResourceType.Code:
                    CodeDataOut cachedCode = codes.Find(c => c.Id == resourceId.Value);
                    if (cachedCode != null)
                    {
                        codes.Remove(cachedCode);
                    }
                    CodeDataOut codeFromDb = mapper.Map<CodeDataOut>(codeDAL.GetById(resourceId.Value));
                    codes.Add(codeFromDb);
                    break;
                case ModifiedResourceType.Alias:
                    CodeAliasViewDataOut cachedAlias = aliases.Find(a => a.AliasId == resourceId.Value);
                    if (cachedAlias != null)
                    {
                        aliases.Remove(cachedAlias);
                    }
                    CodeAliasViewDataOut aliasFromDb = mapper.Map<CodeAliasViewDataOut>(codeAliasViewDAL.GetById(resourceId.Value));
                    aliases.Add(aliasFromDb);
                    break;
                default:
                    break;
            }
        }

        #endregion /Singleton methods

        #region Getters of Non-Code entities
        public List<EnumDTO> GetLanguages()
        {
            return this.languages;
        }

        public List<EnumDTO> GetPatientLanguages()
        {
            return this.patientLanguages;
        }

        public List<CodeSystemDataOut> GetCodeSystems()
        {
            return this.codeSystems;
        }

        #endregion /Getters of Non-Code entities

        #region Getters of Code entities

        public List<CodeDataOut> GetCodes()
        {
            return this.codes;
        }

        public CodeDataOut GetCode(int codeId, bool includeActiveCondition = false)
        {
            if (includeActiveCondition)
            {
                return GetCodes().Find(x => x.Id == codeId && x.IsActive());
            }
            else
            {
                return GetCodes().Find(x => x.Id == codeId);
            }
        }

        public string GetCodePreferredTerm(int codeId)
        {
            string preferredTerm = string.Empty;
            CodeDataOut code = GetCode(codeId);
            if (code != null)
            {
                preferredTerm = code.Thesaurus?.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
            }
            return preferredTerm;
        }

        public int GetCodeIdForPreferredTerm(string preferredTerm, int codesetId)
        {
            return GetCodesByCodeSetId(codesetId)
                .Where(e => e.Thesaurus?.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN) == preferredTerm)
                .Select(en => en.Id)
                .FirstOrDefault();
        }

        public List<CodeDataOut> GetCodesByCodeSetId(int codeSetId)
        {
            return GetCodes()
                .Where(x =>
                    x.CodeSetId == codeSetId
                ).ToList();
        }

        public int? GetCodeId(int codeSetId, string preferredTerm, string language = LanguageConstants.EN)
        {
            return GetCode(codeSetId, preferredTerm, language)?.Id;
        }

        public int? GetCodeIdByAlias(int codeSetId, string alias)
        {
            return aliases
                .Find(al => al.InboundAlias == alias && al.CodeSetId == codeSetId)?.CodeId;
        }

        public IEnumerable<string> GetInboundAliasesForCode(int codeSetId, string preferredTerm)
        {
            int? codeId = GetCodeId(codeSetId, preferredTerm);
            return aliases
                .Where(al => al.CodeId == codeId)
                .Select(inboundAlias => inboundAlias.InboundAlias);
        }

        public IEnumerable<string> GetInboundAliasesForCodeSet(int codeSetId)
        {
            return aliases
                .Where(al => al.CodeSetId == codeSetId)
                .Select(inboundAlias => inboundAlias.InboundAlias);
        }

        public string GetOutboundAlias(int? codeId, string system)
        {
            return aliases
                .Find(al => al.CodeId == codeId.GetValueOrDefault() && al.System == system)
                ?.OutboundAlias;
        }


        private CodeDataOut GetCode(int codeSetId, string preferredTerm, string language = LanguageConstants.EN)
        {
            return GetCodesByCodeSetId(codeSetId)
                .Find(x => x.Thesaurus?.GetPreferredTermByTranslationOrDefault(language) == preferredTerm);
        }

        #endregion /Getters of Code entities

        #region Digital Guideline methods

        public List<CodeDataOut> GetNCCNCategoriesOfEvidenceAndConsensus()
        {
            return new List<CodeDataOut>()
            {
                MockEnumDataOut("Category 1", "Category 1", 50000),
                MockEnumDataOut("Category 2A", "Category 2A", 50001),
                MockEnumDataOut("Category 2B", "Category 2B", 50002),
                MockEnumDataOut("Category 3", "Category 3", 50003)
            };
        }

        public List<CodeDataOut> GetNCCNCategoriesOfPreference()
        {
            return new List<CodeDataOut>()
            {
                MockEnumDataOut("Preferred Intervention", "Preferred Intervention", 50004),
                MockEnumDataOut("Other recommend intervention", "Other recommend intervention", 50005),
                MockEnumDataOut("Useful in certain circumstances", "Useful in certain circumstances", 50006)
            };
        }

        public List<CodeDataOut> GetOxfordLevelOfEvidenceSystem()
        {
            return new List<CodeDataOut>()
            {
                MockEnumDataOut("1a", "Preferred Intervention", 50007),
                MockEnumDataOut("1b", "Other recommend intervention", 50008),
                MockEnumDataOut("1c", "All or none randomized controlled trials", 50009),
                MockEnumDataOut("2a", "All or none randomized controlled trials", 500010),
                MockEnumDataOut("2b", "Individual cohort study or low quality randomized controlled trials (e.g. < 80% follow up)", 500011),
                MockEnumDataOut("2c", "Outcomes Research: ecological studies", 500012),
                MockEnumDataOut("3a", "Systematic review (with homogenity) of case-control studies", 500013),
                MockEnumDataOut("4", "Case series(and poor quality cohort and case-control studies)", 500014),
                MockEnumDataOut("5", "Expert opinion without explicit critical appraisal, or based on physiology, bench reasearch or first principles", 500015),
                MockEnumDataOut("3b", "Individual case controll study", 500016)
            };
        }

        public List<CodeDataOut> GetStrengthOfRecommendation()
        {
            return new List<CodeDataOut>()
            {
                MockEnumDataOut("HIGH","There is a lot of confidence that the true effect lies close to that of the estimated effect",50016),
                MockEnumDataOut("MODERATE","There is moderate confidence in the estimated effect: The true effect is likely to be close to the estimated effect" +
                ", but there is apossiblity that it is substantially different",50017),
                MockEnumDataOut("LOW","There is limited effect in the estimated effect: The true effect might be substantially different from the estimated effect",50018),
                MockEnumDataOut("VERY LOW","There is very little confidence in the estimated effect: The true effect islikely to be substantially different from the estimated effect",50018),
            };
        }

        private CodeDataOut MockEnumDataOut(string label, string definition, int id)
        {
            return new CodeDataOut()
            {
                Id = 10,
                Thesaurus = new ThesaurusEntryDataOut()
                {
                    Id = id,
                    Translations = new List<ThesaurusEntryTranslationDTO>()
                    {
                       new ThesaurusEntryTranslationDTO()
                       {
                           Language = LanguageConstants.EN,
                           PreferredTerm = label,
                           Definition = definition
                       }
                    }
                }
            };
        }


        #endregion /Digital Guideline methods

        #region Initial setters
        private void PopulateData(IServiceProvider serviceProvider)
        {
            PopulateDataFromOtherSources();
            PopulateDataFromDatabase(serviceProvider);
        }

        private void PopulateDataFromDatabase(IServiceProvider serviceProvider)
        {
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var codeSystemDAL = serviceProvider.GetRequiredService<ICodeSystemDAL>();
            var codeAliasViewDAL = serviceProvider.GetRequiredService<ICodeAliasViewDAL>();
            var codeDAL = serviceProvider.GetRequiredService<ICodeDAL>();

            PopulateCodeSystems(mapper, codeSystemDAL);
            PopulateCodes(mapper, codeDAL);
            PopulateAliases(mapper, codeAliasViewDAL);
        }

        private void PopulateDataFromOtherSources()
        {
            PopulateLanguages();
            PopulatePatientLanguages();
            PopulateSmartOncologyEnums();
        }

        private void PopulateLanguages()
        {
            languages.Add(new EnumDTO() { Label = "English", Value = LanguageConstants.EN });
            languages.Add(new EnumDTO() { Label = "German", Value = LanguageConstants.DE });
        }

        private void PopulatePatientLanguages()
        {
            foreach (SpokenLang lang in Enum.GetValues(typeof(SpokenLang)))
            {
                patientLanguages.Add(new EnumDTO() { Label = lang.ToString(), Value = lang.GetLangAttribute().Iso6391 });
            }
        }

        private void PopulateCodeSystems(IMapper mapper, ICodeSystemDAL codeSystemDAL)
        {
            this.codeSystems = mapper.Map<List<CodeSystemDataOut>>(codeSystemDAL.GetAll().OrderBy(x => x.Label));
        }

        private void PopulateCodes(IMapper mapper, ICodeDAL codeDAL)
        {
            List<Code> enums = codeDAL.GetAll().ToList();
            this.codes = mapper.Map<List<CodeDataOut>>(enums);
        }

        private void PopulateAliases(IMapper mapper, ICodeAliasViewDAL codeAliasViewDAL)
        {
            List<CodeAliasView> codeAliasViews = codeAliasViewDAL.GetAllAvailableAliases();
            this.aliases = mapper.Map<List<CodeAliasViewDataOut>>(codeAliasViews);
        }

        #endregion /Initial setters

        #region Smart Oncology -> Chemotherapy Schema enums
        public List<SmartOncologyEnumDataOut> GetSmartOncologyEnums(string type)
        {
            return this.smartOncologyEnums.Where(e => e.Type == type).ToList();
        }

        private void PopulateSmartOncologyEnums()
        {
            var presentationStage = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.PresentationStage);
            var anatomy = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.Anatomy);
            var morphology = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.Morphology);
            var therapeuticContext = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.TherapyCategorizationSheet, SmartOncologyEnumNames.TherapeuticContext);
            var chemotherapyType = GetEnumsFromExcel(SmartOncologyEnumNames.ChemotherapyCompendiumFileName, SmartOncologyEnumNames.ChemotherapySchemasSheet, SmartOncologyEnumNames.ChemotherapyType);

            List<SmartOncologyEnumDataOut> newEnums = new List<SmartOncologyEnumDataOut>();
            newEnums.AddRange(presentationStage);
            newEnums.AddRange(anatomy);
            newEnums.AddRange(morphology);
            newEnums.AddRange(therapeuticContext);
            newEnums.AddRange(chemotherapyType);

            this.smartOncologyEnums = newEnums;

        }

        private List<SmartOncologyEnumDataOut> GetEnumsFromExcel(string fileName, string sheetName, string columnName)
        {
            List<SmartOncologyEnumDataOut> enums = new SchemaColumnImporter(fileName, sheetName, columnName).GetEnumsFromExcel();

            return enums;
        }
        #endregion /Smart Oncology -> Chemotherapy Schema enums
    }

    public enum ModifiedResourceType
    {
        Thesaurus,
        Code,
        Alias
    }
}
