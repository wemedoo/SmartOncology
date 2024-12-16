using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeSetEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CodeSetDAL : ICodeSetDAL
    {
        private readonly SReportsContext context;
        public CodeSetDAL(SReportsContext context)
        {
            this.context = context;
        } 

        public IQueryable<CodeSet> GetAll()
        {
            return context.CodeSets
                .WhereEntriesAreActive()
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations);
        }

        public async Task InsertAsync(CodeSet codeSet)
        {
            CodeSet codeSetFromDb = await GetByIdAsync(codeSet.CodeSetId);

            if (codeSetFromDb == null)
            {
                codeSet.SetActiveFromAndToDatetime();
                context.CodeSets.Add(codeSet);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                codeSetFromDb.Copy(codeSet);
                await context.SaveChangesAsync().ConfigureAwait(false);

                try
                {
                    context.Database.ExecuteSqlRaw("UPDATE CodeSets SET [CodeSetId] = {0} WHERE [CodeSetId] = {1}", codeSet.NewCodeSetId, codeSet.CodeSetId);
                }
                catch 
                {
                    throw new DuplicateException($"Code Set with CodeSetId {(codeSet.NewCodeSetId != 0 ? codeSet.NewCodeSetId : codeSet.CodeSetId)} already exists!");
                }
            }
        }

        public void Insert(CodeSet codeSet)
        {
            CodeSet codeSetFromDb = this.GetById(codeSet.CodeSetId);

            if (codeSetFromDb == null)
            {
                codeSet.SetActiveFromAndToDatetime();
                context.CodeSets.Add(codeSet);
                context.SaveChanges();
            }
            else
            {
                codeSetFromDb.Copy(codeSet);
                context.SaveChanges();

                try
                {
                    context.Database.ExecuteSqlRaw("UPDATE CodeSets SET [CodeSetId] = {0} WHERE [CodeSetId] = {1}", codeSet.NewCodeSetId, codeSet.CodeSetId);
                }
                catch(Exception)
                {
                    throw new DuplicateException($"Code Set with CodeSetId {(codeSet.NewCodeSetId != 0 ? codeSet.NewCodeSetId : codeSet.CodeSetId)} already exists!");
                }
            }
        }

        public int GetIdByPreferredTerm(string preferredTerm)
        {
            return context.CodeSets.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == preferredTerm))
                .Select(x => x.CodeSetId).FirstOrDefault();
        }

        public CodeSet GetByPreferredTerm(string preferredTerm)
        {
            return context.CodeSets.Where(x => x.ThesaurusEntry.Translations
                .Any(m => m.PreferredTerm == preferredTerm)).FirstOrDefault();
        }

        public int GetAllEntriesCount(CodeSetFilter filter)
        {
            return this.GetCodeSetFiltered(filter).Count();
        }

        public List<CodeSet> GetAll(CodeSetFilter filter)
        {
            IQueryable<CodeSet> result = GetCodeSetFiltered(filter);

            result = ApplyOrderByAndPagination(filter, result);

            return result.ToList();
        }

        public IQueryable<CodeSet> GetAllByPreferredTerm(string preferredTerm)
        {
            IQueryable<CodeSet> codeSetQuery = this.context.CodeSets
                .WhereEntriesAreActive()
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations);

            codeSetQuery = codeSetQuery.Where(x => x.ThesaurusEntry.Translations.Any(y => y.PreferredTerm.Contains(preferredTerm)));

            return codeSetQuery;
        }

        private IQueryable<CodeSet> GetCodeSetFiltered(CodeSetFilter filter)
        {
            IQueryable<CodeSet> codeSetQuery = this.context.CodeSets
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations);

            if (filter.CodeSetId != null)
            {
                codeSetQuery = codeSetQuery.Where(x => x.CodeSetId == filter.CodeSetId);
            }
            if (!string.IsNullOrEmpty(filter.CodeSetDisplay))
            {
                codeSetQuery = codeSetQuery.Where(x => x.ThesaurusEntry.Translations
                    .Any(y => y.PreferredTerm.ToLower().Contains(filter.CodeSetDisplay.ToLower())));
            }
            if (filter.ShowActive && !filter.ShowInactive)
            {
                codeSetQuery = codeSetQuery.WhereEntriesAreActive();
            }
            if (!filter.ShowActive && filter.ShowInactive)
            {
                codeSetQuery = codeSetQuery.WhereEntriesAreInactive();
            }
            if (filter.OnlyApplicableInDesigner)
            {
                codeSetQuery = codeSetQuery.Where(x => x.ApplicableInDesigner);
            }

            return codeSetQuery;
        }

        public CodeSet GetById(int codeSetId)
        {
            return context.CodeSets
                     .Include(x => x.ThesaurusEntry)
                     .Include(x => x.ThesaurusEntry.Translations)
                     .FirstOrDefault(x => x.CodeSetId == codeSetId);
        }

        public async Task<CodeSet> GetByIdAsync(int codeSetId)
        {
            return await context.CodeSets
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations)
                .FirstOrDefaultAsync(x => x.CodeSetId == codeSetId);
        }

        private IQueryable<CodeSet> SortByField(IQueryable<CodeSet> result, CodeSetFilter filterData)
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

        public int GetThesaurusId(int codeSetId)
        {
            return context.CodeSets
              .WhereEntriesAreActive()
              .FirstOrDefault(x => x.CodeSetId == codeSetId).ThesaurusEntryId;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.CodeSets
                .WhereEntriesAreActive()
                .Any(x => x.ThesaurusEntryId == thesaurusId);
        }

        public bool ExistCodeSet(int codeSetId)
        {
            return this.context.CodeSets
                .WhereEntriesAreActive()
                .Any(x => x.CodeSetId.Equals(codeSetId));
        }

        public bool ExistCodeSetByPreferredTerm(string preferredTerm)
        {
            return this.context.CodeSets
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations)
                .WhereEntriesAreActive()
                .Any(x => x.ThesaurusEntry.Translations.Any(y => y.PreferredTerm.ToLower().Equals(preferredTerm.ToLower())));
        }

        public int GetIdByThesaurusId(int thesaurusId)
        {
            return context.CodeSets
               .WhereEntriesAreActive()
               .FirstOrDefault(x => x.ThesaurusEntryId == thesaurusId).CodeSetId;
        }

        public int UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            int entriesUpdated = 0;
            List<CodeSet> codeSets = context.CodeSets.Where(x => x.ThesaurusEntryId == oldThesaurus).ToList();
            foreach (CodeSet codeSet in codeSets)
            {
                codeSet.ReplaceThesauruses(oldThesaurus, newThesaurus);
                ++entriesUpdated;
            }

            context.SaveChanges();

            return entriesUpdated;
        }

        public int GetNextCodeSetId()
        {
            return context.CodeSets.Select(x => x.CodeSetId).OrderByDescending(id => id).FirstOrDefault() + 1;  // taking also Inactive/Deleted
        }

        public void Delete(int codeSetId)
        {
            CodeSet fromDb = this.GetById(codeSetId);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        public async Task<PaginationData<CodeSet>> GetAllForAutoCompleteNameAndCount(CodeSetFilter filter)
        {
            IQueryable<CodeSet> query = GetCodeSetFiltered(filter);

            int count = await query.CountAsync().ConfigureAwait(false);

            query = ApplyOrderByAndPagination(filter, query);

            return new PaginationData<CodeSet>(count, await query.ToListAsync().ConfigureAwait(false));
        }

        private IQueryable<CodeSet> ApplyOrderByAndPagination(CodeSetFilter filter, IQueryable<CodeSet> query)
        {
            if (filter.ColumnName != null)
                query = SortByField(query, filter);
            else
                query = query.OrderByDescending(x => x.EntryDatetime)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);
            return query;
        }
    }
}
