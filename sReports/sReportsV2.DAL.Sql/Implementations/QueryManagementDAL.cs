using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.QueryManagement;
using sReportsV2.SqlDomain.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;
using System.Linq;
using System.Collections.Generic;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Constants;
using System;

namespace sReportsV2.SqlDomain.Implementations
{
    public class QueryManagementDAL : IQueryManagementDAL
    {
        private readonly SReportsContext context;
        public QueryManagementDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task<Query> GetById(int id)
        {
            var query = await context.Queries.Include(x => x.Reason.ThesaurusEntry)
                .Include(x => x.Reason.ThesaurusEntry.Translations)
                .Include(x => x.Status.ThesaurusEntry)
                .Include(x => x.Status.ThesaurusEntry.Translations)
                .Include(x => x.History)
                    .ThenInclude(y => y.LastUpdateBy)
                .WhereEntriesAreActive()
                .FirstOrDefaultAsync(x => x.QueryId == id);

            return query;
        }

        public async Task<List<Query>> GetListById(int id)
        {
            var query = await context.Queries
                .Include(x => x.Reason.ThesaurusEntry)
                .Include(x => x.Reason.ThesaurusEntry.Translations)
                .Include(x => x.Status.ThesaurusEntry)
                .Include(x => x.Status.ThesaurusEntry.Translations)
                .WhereEntriesAreActive()
                .Where(x => x.QueryId == id)
                .ToListAsync();

            return query;
        }

        public async Task<int> Create(Query query, int? userId = null)
        {
            if (IsQueryAdded(query))
            {
                return 0;
            }

            query.CreatedById = userId;
            context.Queries.Add(query);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return query.QueryId;
        }

        public async Task<int> Update(Query query, int userId, List<QueryHistory> historyList = null)
        {
            query.LastUpdateById = userId;
            context.UpdateEntryMetadata(query);

            await context.SaveChangesAsync().ConfigureAwait(false);
            await AddNewHistories(historyList, userId).ConfigureAwait(false);

            return query.QueryId;
        }

        public async Task Delete(int queryId)
        {
            Query fromDb = await context.Queries.Include(x => x.History).FirstOrDefaultAsync(x => x.QueryId == queryId);
            if (fromDb != null)
            {
                fromDb.Delete();
                await context.SaveChangesAsync();
            }
        }

        private bool IsQueryAdded(Query query)
        {
            return context.Queries.WhereEntriesAreActive().Any(q =>
                q.FieldId == query.FieldId &&
                q.StatusCD == query.StatusCD &&
                q.ReasonCD == query.ReasonCD &&
                q.Description == query.Description
            );
        }

        public async Task<List<Query>> GetAll(QueryFilter filter)
        {
            IQueryable<Query> result = GetQueryFiltered(filter);

            if (string.IsNullOrEmpty(filter.FieldLabel) && (string.IsNullOrEmpty(filter.ColumnName) || filter.ColumnName != AttributeNames.FieldId))
            {
                if (filter.ColumnName != null)
                    result = SortByField(result, filter);
                else
                    result = result.OrderByDescending(x => x.QueryId)
                        .Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize);
            }

            return await result.ToListAsync().ConfigureAwait(false);
        }

        public async Task<int> GetAllEntriesCount(QueryFilter filter)
        {
            return await GetQueryFiltered(filter).CountAsync().ConfigureAwait(false);
        }

        public async Task<List<Query>> GetByFieldId(QueryFilter filter)
        {
            var query = context.Queries
                 .Where(x => x.FieldId == filter.FieldId)
                 .Include(x => x.Reason.ThesaurusEntry)
                 .Include(x => x.Reason.ThesaurusEntry.Translations)
                 .Include(x => x.Status.ThesaurusEntry)
                 .Include(x => x.Status.ThesaurusEntry.Translations)
                 .Include(x => x.History)
                     .ThenInclude(y => y.LastUpdateBy)
                 .WhereEntriesAreActive();

            if (filter.ColumnName != null)
                query = SortQueriesByField(query, filter);
            else
                query = query.OrderByDescending(x => x.QueryId);

            return await query.ToListAsync();
        }

        public List<Query> GetByFieldIds(List<string> fieldIds)
        {
            if (fieldIds == null || fieldIds.Count == 0)
                return new List<Query>();

            return context.Queries
                .Where(x => fieldIds.Contains(x.FieldId))
                .Include(x => x.Reason.ThesaurusEntry)
                .Include(x => x.Reason.ThesaurusEntry.Translations)
                .Include(x => x.Status.ThesaurusEntry)
                .Include(x => x.Status.ThesaurusEntry.Translations)
                .Include(x => x.History)
                    .ThenInclude(y => y.LastUpdateBy)
                .WhereEntriesAreActive()
                .ToList();
        }

