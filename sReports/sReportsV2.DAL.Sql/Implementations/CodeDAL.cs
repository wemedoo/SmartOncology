using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using sReportsV2.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class CodeDAL : BaseDisposalDAL, ICodeDAL
    {
        private readonly IConfiguration configuration;

        public CodeDAL(SReportsContext context, IConfiguration configuration) : base(context)
        {
            this.configuration = configuration;
        }

        public bool CodesExist(int codeSetId)
        {
            return context.Codes
                .WhereEntriesAreActive()
                .Any(x => x.CodeSetId == codeSetId);
        }

        public void Delete(int codeId)
        {
            Code fromDb = this.GetById(codeId);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        public IQueryable<Code> GetAll()
        {
            return context.Codes
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations);        
        }

        public List<Code> GetAll(CodeFilter filter)
        {
            IQueryable<Code> result = GetCodeFiltered(filter);

            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderByDescending(x => x.EntryDatetime)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

            return result.ToList();
        }

        public List<Code> GetAllAssociationsFiltered(CodeFilter filter)
        {
            IQueryable<Code> result = GetCodeAssociationFiltered(filter);

            if (filter.ColumnName != null)
                result = SortAssociationByField(result, filter);
            else
                result = result.OrderByDescending(x => x.EntryDatetime);

            return result.ToList();
        }

        public int Insert(Code code, string organizationTimeZone = null)
        {
            Code codeFromDb = this.GetById(code.CodeId);

            if (codeFromDb == null)
            {
                code.SetActiveFromAndToDatetime(organizationTimeZone);
                context.Codes.Add(code);
            }
            else
            {
                codeFromDb.Copy(code, organizationTimeZone);
            }
            context.SaveChanges();

            return code.CodeId;
        }

        public int Update(Code code)
        {
            Code codeFromDb = this.GetById(code.CodeId);
            codeFromDb.Copy(code);

            return code.CodeId;
        }

        public Code GetById(int codeId)
        {
            return context.Codes
                    .Include(x => x.ThesaurusEntry)
                    .Include(x => x.ThesaurusEntry.Translations)
                    .Include(x => x.ThesaurusEntry.Codes)
                    .FirstOrDefault(x => x.CodeId == codeId);
        }

        public List<Code> GetByIds(List<int> codeIds)
        {
            return context.Codes
                    .Include(x => x.ThesaurusEntry)
                    .Include(x => x.ThesaurusEntry.Translations)
                    .Include(x => x.ThesaurusEntry.Codes)
                    .Where(x => codeIds.Contains(x.CodeId))
                    .ToList()
                    ;
        }

        public void InsertMany(List<int> bulkedThesauruses, int? codeSetId)
        {
            DataTable customEnumDataTable = new DataTable();
            customEnumDataTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));
            customEnumDataTable.Columns.Add(new DataColumn("CodeSetId", typeof(int)));
            customEnumDataTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTimeOffset)));
            customEnumDataTable.Columns.Add(new DataColumn("ActiveFrom", typeof(DateTimeOffset)));
            customEnumDataTable.Columns.Add(new DataColumn("ActiveTo", typeof(DateTimeOffset)));
            customEnumDataTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTimeOffset)));

            for (int i = 0; i < bulkedThesauruses.Count; i++)
            {
                DataRow translationRow = customEnumDataTable.NewRow();
                translationRow["ThesaurusEntryId"] = bulkedThesauruses[i];
                translationRow["CodeSetId"] = codeSetId;
                translationRow["EntryDatetime"] = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
                translationRow["ActiveFrom"] = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
                translationRow["ActiveTo"] = DateTimeOffset.MaxValue;
                translationRow["LastUpdate"] = DBNull.Value;

                customEnumDataTable.Rows.Add(translationRow);
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "Codes"
            };
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");
            objbulk.ColumnMappings.Add("CodeSetId", "CodeSetId");
            objbulk.ColumnMappings.Add("EntryDatetime", "EntryDatetime");
            objbulk.ColumnMappings.Add("ActiveFrom", "ActiveFrom");
            objbulk.ColumnMappings.Add("ActiveTo", "ActiveTo");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");

            con.Open();
            objbulk.WriteToServer(customEnumDataTable);
            con.Close();
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context
                .Codes
                .WhereEntriesAreActive()
                .Any(x => x.ThesaurusEntryId == thesaurusId);
        }

        public int UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            int entriesUpdated = 0;
            foreach(Code code in GetAllByThesaurusId(oldThesaurus))
            {
                code.ReplaceThesauruses(oldThesaurus, newThesaurus);
                ++entriesUpdated;
            }
            context.SaveChanges();

            return entriesUpdated;
        }

        public Code GetByPreferredTerm(string preferredTerm, int? codeSetId, string language = LanguageConstants.EN)
        {
            return context.Codes.Where(x => x.ThesaurusEntry.Translations
                .Any(m => m.PreferredTerm == preferredTerm && m.Language == language) && x.CodeSetId == codeSetId).FirstOrDefault();
        }

        public Code GetByPreferredTerm(string preferredTerm, string codeSet, string language = LanguageConstants.EN)
        {
            int? codeSetId = context.CodeSets.Where(x => x.ThesaurusEntry.Translations
                .Any(th => th.PreferredTerm == codeSet && th.Language == language)).Select(x => x.CodeSetId).FirstOrDefault();
            return GetByPreferredTerm(preferredTerm, codeSetId, language);
        }

        public int GetAllEntriesCount(CodeFilter filter)
        {
            return this.GetCodeFiltered(filter).Count();
        }

        public int GetAllAssociationsCount(CodeFilter filter)
        {
            return this.GetCodeAssociationFiltered(filter).Count();
        }

        public List<Code> GetByCodeSetId(int codeSetId)
        {
            return context.Codes
                 .WhereEntriesAreActive()
                 .Where(x => x.CodeSetId == codeSetId)
                 .Include(x => x.ThesaurusEntry)
                 .Include(x => x.ThesaurusEntry.Translations).ToList();
        }

        public List<Code> GetByCodeSet(string codeSet, string language = LanguageConstants.EN)
        {
            int? codeSetId = context.CodeSets.Where(x => x.ThesaurusEntry.Translations
                .Any(th => th.PreferredTerm == codeSet && th.Language == language)).Select(x => x.CodeSetId).FirstOrDefault();
            return GetCommunicationSystems(codeSetId.GetValueOrDefault());
        }

        public int? GetIdByPreferredTermAndNullCodeset(string preferredTerm, string language = LanguageConstants.EN)
        {
            return context.Codes
                .WhereEntriesAreActive()
                .Where(x => 
                    x.ThesaurusEntry.Translations.Any(th => th.PreferredTerm == preferredTerm && th.Language == language) 
                    && x.CodeSetId == null
                    )
                .Select(x => x.CodeId)
                .FirstOrDefault();
        }

        public int UpdateCodesetByCodeId(int codeId, int newCodeSetId)
        {
            Code code = context.Codes.Where(x => x.CodeId == codeId).FirstOrDefault();
            if (code != null)
            {
                code.CodeSetId = newCodeSetId;
                code.SetLastUpdate();
            }
            return context.SaveChanges();
        }

        public int GetIdByPreferredTerm(string preferredTerm)
        {
            return context.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == preferredTerm))
                .Select(x => x.CodeId).FirstOrDefault();
        }

        public int GetByCodeSetIdAndPreferredTerm(int codeSetId, string preferredTerm)
        {
            return context.Codes.Where(x => x.CodeSetId == codeSetId && x.ThesaurusEntry.Translations
                .Any(m => m.PreferredTerm == preferredTerm))
                .Select(x => x.CodeId).FirstOrDefault();
        }

        public List<Code> GetCommunicationSystems(int codeSetId)
        {
            return context.Codes
                 .WhereEntriesAreActive()
                 .Where(x => x.CodeSetId == codeSetId)
                 .Include(x => x.ThesaurusEntry)
                 .Include(x => x.ThesaurusEntry.Translations).ToList();
        }

        public IQueryable<Code> GetCodeFiltered(CodeFilter filter)
        {
            IQueryable<Code> codeQuery = this.context.Codes
                .Where(x => x.CodeSetId == filter.CodeSetId)
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations)
                .Include(x => x.ThesaurusEntry.Codes);

            if (filter.Id != null)
            {
                codeQuery = codeQuery.Where(x => x.CodeId == filter.Id);
            }
            if (!string.IsNullOrEmpty(filter.CodeDisplay))
            {
                codeQuery = codeQuery.Where(x => x.ThesaurusEntry.Translations
                    .Any(y => y.PreferredTerm.Contains(filter.CodeDisplay)) || x.ThesaurusEntry.Codes.Any(y => y.Code.ToLower().Contains(filter.CodeDisplay.ToLower())));
            }
            if (filter.ShowActive && !filter.ShowInactive)
            {
                codeQuery = codeQuery.WhereEntriesAreActive();
            }
            if (!filter.ShowActive && filter.ShowInactive)
            {
                codeQuery = codeQuery.WhereEntriesAreInactive();
            }
            if (filter.CodeSetId != 0)
            {
                codeQuery = codeQuery.Where(x => x.CodeSetId == filter.CodeSetId);
            }

            return codeQuery;
        }

        private IQueryable<Code> GetCodeAssociationFiltered(CodeFilter filter)
        {
            IQueryable<Code> codeQuery = this.context.Codes
                .WhereEntriesAreActive()
                .Where(x => x.CodeSetId == filter.CodeSetId)
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations);

            return codeQuery;
        }

        private IQueryable<Code> SortByField(IQueryable<Code> result, CodeFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
            }
        }

        private IQueryable<Code> SortAssociationByField(IQueryable<Code> result, CodeFilter filterData)
        {
            if (filterData.ColumnName.Contains("_"))
            {
                filterData.ColumnName = filterData.ColumnName.Split('_')[1];
            }
            switch (filterData.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    if (filterData.IsAscending)
                        return result.AsEnumerable()
                                .OrderBy(x => x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage))
                                .AsQueryable();
                    else
                        return result.AsEnumerable()
                                .OrderByDescending(x => x.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage))
                                .AsQueryable();
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
            }
        }

        private List<Code> GetAllByThesaurusId(int thesaurusId)
        {
            return context.Codes
                .WhereEntriesAreActive()
                .Where(x => x.ThesaurusEntryId == thesaurusId)
                .ToList();
        }

        public void SetCodeToInactive(int? codeId, string organizationTimeZone = null)
        {
            Code code = context.Codes.FirstOrDefault(x => x.CodeId == codeId);
            if (code != null)
            {
                code.Delete(setLastUpdateProperty: false, organizationTimeZone: organizationTimeZone);
                context.SaveChanges();
            }
            
        }

        public Code GetByCodeSetIdAndThesaurusId(int? codeSetId, int thesaurusId)
        {
            return context.Codes
                .WhereEntriesAreActive()
                .Where(x => x.CodeSetId == codeSetId && x.ThesaurusEntryId == thesaurusId)
                .FirstOrDefault();
        }

        public int InsertOrUpdateTaskDocumentCode(Code code, string organizationTimeZone = null)
        {
            Code codeFromDb = GetByCodeSetIdAndThesaurusId(code.CodeSetId, code.ThesaurusEntryId);

            if (codeFromDb == null)
            {
                code.SetActiveFromAndToDatetime(organizationTimeZone);
                context.Codes.Add(code);
            }
            else
            {
                codeFromDb.SetActiveFromAndTo(organizationTimeZone);
                code.CodeId = codeFromDb.CodeId;
            }
            context.SaveChanges();

            return code.CodeId;
        }
    }
}
