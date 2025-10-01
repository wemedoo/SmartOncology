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
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

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

        public EpisodeOfCare GetById(int id)
        {
            return context.EpisodeOfCares
                .Include(x => x.Patient)
                .Include(x => x.Encounters)
                    .ThenInclude(x => x.Tasks)
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
                .Include(x => x.PersonnelTeam)
                .Include(x => x.DiagnosisCondition)
                .Include(x => x.DiagnosisCondition.Translations)
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
                context.EpisodeOfCares.Add(entity);
            }
            else 
            {
                EpisodeOfCare episodeOfCare = this.GetById(entity.EpisodeOfCareId);
                episodeOfCare.Copy(entity);
            }

            context.SaveChanges();

            return entity.EpisodeOfCareId;
        }

        public async Task<int> InsertOrUpdateAsync(EpisodeOfCare entity, UserData user)
        {
            if (entity.EpisodeOfCareId == 0)
            {
                context.EpisodeOfCares.Add(entity);
            }
            else
            {
                EpisodeOfCare episodeOfCare = await this.GetByIdAsync(entity.EpisodeOfCareId);
                episodeOfCare.Copy(entity);
                episodeOfCare.SetLastUpdate();
            }

            await context.SaveChangesAsync();

            return entity.EpisodeOfCareId;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.EpisodeOfCares.Any(x => x.DiagnosisConditionId == thesaurusId);
        }

        public int ReplaceThesaurus(ThesaurusMerge thesaurusMerge, UserData userData = null)
        {
            int i = 0;
            List<EpisodeOfCare> episodes = context.EpisodeOfCares.Where(x => x.DiagnosisConditionId == thesaurusMerge.OldThesaurus).ToList();
            foreach (EpisodeOfCare eoc in episodes)
            {
                eoc.ReplaceThesauruses(thesaurusMerge);
                i++;
            }

            context.SaveChanges();

            return i;
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