        private IQueryable<Query> GetQueryFiltered(QueryFilter filter)
        {
            IQueryable<Query> query = context.Queries
                .Include(x => x.Reason.ThesaurusEntry)
                .Include(x => x.Reason.ThesaurusEntry.Translations)
                .Include(x => x.Status.ThesaurusEntry)
                .Include(x => x.Status.ThesaurusEntry.Translations)
                .WhereEntriesAreActive();

            if (filter.ReasonCD.HasValue)
            {
                query = query.Where(x => x.ReasonCD == filter.ReasonCD);
            }
            if (filter.StatusCD.HasValue)
            {
                query = query.Where(x => x.StatusCD == filter.StatusCD);
            }
            if (!string.IsNullOrEmpty(filter.FieldId))
            {
                query = query.Where(x => x.FieldId != null && x.FieldId == filter.FieldId);
            }
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(x => x.Title != null && x.Title.Contains(filter.Title));
            }
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(x => x.Description != null && x.Description.Contains(filter.Description));
            }

            return query;
        }

        private IQueryable<Query> SortByField(IQueryable<Query> result, QueryFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Reason:
                    return filterData.IsAscending
                        ? result.OrderBy(x =>
                              x.Reason.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize)
                        : result.OrderByDescending(x =>
                              x.Reason.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
                case AttributeNames.Status:
                    return filterData.IsAscending
                        ? result.OrderBy(x =>
                              x.Status.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize)
                        : result.OrderByDescending(x =>
                              x.Status.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                          .Skip((filterData.Page - 1) * filterData.PageSize)
                          .Take(filterData.PageSize);
            }
        }

        private IQueryable<Query> SortQueriesByField(IQueryable<Query> result, QueryFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.QueryReason:
                    return filterData.IsAscending
                        ? result.OrderBy(x =>
                              x.Reason.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                        : result.OrderByDescending(x =>
                              x.Reason.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault());
                case AttributeNames.QueryStatus:
                    return filterData.IsAscending
                        ? result.OrderBy(x =>
                              x.Status.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault())
                        : result.OrderByDescending(x =>
                              x.Status.ThesaurusEntry.Translations
                                  .Where(t => t.Language == filterData.ActiveLanguage)
                                  .Select(t => t.PreferredTerm)
                                  .FirstOrDefault());
                case AttributeNames.RaisedBy:
                    return filterData.IsAscending
                        ? result.OrderBy(x => x.CreatedBy != null ? x.CreatedBy.FirstName : "System")
                                .ThenBy(x => x.CreatedBy != null ? x.CreatedBy.LastName : "")
                        : result.OrderByDescending(x => x.CreatedBy != null ? x.CreatedBy.FirstName : "System")
                                .ThenByDescending(x => x.CreatedBy != null ? x.CreatedBy.LastName : "");
                case AttributeNames.QueryDescription:
                    return filterData.IsAscending
                        ? result.OrderBy(x => x.Description)
                        : result.OrderByDescending(x => x.Description);
                case AttributeNames.DaysOpen:
                    return filterData.IsAscending
                        ? result.OrderBy(q => EF.Functions.DateDiffDay(q.EntryDatetime, DateTime.UtcNow))
                        : result.OrderByDescending(q => EF.Functions.DateDiffDay(q.EntryDatetime, DateTime.UtcNow));

                case AttributeNames.DaysSinceLastChange:
                    return filterData.IsAscending
                        ? result.OrderBy(q => EF.Functions.DateDiffDay(q.LastUpdate ?? q.EntryDatetime, DateTime.UtcNow))
                        : result.OrderByDescending(q => EF.Functions.DateDiffDay(q.LastUpdate ?? q.EntryDatetime, DateTime.UtcNow));
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
            }
        }

        private async Task AddNewHistories(List<QueryHistory> historyList, int userId)
        {
            if (historyList == null || !historyList.Any())
                return;

            var newHistory = historyList
                .Where(h => h.QueryHistoryId == 0)
                .ToList();

            if (!newHistory.Any())
                return;

            foreach (var history in historyList) 
            {
                history.LastUpdateById = userId;
            }

            context.QueryHistories.AddRange(newHistory);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
