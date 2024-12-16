using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.UMLS.Classes
{
    public class SqlImporter
    {
        private int insertedThousands;
        private readonly int bulkInsertStep;

        private readonly Dictionary<RankKey, string> rankDictionary;
        private readonly Dictionary<string, List<string>> defDictionary;
        private readonly Dictionary<string, string> languages;
        private List<ThesaurusEntry> thesauruses;

        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IThesaurusTranslationDAL translationDAL;
        private readonly IO4CodeableConceptDAL o4codeableConceptDAL;
        private readonly ICodeDAL codeDAL;
        private readonly ICodeSystemDAL codeSystemDAL;
        private readonly IAdministrativeDataDAL administrativeDataDAL;
        private readonly IConfiguration configuration;

        public SqlImporter(IThesaurusDAL thesaurusDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, ICodeDAL codeDAL, ICodeSystemDAL codeSystemDAL, IAdministrativeDataDAL administrativeDataDAL, IConfiguration configuration)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.codeDAL = codeDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.administrativeDataDAL = administrativeDataDAL;
            this.configuration = configuration;

            this.insertedThousands = 0;
            this.bulkInsertStep = 50000;
            this.thesauruses = new List<ThesaurusEntry>();
            this.rankDictionary = new Dictionary<RankKey, string>();
            this.defDictionary = new Dictionary<string, List<string>>();
            this.languages = new Dictionary<string, string>()
            {
                {"ENG", LanguageConstants.EN },
                {"FRE", LanguageConstants.FR },
                {"GER", LanguageConstants.DE },
                {"ITA", LanguageConstants.IT },
                {"SPA", LanguageConstants.ES },
                {"POR", LanguageConstants.PT }
            };
        }

        public async Task Import()
        {
            if (thesaurusDAL.GetAllCount() == 0)
            {
                await LoadMRRANKIntoMemory("MRRANK.RRF");
                await LoadMRDEFIntoMemory("MRDEF.RRF");
                await LoadMRCONSOIntoMemory("MRCONSO.RRF");
            }

        }

        public async Task ImportCodingSystems()
        {
            if (codeSystemDAL.GetAllCount() == 0)
            {
                SetCustomCodeSystems();
                await LoadMRSABIntoMemory("MRSAB.RRF");
            }
        }

        #region MRCONSO
        private async Task LoadMRCONSOIntoMemory(string fileName)
        {

            string currentConcept = string.Empty;
            int currentHigherRank = 0;
            int? draftStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusState, CodeAttributeNames.Draft);

            ThesaurusEntry thesaurus = new ThesaurusEntry()
            {
                StateCD = draftStateCD,
                Translations = new List<ThesaurusEntryTranslation>()
            };
            try
            {
                List<CodeSystem> codeSystems = codeSystemDAL.GetAll();
                BlobClient cloudBlockBlob = await GetOrCreateBlob(fileName, StorageDirectoryNames.UMLS);
                using (var reader = new StreamReader(await cloudBlockBlob.OpenReadAsync()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (currentConcept != parts[0])
                        {
                            if (!string.IsNullOrWhiteSpace(currentConcept))
                            {
                                thesauruses.Add(thesaurus);
                            }
                            InsertIfBatchIsFull();
                            currentConcept = parts[0];
                            currentHigherRank = 0;
                            var codeUmls = codeSystems.Find(x => x.SAB == "MTH");
                            thesaurus.SetCode(new O4CodeableConcept()
                            {
                                CodeSystemId = codeUmls.CodeSystemId,
                                Code = parts[0],
                                Value = parts[14],
                                VersionPublishDate = DateTime.Now,
                                EntryDateTime = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(),
                            });
                        }
                        if (languages.ContainsKey(parts[1]))
                        {
                            string definition = GetDefinition(parts[7]);
                            thesaurus.SetTranslation(definition, languages[parts[1]]);

                            SetDefinitionOfConcept(parts[11], parts[12], currentHigherRank, definition, parts[1], thesaurus);
                        }


                        var code = codeSystems.Find(x => parts[11].Contains(x.SAB));
                        if (code != null)
                        {
                            thesaurus.SetCode(new O4CodeableConcept()
                            {
                                Code = parts[13],
                                Value = parts[14],
                                VersionPublishDate = DateTime.Now,
                                CodeSystemId = code.CodeSystemId,
                                EntryDateTime = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(),
                            });
                        }

                    }

                    InsertThesauruses(thesauruses);
                }
            }
            catch (Exception e)
            {
                HandleException(e, fileName);
            }
        }

        private void SetDefinitionOfConcept(string sab, string tty, int currentHigherRank, string definition, string language, ThesaurusEntry thesaurus)
        {
            RankKey rankKey = new RankKey()
            {
                SAB = sab,
                TTY = tty
            };

            if (rankDictionary.ContainsKey(rankKey))
            {
                int rankOfNewAtom = Int32.Parse(rankDictionary[rankKey]);

                if (!string.IsNullOrWhiteSpace(definition) && rankOfNewAtom > currentHigherRank)
                {
                    thesaurus.Translations.Find(x => x.Language == languages[language]).Definition = definition;
                }
            }
        }

        private string GetDefinition(string atomIdentifier)
        {
            return defDictionary.ContainsKey(atomIdentifier) ? string.Join(Environment.NewLine, defDictionary[atomIdentifier]) : string.Empty;
        }

        private void InsertIfBatchIsFull()
        {
            if (thesauruses.Count == bulkInsertStep)
            {
                LogHelper.Info($"{++insertedThousands}");
                InsertThesauruses(thesauruses);
                thesauruses = new List<ThesaurusEntry>();
            }
        }

        private void InsertThesauruses(List<ThesaurusEntry> thesauruses)
        {
            thesaurusDAL.InsertMany(thesauruses);
            var bulkedThesauruses = thesaurusDAL.GetLastBulkInserted(thesauruses.Count);
            translationDAL.InsertMany(thesauruses, bulkedThesauruses, configuration);
            o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses, configuration);
            administrativeDataDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertManyVersions(thesauruses, bulkedThesauruses);
        }
        #endregion /MRCONSO

        private async Task LoadMRDEFIntoMemory(string fileName)
        {
            try
            {
                BlobClient cloudBlockBlob = await GetOrCreateBlob(fileName, StorageDirectoryNames.UMLS);
                using (var reader = new StreamReader(await cloudBlockBlob.OpenReadAsync()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (!defDictionary.ContainsKey(parts[1]))
                            {
                                defDictionary.Add(parts[1], new List<string>() { parts[5] });
                            }
                            else
                            {
                                defDictionary[parts[1]].Add(parts[5]);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, fileName);
            }
        }

        private async Task LoadMRRANKIntoMemory(string fileName)
        {
            try
            {
                BlobClient cloudBlockBlob = await GetOrCreateBlob(fileName, StorageDirectoryNames.UMLS);
                using (var reader = new StreamReader(await cloudBlockBlob.OpenReadAsync()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        RankKey key = new RankKey() { SAB = parts[1], TTY = parts[2] };
                        if (rankDictionary.Any(x => x.Key.Equals(key)))
                        {
                            rankDictionary.Add(key, parts[0]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, fileName);
            }
        }

        private async Task LoadMRSABIntoMemory(string fileName)
        {
            List<CodeSystem> codingSystems = new List<CodeSystem>();
            try
            {
                BlobClient cloudBlockBlob = await GetOrCreateBlob(fileName, StorageDirectoryNames.UMLS);
                using (var reader = new StreamReader(await cloudBlockBlob.OpenReadAsync()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');

                        if (!string.IsNullOrEmpty(parts[11]) && !string.IsNullOrWhiteSpace(parts[4]) && !codingSystems.Exists(x => x.SAB == parts[3]))
                        {
                            codingSystems.Add(new CodeSystem()
                            {
                                Value = parts[11].Split(';')[11],
                                Label = parts[4],
                                SAB = parts[3]
                            });
                        }
                    }
                }

                codeSystemDAL.InsertMany(codingSystems);
            }
            catch (Exception e)
            {
                HandleException(e, fileName);
            }
        }

        #region Common
        private async Task<BlobClient> GetOrCreateBlob(string resourceId, string domain)
        {
            bool isAzureInstance = Convert.ToBoolean(configuration["AzureInstance"]);
            if (!isAzureInstance) throw new ArgumentException("Not an Azure Instance");
            return await CloudStorageHelper.GetOrCreateBlob(resourceId, domain, configuration["AccountStorage"]);
        }

        private void HandleException(Exception ex, string fileName)
        {
            LogHelper.Error($"The file ({fileName}) could not be read, exception message: {ex.Message}");
            LogHelper.Error(ex.StackTrace);
        }

        private void SetCustomCodeSystems()
        {
            List<CodeSystem> codingSystems = new List<CodeSystem>
            {
                new CodeSystem
                {
                    Value = "Oomnia-External-ID",
                    Label = ResourceTypes.OomniaExternalId,
                    SAB = "OOMNIA"
                },
                new CodeSystem()
                {
                    Value = ResourceTypes.CountryCodingSystem,
                    Label = "ISO 3166 Codes for the representation of names of countries and their subdivisions"
                }
            };
            List<CodeSystem> codingSystemsToAdd = new List<CodeSystem>();
            foreach (CodeSystem codeSystem in codingSystems)
            {
                if (codeSystemDAL.GetByValue(codeSystem.Value) == null)
                {
                    codingSystemsToAdd.Add(codeSystem);
                }
            }
            codeSystemDAL.InsertMany(codingSystemsToAdd);
        }
        #endregion /Common
    }
}
