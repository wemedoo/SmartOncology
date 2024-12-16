using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CodeAssociationDAL : ICodeAssociationDAL
    {
        private readonly SReportsContext context;
        public CodeAssociationDAL(SReportsContext context)
        {
            this.context = context;
        }
        public IQueryable<CodeAssociation> GetAll()
        {
            return context.CodeAssociations
              .WhereEntriesAreActive()
              .Include(x => x.Parent)
              .Include(x => x.Child)
              .Include(x => x.Parent.ThesaurusEntry)
              .Include(x => x.Child.ThesaurusEntry)
              .Include(x => x.Parent.ThesaurusEntry.Translations)
              .Include(x => x.Child.ThesaurusEntry.Translations);
        }

        public int GetAllEntriesCount(CodeAssociationFilter filter)
        {
            if (filter.IsChildToParent)
                return this.GetAllChildEntities(filter).Count();
            else
                return this.GetAllParentEntities(filter).Count();
        }

        public void Insert(List<CodeAssociation> codeAssociations, DateTimeOffset? activeTo = null)
        {
            foreach (var codeAssociation in codeAssociations)
            {
                CodeAssociation codeAssociationFromDb = this.GetById(codeAssociation.CodeAssociationId);

                if (codeAssociationFromDb == null)
                {
                    codeAssociation.CodeAssociationId = 0;
                    context.CodeAssociations.Add(codeAssociation);
                }
                else
                {
                    codeAssociationFromDb.Copy(codeAssociation);
                    if (activeTo != null)
                        codeAssociationFromDb.ActiveTo = activeTo.Value;
                }
                context.SaveChanges();
            }
        }

        public CodeAssociation GetById(int codeAssociationId)
        {
            return context.CodeAssociations
                  .Include(x => x.Parent)
                  .Include(x => x.Child)
                  .Include(x => x.Parent.ThesaurusEntry)
                  .Include(x => x.Child.ThesaurusEntry)
                  .Include(x => x.Parent.ThesaurusEntry.Translations)
                  .Include(x => x.Child.ThesaurusEntry.Translations)
                  .WhereEntriesAreActive()
                  .FirstOrDefault(x => x.CodeAssociationId == codeAssociationId);
        }

        public List<CodeAssociation> GetAll(CodeAssociationFilter filter)
        {
            IQueryable<CodeAssociation> result;
            if (filter.IsChildToParent)
                result = GetAllChildEntities(filter);
            else
                result = GetAllParentEntities(filter);

            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderBy(x => x.EntryDatetime);

            return result.ToList();
        }

        public async Task<PaginationData<CodeAssociation>> GetAllForCodedFieldsAndCount(CodeAssociationFilter filter)
        {
            IQueryable<CodeAssociation> query = GetAll();

            if (filter.CodeSetId > 0)
                query = query.Where(x => x.Parent.CodeSetId == filter.CodeSetId);
            if (filter.ParentId > 0)
                query = query.Where(x => x.ParentId == filter.ParentId);
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(x => x.Parent.ThesaurusEntry.Translations.Any(y => y.PreferredTerm.ToLower().Contains(filter.SearchTerm.ToLower())));

            query = query.GroupBy(x => x.ParentId).Select(g => g.FirstOrDefault()).Include(x => x.Parent.ThesaurusEntry.Translations);

            int count = await query.CountAsync().ConfigureAwait(false);

            if (filter.ColumnName != null)
                query = SortByField(query, filter);
            else
                query = query.OrderByDescending(x => x.EntryDatetime);

            return new PaginationData<CodeAssociation>(count, await query.ToListAsync().ConfigureAwait(false));
        }

        private IQueryable<CodeAssociation> SortByField(IQueryable<CodeAssociation> result, CodeAssociationFilter filterData)
        {
            if (filterData.ColumnName.Contains("_"))
                filterData.ColumnName = filterData.ColumnName.Split('_')[1];
            
            switch (filterData.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.Parent.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage)).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.Parent.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage)).AsQueryable();
                case AttributeNames.ChildPreferredTerm:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.Child.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage)).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.Child.ThesaurusEntry.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN, filterData.ActiveLanguage)).AsQueryable();
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
            }
        }

        private IQueryable<CodeAssociation> GetAllChildEntities(CodeAssociationFilter filter)
        {
            IQueryable<CodeAssociation> codeSetQuery = this.GetAll()
                .Where(x => x.Parent.CodeSetId == filter.CodeSetId);

            return codeSetQuery;
        }

        private IQueryable<CodeAssociation> GetAllParentEntities(CodeAssociationFilter filter)
        {
            IQueryable<CodeAssociation> codeSetQuery = this.GetAll()
                .Where(x => x.Child.CodeSetId == filter.CodeSetId);

            return codeSetQuery;
        }

        public bool ExistAssociation(int parentId, int childId)
        {
            if (parentId == childId)
                return true;

            return this.context.CodeAssociations.WhereEntriesAreActive().Any(x => x.ParentId == parentId && x.ChildId == childId);
        }

        public void Delete(int associationId)
        {
            CodeAssociation fromDb = this.GetById(associationId);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        public List<CodeAssociation> GetAllByParentId(int parentId)
        {
            return context.CodeAssociations
                .WhereEntriesAreActive()
                .Where(x => x.ParentId == parentId).ToList();
        }

        public List<CodeAssociation> GetAllByParentOrChildId(int codeId)
        {
            return context.CodeAssociations
                .Where(x => x.ParentId == codeId || x.ChildId == codeId).ToList();
        }

        public List<int> GetByParentId(int parentId)
        {
            List<int> result = context.CodeAssociations
                 .WhereEntriesAreActive()
                 .Where(x => x.ParentId == parentId)
                 .Select(x => x.ChildId ?? 0)
                 .ToList();

            return result;
        }

        public Dictionary<int, Dictionary<int, string>> InitializeMissingValueList(string language = LanguageConstants.EN) 
        {
            return new Dictionary<int, Dictionary<int, string>>
            {
                { (int)CodeSetList.MissingValueNumber, GetCodeMissingValues((int)CodeSetList.MissingValueNumber, language)},
                { (int)CodeSetList.MissingValueDate, GetCodeMissingValues((int)CodeSetList.MissingValueDate, language)},
                { (int)CodeSetList.MissingValueDateTime, GetCodeMissingValues((int)CodeSetList.MissingValueDateTime, language)},
                { (int)CodeSetList.NullFlavor, GetCodeMissingValues((int)CodeSetList.NullFlavor, language)},
            };
        }

        private Dictionary<int, string> GetCodeMissingValues(int codeSetId, string language)
        {
            if (codeSetId == (int)CodeSetList.NullFlavor) 
            {
                return context.Codes
                    .WhereEntriesAreActive()
                    .Where(x => x.CodeSetId == codeSetId).AsEnumerable()
                    .ToDictionary(
                        code => code.CodeId,
                        code => code.ThesaurusEntry?.GetPreferredTermByTranslationOrDefault(language)?.ToString() ?? ""
                    );
            }
            else
            {
                List<Code> codes = context.Codes
                    .WhereEntriesAreActive()
                    .Where(x => x.CodeSetId == codeSetId)
                    .Include(x => x.ThesaurusEntry)
                    .Include(x => x.ThesaurusEntry.Translations)
                    .ToList();

                return codes.ToDictionary(
                    code => context.CodeAssociations
                        .WhereEntriesAreActive()
                        .Where(x => x.ChildId == code.CodeId)
                        .FirstOrDefault().ParentId
                    ,
                    code => code.ThesaurusEntry?.GetPreferredTermByTranslationOrDefault(language)?.ToString() ?? ""
                );
            }
        }
    }
}
