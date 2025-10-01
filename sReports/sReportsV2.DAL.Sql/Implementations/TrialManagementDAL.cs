using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Helpers;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class TrialManagementDAL : ITrialManagementDAL
    {
        private readonly SReportsContext context;
        public TrialManagementDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task<ClinicalTrial> InsertOrUpdate(ClinicalTrial trial)
        {
            ClinicalTrial result;

            if (trial.ClinicalTrialId == 0)
            {
                context.ClinicalTrials.Add(trial);
                result = trial;
            }
            else
            {
                ClinicalTrial dbTrial = context.ClinicalTrials.FirstOrDefault(x => x.ClinicalTrialId == trial.ClinicalTrialId);
                dbTrial.Copy(trial);
                context.UpdateEntryMetadata(dbTrial);
                result = dbTrial;
            }

            await context.SaveChangesAsync();
            return result;
        }

        public async Task<ClinicalTrial> GetByProjectId(int projectId)
        {
            return await context.ClinicalTrials
                .WhereEntriesAreActive()
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);
        }

        public async Task<int> Delete(int id)
        {
            ClinicalTrial dbTrial = context.ClinicalTrials.FirstOrDefault(x => x.ClinicalTrialId == id);
            dbTrial.Delete();

            return await context.SaveChangesAsync();
        }

        public async Task<int> Archive(int id)
        {
            ClinicalTrial dbTrial = context.ClinicalTrials.FirstOrDefault(x => x.ClinicalTrialId == id);
            dbTrial.IsArchived = true;
            context.UpdateEntryMetadata(dbTrial);
            
            return await context.SaveChangesAsync();
        }

        public async Task<PaginationData<AutoCompleteData>> GetTrialAutoCompleteTitleAndCount(TrialManagementFilter filter)
        {
            IQueryable<ClinicalTrial> query = context.ClinicalTrials
                .WhereEntriesAreActive()
                .Where(x => string.IsNullOrEmpty(filter.ClinicalTrialTitle) || x.ClinicalTrialTitle.ToLower().Contains(filter.ClinicalTrialTitle.ToLower()));

            int count = await query.CountAsync().ConfigureAwait(false);

            query = GetTrialsOrderedByFilter(filter, query);
            query = ApplyPagingByFilter(filter, query);

            List<AutoCompleteData> result = await query.Select(x => new AutoCompleteData() { Id = x.ClinicalTrialId.ToString(), Text = x.ClinicalTrialTitle })
                .ToListAsync().ConfigureAwait(false);

            return new PaginationData<AutoCompleteData>(count, result);
        }

        public List<ClinicalTrial> GetlClinicalTrialsByName(string name)
        {
            return context.ClinicalTrials
                .WhereEntriesAreActive()
                .Where(clinicalTrial => !string.IsNullOrEmpty(clinicalTrial.ClinicalTrialTitle) && clinicalTrial.ClinicalTrialTitle.ToLower().Contains(name.ToLower()) 
                && clinicalTrial.IsArchived.HasValue && !clinicalTrial.IsArchived.Value
                ).ToList();
        }

        public List<ClinicalTrial> GetlClinicalTrialByIds(List<int> ids)
        {
            return context.ClinicalTrials
                .WhereEntriesAreActive()
                .Where(clinicalTrial => ids.Contains(clinicalTrial.ClinicalTrialId))
                .ToList();
        }

        private IQueryable<ClinicalTrial> GetTrialsOrderedByFilter(TrialManagementFilter filter, IQueryable<ClinicalTrial> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.ColumnName))
            {
                switch (filter.ColumnName)
                {
                    case AttributeNames.ClinicalTrialRecruitmentStatus:
                        if (filter.IsAscending)
                        {
                            query = query.OrderBy(x => x.ClinicalTrialRecruitmentStatus.ThesaurusEntry.Translations.FirstOrDefault(y => y.Language == LanguageConstants.EN).PreferredTerm);
                            break;
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.ClinicalTrialRecruitmentStatus.ThesaurusEntry.Translations.FirstOrDefault(y => y.Language == LanguageConstants.EN).PreferredTerm);
                            break;
                        }
                    default:
                        query = SortTableHelper.OrderByField(query, filter.ColumnName, filter.IsAscending);
                        break;
                }
                        
            }
            else
            {
                query = query.OrderBy(x => x.ClinicalTrialId);
            }
            return query;
        }

        private IQueryable<ClinicalTrial> ApplyPagingByFilter(TrialManagementFilter filter, IQueryable<ClinicalTrial> query)
        {
            return query
                .Skip(filter.GetHowManyElementsToSkip())
                .Take(filter.PageSize); 
        }

    }
}
