using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusDAL : IThesaurusDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public ThesaurusDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public ThesaurusEntry GetById(int id)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .Include("Codes.System")
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ThesaurusEntryId == id);
        }

        public List<ThesaurusEntry> GetAllByIds(IEnumerable<int> ids)
        {
            return context.Thesauruses
                .Include(x => x.Codes)
                .Where(x => ids.Contains(x.ThesaurusEntryId)).ToList();
        }

        public O4CodeableConcept GetCodeById(int id)
        {
            return context.O4CodeableConcepts
                .FirstOrDefault(x => x.O4CodeableConceptId == id);
        }

        public List<ThesaurusEntry> GetFiltered(GlobalThesaurusFilter filterDataIn)
        {
            return this.GetFilteredQuery(filterDataIn)
                      .OrderBy(x => x.ThesaurusEntryId)
                      .Skip((filterDataIn.Page - 1) * filterDataIn.PageSize)
                      .Take(filterDataIn.PageSize)
                      .ToList(); 
        }

        public int GetFilteredCount(GlobalThesaurusFilter filterDataIn)
        {
            return this.GetFilteredQuery(filterDataIn).Count();
        }

        public IQueryable<ThesaurusEntry> GetFilteredQuery(GlobalThesaurusFilter filterDataIn)
        {
            IQueryable<ThesaurusEntry> result = context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.Codes)
                .WhereEntriesAreActive();

            if (filterDataIn != null && !string.IsNullOrWhiteSpace(filterDataIn.Term)) 
            {
                if (!string.IsNullOrWhiteSpace(filterDataIn.TermIndicator) && filterDataIn.TermIndicator.Equals("exact"))
                {
                    result = result.Where(x => x.Translations.Any(t => t.PreferredTerm.Equals(filterDataIn.Term) && (filterDataIn.Language == null || t.Language == filterDataIn.Language)));
                }
                else
                {
                    result = result.Where(x => x.Translations.Any(t => t.PreferredTerm.Contains(filterDataIn.Term) && (filterDataIn.Language == null || t.Language == filterDataIn.Language)));
                }
            }

            return result;
        }

        public void InsertOrUpdate(ThesaurusEntry thesaurus)
        {
            if (thesaurus.ThesaurusEntryId == 0)
            {
                context.Thesauruses.Add(thesaurus);
            }
            else
            {
                thesaurus.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public void DeleteCode(int id)
        {
            O4CodeableConcept code = context.O4CodeableConcepts.FirstOrDefault(x => x.O4CodeableConceptId == id);
            code.IsDeleted = true;
            context.SaveChanges();
        }

        public int GetAllCount()
        {
            return context.Thesauruses.WhereEntriesAreActive().Count();
        }

        public int GetAllEntriesCount(ThesaurusEntryFilterData filterData)
        {
            return GetThesaurusEntriesFiltered(filterData).Count();
        }

        public long GetUmlsEntriesCount()
        {
            return context.Thesauruses
                .Where(x => x.Codes.Any(c => c.System.SAB == "MTH"))
                .Count();
        }

        //Import thesaurus from UMLS section
        public void InsertMany(List<ThesaurusEntry> thesauruses)
        {
            DataTable thesaurusesTable = new DataTable();
            thesaurusesTable.Columns.Add(new DataColumn("StateCD", typeof(int)) { AllowDBNull = true });
            thesaurusesTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTimeOffset)));
            thesaurusesTable.Columns.Add(new DataColumn("ActiveFrom", typeof(DateTimeOffset)));
            thesaurusesTable.Columns.Add(new DataColumn("ActiveTo", typeof(DateTimeOffset)));
            thesaurusesTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTimeOffset)));
            thesaurusesTable.Columns.Add(new DataColumn("PreferredLanguage", typeof(string)));
            thesaurusesTable.Columns.Add(new DataColumn("AdministrativeDataId", typeof(int)));

            int i = 1;
            foreach (var thesaurus in thesauruses)
            {
                DataRow thesaurusRow = thesaurusesTable.NewRow();
                thesaurusRow["StateCD"] = thesaurus.StateCD;
                thesaurusRow["EntryDatetime"] = thesaurus.EntryDatetime;
                thesaurusRow["ActiveFrom"] = thesaurus.ActiveFrom;
                thesaurusRow["ActiveTo"] = thesaurus.ActiveTo;
                thesaurusRow["LastUpdate"] = DBNull.Value;
                thesaurusRow["PreferredLanguage"] = thesaurus.PreferredLanguage;
                thesaurusRow["AdministrativeDataId"] = i;
                thesaurusesTable.Rows.Add(thesaurusRow);
                i++;
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "ThesaurusEntries"
            };
            objbulk.ColumnMappings.Add("StateCD", "StateCD");
            objbulk.ColumnMappings.Add("EntryDateTime", "EntryDatetime");
            objbulk.ColumnMappings.Add("ActiveFrom", "ActiveFrom");
            objbulk.ColumnMappings.Add("ActiveTo", "ActiveTo");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");
            objbulk.ColumnMappings.Add("PreferredLanguage", "PreferredLanguage");
            objbulk.ColumnMappings.Add("AdministrativeDataId", "AdministrativeDataId");

            con.Open();
            objbulk.WriteToServer(thesaurusesTable);
            con.Close();
        }

        public List<int> GetLastBulkInserted(int size)
        {
             return context.Thesauruses.OrderByDescending(x => x.EntryDatetime).Take(size).Select(x => x.ThesaurusEntryId).ToList();           
        }

        public List<int> GetBulkInserted(int size)
        {
            return context.Thesauruses.OrderBy(x => x.ThesaurusEntryId).Take(size).Select(x => x.ThesaurusEntryId).ToList();
        }
        //End import thesaurus from UMLS section

        private IQueryable<ThesaurusEntryView> GetThesaurusEntriesFiltered(ThesaurusEntryFilterData filterData)
        {
            IQueryable<ThesaurusEntryView> result = context.ThesaurusEntryViews.WhereEntriesAreActive();

            if (filterData != null)
            {
                result = result.Where(x => x.Language.Equals(filterData.ActiveLanguage) && (filterData.Id == 0 || x.ThesaurusEntryId.Equals(filterData.Id)));

                if (filterData.StateCD.HasValue) 
                {
                    result = result.Where(x => x.StateCD == filterData.StateCD);
                }

                if (!string.IsNullOrEmpty(filterData.PreferredTerm))
                {
                    if (filterData.IsSearchTable)
                        result = result.Where(x => x.Language.Equals(filterData.ActiveLanguage) && x.PreferredTerm.StartsWith(filterData.PreferredTerm));
                    else
                        result = result.Where(x => x.PreferredTerm.Contains(filterData.PreferredTerm));
                }

                if (!string.IsNullOrEmpty(filterData.Abbreviation))
                {
                    result = result.Where(x => !string.IsNullOrEmpty(x.AbbreviationsString) && x.AbbreviationsString.Contains(filterData.Abbreviation));
                }

                if (!string.IsNullOrEmpty(filterData.Synonym))
                {
                    result = result.Where(x => !string.IsNullOrEmpty(x.SynonymsString) && x.SynonymsString.Contains(filterData.Synonym));
                }
            }

            return result;
        }

        public List<ThesaurusEntryView> GetAll(ThesaurusEntryFilterData filterData)
        {
            IQueryable<ThesaurusEntryView> result = GetThesaurusEntriesFiltered(filterData);

            if (filterData.ColumnName != null)
                result = SortByField(result, filterData);
            else if (filterData.PreferredTerm != null && filterData.IsSearchTable)
                result = result.OrderBy(x => x.PreferredTerm)
                    .Skip((filterData.Page - 1) * filterData.PageSize)
                    .Take(filterData.PageSize);
            else
                result = result.OrderByDescending(x => x.ThesaurusEntryId)
                    .Skip((filterData.Page - 1) * filterData.PageSize)
                    .Take(filterData.PageSize);

            return result.ToList();
        }

        public bool ExistsThesaurusEntry(int id)
        {
            return context.Thesauruses.Any(x => x.ThesaurusEntryId == id);
        }

        public void Delete(int id, string organizationTimeZone = null)
        {
            var fromDb = GetById(id);
            if(fromDb != null)
            {
                fromDb.Delete(organizationTimeZone: organizationTimeZone);
                context.SaveChanges();
            }
        }

        public List<ThesaurusEntry> GetAllSimilar(ThesaurusReviewFilterData filter, string preferredTerm, string language, int? productionStateCD)
        {
            return !string.IsNullOrWhiteSpace(preferredTerm) ? this.GetAllSimilarQuery(filter, preferredTerm, language, productionStateCD)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList() : new List<ThesaurusEntry>();
        }

        public long GetAllSimilarCount(ThesaurusReviewFilterData filter, string preferredTerm, string language, int? productionStateCD)
        {
            return !string.IsNullOrWhiteSpace(preferredTerm) ? this.GetAllSimilarQuery(filter, preferredTerm, language, productionStateCD).Count() : 0;
        }

        private IQueryable<ThesaurusEntry> GetAllSimilarQuery(ThesaurusReviewFilterData filter, string preferredTerm, string language, int? productionStateCD)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .WhereEntriesAreActive()
                .Where(x => x.ThesaurusEntryId != filter.Id && x.StateCD == productionStateCD)
                .Where(x => x.Translations.Any(y => y.Language == language && y.PreferredTerm.Contains(preferredTerm)));
        }

        public void UpdateState(int thesaurusId, int? stateCD)
        {
            ThesaurusEntry thesaurus = this.GetById(thesaurusId);
            thesaurus.StateCD = stateCD;
            context.SaveChanges();
        }

        public List<ThesaurusEntry> GetByIdsList(List<int> thesaurusList)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .WhereEntriesAreActive()
                .Where(x => thesaurusList.Contains(x.ThesaurusEntryId))
                .ToList();
        }

        public List<string> GetAll(string language, string searchValue, int page)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .WhereEntriesAreActive()
                .Where(z => z.Translations
                    .Any(y => y.Language.Equals(language) && y.PreferredTerm.Contains(searchValue)))
                .OrderBy(x => x.ThesaurusEntryId)
                .Skip(page).Take(10)
                .AsEnumerable()
                .Select(m => m.GetPreferredTermByActiveLanguage(language))
                .ToList();
        }

        public int GetThesaurusIdThatHasCodeableConcept(string codeValue)
        {
            return context.O4CodeableConcepts.Where(x => x.Value == codeValue && !x.IsDeleted)
                .Select(x => x.ThesaurusEntryId).FirstOrDefault().GetValueOrDefault();
        }

        public ThesaurusEntry GetByPreferredTerm(string prefferedTerm, string language = LanguageConstants.EN)
        {
            return context.ThesaurusEntryTranslations.Where(translation => translation.PreferredTerm == prefferedTerm && translation.Language == language)
                .Select(translation => translation.ThesaurusEntry).FirstOrDefault();
        }

        public int GetIdByPreferredTerm(string preferredTerm, string language = LanguageConstants.EN)
        {
            return context.ThesaurusEntryTranslations.Where(translation => translation.PreferredTerm == preferredTerm).Select(translation => translation.ThesaurusEntryId).FirstOrDefault();
        }

        private IQueryable<ThesaurusEntryView> SortByField(IQueryable<ThesaurusEntryView> result, ThesaurusEntryFilterData filterData)
        {
            return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                  .Skip((filterData.Page - 1) * filterData.PageSize)
                  .Take(filterData.PageSize);
        }
    }
}
