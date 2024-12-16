using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.File;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.CodeSetEntry;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.CodeEntry.DataIn;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.SqlDomain.BulkOperations;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CodeSetBLL : ICodeSetBLL
    {
        private readonly ICodeSetDAL codeSetDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeDAL codeDAL;
        private readonly ICodeSystemDAL codeSystemDAL;
        private readonly IMapper Mapper;
        private readonly IConfiguration configuration;

        public CodeSetBLL(ICodeSetDAL codeSetDAL, IThesaurusDAL thesaurusDAL, ICodeDAL codeDAL, ICodeSystemDAL codeSystemDAL, IMapper mapper, IConfiguration configuration)
        {
            this.codeSetDAL = codeSetDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.codeDAL = codeDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.configuration = configuration;
            Mapper = mapper;
        }

        public PaginationDataOut<CodeSetDataOut, DataIn> GetAllFiltered(CodeSetFilterDataIn dataIn)
        {
            CodeSetFilter filter = Mapper.Map<CodeSetFilter>(dataIn);
            PaginationDataOut<CodeSetDataOut, DataIn> result = new PaginationDataOut<CodeSetDataOut, DataIn>()
            {
                Count = this.codeSetDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<CodeSetDataOut>>(this.codeSetDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public int GetCodeSetThesaurusId(int codeSetId)
        {
            return codeSetDAL.GetThesaurusId(codeSetId);
        }

        public void Insert(CodeSetDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            CodeSet entry = Mapper.Map<CodeSet>(dataIn);
            codeSetDAL.Insert(entry);
        }

        public async Task InsertAsync(CodeSetDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            CodeSet entry = Mapper.Map<CodeSet>(dataIn);

            await codeSetDAL.InsertAsync(entry);
        }

        public bool ExistCodeSet(int codeSetId) 
        {
            return codeSetDAL.ExistCodeSet(codeSetId);
        }

        public bool ExistCodeSetByPreferredTerm(string codeSetName)
        {
            return codeSetDAL.ExistCodeSetByPreferredTerm(codeSetName); 
        }

        public int GetByPreferredTerm(string preferredTerm)
        {
            int thesaurusId = thesaurusDAL.GetByPreferredTerm(preferredTerm).ThesaurusEntryId;
            return codeSetDAL.GetIdByThesaurusId(thesaurusId);
        }

        public AutocompleteResultDataOut GetAutoCompleteCodes(AutocompleteDataIn dataIn, int codesetId, string activeLanguage)
        {
            int defaultPageSize = 10;
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            var filter = new CodeFilter() { CodeSetId = codesetId, CodeDisplay = dataIn.Term, Page = dataIn.Page, PageSize = defaultPageSize };
            
            List<Code> filtered = codeDAL.GetAll(filter);

            List<AutocompleteDataOut> autocompleteDataDataOuts = filtered
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.CodeId.ToString(),
                    text = x.ThesaurusEntry.Codes.FirstOrDefault()?.Code + " " + x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(activeLanguage)
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autocompleteDataDataOuts,
                pagination = new AutocompletePaginatioDataOut() { more = this.codeDAL.GetAllEntriesCount(filter) > dataIn.Page * defaultPageSize, }
            };

            return result;
        }

        public async Task<AutocompleteResultDataOut> GetAutoCompleteNames(AutocompleteDataIn dataIn, bool onlyApplicableInDesigner, string language)
        {
            int defaultPageSize = 10;
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            CodeSetFilter codeSetFilter = Mapper.Map<CodeSetFilter>(dataIn);
            codeSetFilter.OnlyApplicableInDesigner = onlyApplicableInDesigner;
            codeSetFilter.PageSize = defaultPageSize;

            PaginationData<CodeSet> codeSetsAndCount = await codeSetDAL.GetAllForAutoCompleteNameAndCount(codeSetFilter);

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = codeSetsAndCount.Data.Where(x => x.CodeSetId.ToString() != dataIn.ExcludeId).Select(x => new AutocompleteDataOut()
                {
                    id = x.CodeSetId.ToString(),
                    text = x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(language)
                }).ToList(),
                pagination = new AutocompletePaginatioDataOut() { more = codeSetsAndCount.Count > dataIn.Page * defaultPageSize }
            };
            return result;
        }

        public string GetCodedCodeSetDisplay(int codeSetId)
        {
            return GetById(codeSetId)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
        }

        #region Import CodeSet from CSV

        public bool ImportFileFromCsv(IFormFile file, string codesetName, bool applicableInDesigner)
        {
            bool success = false;

            if (file != null && !string.IsNullOrWhiteSpace(codesetName))
            {
                if (codeSetDAL.GetIdByPreferredTerm(codesetName) != 0)
                    throw new DuplicateException($"Codeset with given name ({codesetName}) is already defined!");

                int newCodeSetId = codeSetDAL.GetNextCodeSetId();
                InsertCodeset(codesetName, newCodeSetId, applicableInDesigner);
                int codeSystemId = InsertCodeSystem(codesetName);

                success = ProcessCsvFileRowsInBatch(file, newCodeSetId, codeSystemId);
            }
            return success;
        }

        private int InsertCodeSystem(string codeSystemName)
        {
            CodeSystem codeSystem = new CodeSystem() { Label = codeSystemName, Value = codeSystemName };
            codeSystemDAL.InsertOrUpdate(codeSystem);
            return codeSystem.CodeSystemId;
        }

        private void InsertCodeset(string codesetName, int codesetId, bool applicableInDesigner)
        {
            int thesaurusId;
            ThesaurusEntry thesaurusEntryDb = thesaurusDAL.GetByPreferredTerm(codesetName);

            if (thesaurusEntryDb != null)
            {
                thesaurusId = thesaurusEntryDb.ThesaurusEntryId;
            }
            else
            {
                ThesaurusEntry thesaurus = Mapper.Map<ThesaurusEntry>(new ThesaurusEntryDataIn()
                {
                    Translations = new List<ThesaurusEntryTranslationDataIn>()
                        {
                            new ThesaurusEntryTranslationDataIn()
                            {
                                Language = LanguageConstants.EN,
                                PreferredTerm = codesetName,
                                Definition = codesetName
                            }
                        }
                });
                thesaurusDAL.InsertOrUpdate(thesaurus);
                thesaurusId = thesaurus.ThesaurusEntryId;
            }

            codeSetDAL.Insert(Mapper.Map<CodeSet>(new CodeSetDataIn()
                    {
                        CodeSetId = codesetId,
                        ThesaurusEntryId = thesaurusId,
                        ApplicableInDesigner = applicableInDesigner
                    }
                )
            );
        }

        private bool ProcessCsvFileRowsInBatch(IFormFile file, int codeSetId, int codeSystemId)
        {
            List<ThesaurusEntry> thesaurusEntries = new List<ThesaurusEntry>();
            List<int> thesaurusEntriesIds = new List<int>();
            List<ThesaurusEntryTranslation> thesaurusEntryTranslations = new List<ThesaurusEntryTranslation>();
            List<O4CodeableConcept> o4CodeableConcepts = new List<O4CodeableConcept>();

            int i = 0;
            foreach (IEnumerable<string> batch in CsvReader.ReadBatches(file.OpenReadStream(), batchSize: 1000))
            {
                var thesaurusEntriesToAdd = Enumerable.Range(0, batch.Count()).Select(e => new ThesaurusEntry()).ToList();
                thesaurusEntries = thesaurusEntries.Concat(thesaurusEntriesToAdd).ToList();

                thesaurusEntriesIds = thesaurusEntriesIds
                    .Concat(BulkOperations.BulkInsert(thesaurusEntriesToAdd, "ThesaurusEntries", configuration))
                    .ToList();

                foreach (var row in batch)
                {
                    thesaurusEntries[i].ThesaurusEntryId = thesaurusEntriesIds[i];
                    ProcessCsvRow(row, ref thesaurusEntryTranslations, ref o4CodeableConcepts, thesaurusEntriesIds[i], codeSystemId);
                    i++;
                }
            }

            BulkOperations.BulkInsert(o4CodeableConcepts, "O4CodeableConcepts", configuration);
            BulkOperations.BulkInsert(thesaurusEntryTranslations, "ThesaurusEntryTranslations", configuration);

            bool success = InsertCodes(thesaurusEntries, codeSetId);
            return success;
        }

        private void ProcessCsvRow(string row, ref List<ThesaurusEntryTranslation> thesaurusEntryTranslations, ref List<O4CodeableConcept> o4CodeableConcepts, int thesaurusEntriesId, int codeSystemId)
        {
            var cells = row.SplitCsvRow();

            if (cells.Length == 3)
            {
                string version = cells[0], code = cells[1], label = cells[2];
                o4CodeableConcepts.Add(new O4CodeableConcept() { ThesaurusEntryId = thesaurusEntriesId, CodeSystemId = codeSystemId, Version = version, Code = code, Value = label });
                thesaurusEntryTranslations.Add(new ThesaurusEntryTranslation() { ThesaurusEntryId = thesaurusEntriesId, Language = LanguageConstants.EN, PreferredTerm = label, Definition = label });
            }
            else
            {
                throw new ArgumentException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} received a CSV row with more than 3 Cells, during CSV CodeSet Upload. Row : {row}");
            }
        }

        private bool InsertCodes(List<ThesaurusEntry> thesauruses, int codeSetId)
        {
            List<Code> codesToInsert = new List<Code>();

            Code code = Mapper.Map<Code>(new CodeDataIn
            {
                CodeSetId = codeSetId
            });

            foreach (var thesaurusToAdd in thesauruses)
            {
                codesToInsert.Add(new Code(code.CreatedById)
                {
                    ThesaurusEntryId = thesaurusToAdd.ThesaurusEntryId,
                    CodeSetId = code.CodeSetId
                });
            }

            List<int> insertedCodeIds = BulkOperations.BulkInsert(codesToInsert, "Codes", configuration);

            bool success = insertedCodeIds.Count == thesauruses.Count;

            return success;
        }

        #endregion

        public CodeSetDataOut GetById(int codeSetId)
        {
            CodeSet codeSet = codeSetDAL.GetById(codeSetId);
            return Mapper.Map<CodeSetDataOut>(codeSet);
        }

        public List<CodeSetDataOut> GetAllByPreferredTerm(string preferredTerm)
        {
            return Mapper.Map<List<CodeSetDataOut>>(codeSetDAL.GetAllByPreferredTerm(preferredTerm).ToList());
        }

        public void Delete(int codeSetId)
        {
            codeSetDAL.Delete(codeSetId);
        }
    }
}
