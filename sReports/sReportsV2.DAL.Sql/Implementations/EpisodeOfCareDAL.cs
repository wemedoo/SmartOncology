using sReportsV2.Common.Entities.User;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class EpisodeOfCareDAL : IEpisodeOfCareDAL
    {
        private readonly SReportsContext context;
        public EpisodeOfCareDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task DeleteAsync(int eocId)
        {
            EpisodeOfCare fromDb = await GetByIdAsync(eocId);
            if (fromDb != null)
            {
                fromDb.Delete();
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<EpisodeOfCare>> GetAllAsync(EpisodeOfCareFilter filter)
        {
            return await GetFiltered(filter)
                 .OrderByDescending(x => x.EntryDatetime)
                 .Skip((filter.Page - 1) * filter.PageSize)
                 .Take(filter.PageSize)
                 .ToListAsync();
        }

        public async Task<long> GetAllEntriesCountAsync(EpisodeOfCareFilter filter)
        {
            return await GetFiltered(filter).CountAsync();
        }

        public EpisodeOfCare GetById(int id)
        {
            return context.EpisodeOfCares
                .Include(x => x.Patient)
                .Include(x => x.Encounters)
                    .ThenInclude(x => x.Tasks)
                .Include(x => x.WorkflowHistory)
                .Include(x => x.PersonnelTeam)
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.EpisodeOfCareId == id);
        }

        public async Task<EpisodeOfCare> GetByIdAsync(int id)
        {
            return await context.EpisodeOfCares
                .Include(x => x.Patient)
                .Include(x => x.Encounters)
                    .ThenInclude(x => x.Tasks)
                .Include(x => x.WorkflowHistory)
                .Include(x => x.PersonnelTeam)
                .WhereEntriesAreActive()
                .FirstOrDefaultAsync(x => x.EpisodeOfCareId == id).ConfigureAwait(false);
        }

        public List<EpisodeOfCare> GetByPatientId(int patientId)
        {
            return this.context.EpisodeOfCares
                .WhereEntriesAreActive()
                .Where(x => x.PatientId == patientId)
                .ToList();
        }

        public int InsertOrUpdate(EpisodeOfCare entity, UserData user)
        {
            if (entity.EpisodeOfCareId == 0)
            {
                entity.SetWorkflow(user);
                context.EpisodeOfCares.Add(entity);
            }
            else 
            {
                EpisodeOfCare episodeOfCare = this.GetById(entity.EpisodeOfCareId);
                episodeOfCare.Copy(entity);
                episodeOfCare.SetWorkflow(user);
            }

            context.SaveChanges();

            return entity.EpisodeOfCareId;
        }

        public async Task<int> InsertOrUpdateAsync(EpisodeOfCare entity, UserData user)
        {
            if (entity.EpisodeOfCareId == 0)
            {
                entity.SetWorkflow(user);
                context.EpisodeOfCares.Add(entity);
            }
            else
            {
                EpisodeOfCare episodeOfCare = await this.GetByIdAsync(entity.EpisodeOfCareId);
                episodeOfCare.Copy(entity);
                episodeOfCare.SetWorkflow(user);
                episodeOfCare.SetLastUpdate();
            }

            await context.SaveChangesAsync();

            return entity.EpisodeOfCareId;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.EpisodeOfCares.Any(x => x.TypeCD == thesaurusId || x.DiagnosisRole == thesaurusId);
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            List<EpisodeOfCare> episodes = context.EpisodeOfCares.Where(x => x.DiagnosisRole == oldThesaurus).ToList();
            foreach (EpisodeOfCare eoc in episodes)
            {
                eoc.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }

            context.SaveChanges();
        }

        private IQueryable<EpisodeOfCare> GetFiltered(EpisodeOfCareFilter filter)
        {
            IQueryable<EpisodeOfCare> filteredData = context.EpisodeOfCares
                .Include(x => x.WorkflowHistory)
                .WhereEntriesAreActive();
            if (!string.IsNullOrEmpty(filter.Description))
            {
                filteredData = filteredData.Where(x => x.Description.Contains(filter.Description));
            }

            if (filter.PeriodStartDate != null)
            {
                DateTime beginStartDate = filter.PeriodStartDate ?? DateTime.Now;
                DateTime endStartDate = beginStartDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.Start >= beginStartDate && x.Period.Start < endStartDate);
            }

            if (filter.PeriodEndDate != null)
            {
                DateTime beginEndDate = filter.PeriodEndDate ?? DateTime.Now;
                DateTime endEndDate = beginEndDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.End >= beginEndDate && x.Period.End < endEndDate);
            }

            if (filter.TypeCD != 0)
            {
                filteredData = filteredData.Where(x => x.TypeCD.Equals(filter.TypeCD));
            }

            if (filter.FilterByIdentifier)
            {
                filteredData = filteredData.Where(x => x.PatientId.Equals(filter.PatientId));
            }

            filteredData = filteredData.Where(x => x.OrganizationId == filter.OrganizationId);

            return filteredData;
        }

        public async Task<List<EpisodeOfCare>> GetByPatientIdFilteredAsync(EpisodeOfCareFilter filter)
        {
            var result = GetCodeSetFilteredAsync(filter);

            return await result.ConfigureAwait(false);
        }

        private async Task<List<EpisodeOfCare>> GetCodeSetFilteredAsync(EpisodeOfCareFilter filter)
        {
            IQueryable<EpisodeOfCare> codeSetQuery = this.context.EpisodeOfCares
                .Include(x => x.PersonnelTeam)
                .WhereEntriesAreActive()
                .Where(x => x.PatientId == filter.PatientId);

            if (filter.StatusCD != 0)
            {
                codeSetQuery = codeSetQuery.Where(x => x.StatusCD == filter.StatusCD);
            }
            if (filter.TypeCD != 0)
            {
                codeSetQuery = codeSetQuery.Where(x => x.TypeCD == filter.TypeCD);
            }

            return await codeSetQuery.ToListAsync().ConfigureAwait(false);
        }
    }
}
