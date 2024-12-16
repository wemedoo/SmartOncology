using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.SqlDomain.Helpers;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class EncounterDAL : IEncounterDAL
    {
        private readonly SReportsContext context;
        public EncounterDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task DeleteAsync(int encounterId)
        {
            Encounter fromDb = await context.Encounters
                .Include(x => x.Tasks).FirstOrDefaultAsync(x => x.EncounterId == encounterId);
            if (fromDb != null)
            {
                fromDb.Delete();
                await context.SaveChangesAsync();
            }
        }

        public List<Encounter> GetAllByEocId(int eocId)
        {
            return context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId.Equals(eocId))
                .OrderByDescending(x => x.AdmissionDate)
                .ToList();
        }

        public async Task<List<Encounter>> GetAllByEocIdAsync(int eocId)
        {
            return await context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId.Equals(eocId))
                .OrderByDescending(x => x.AdmissionDate)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Encounter> GetByIdAsync(int id)
        {
            return await context.Encounters
                .Include(x => x.PersonnelEncounterRelations)
                .Include("PersonnelEncounterRelations.Personnel")
                .Include("PersonnelEncounterRelations.Personnel.PersonnelIdentifiers")
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.EncounterId == id)
                .ConfigureAwait(false);
        }

        public Encounter GetById(int id)
        {
            return context.Encounters
                .Include(x => x.PersonnelEncounterRelations)
                .Include("PersonnelEncounterRelations.Personnel")
                .Include("PersonnelEncounterRelations.Personnel.PersonnelIdentifiers")
                .FirstOrDefault(x => x.EncounterId == id);
        }

        public int GetEncounterTypeByEncounterId(int encounterId)
        {
            var encounter = context.Encounters.Where(x => x.EncounterId == encounterId).Select(x => x.TypeCD).SingleOrDefault();
            return encounter.GetValueOrDefault();
        }

        public int InsertOrUpdate(Encounter encounter)
        {
            if (encounter.EncounterId == 0)
            {
                context.Encounters.Add(encounter);
            }
            else
            {
                context.UpdateEntryMetadata(encounter);
            }

            context.SaveChanges();

            return encounter.EncounterId;
        }

        public async Task<int> InsertOrUpdateAsync(Encounter encounter)
        {
            if (encounter.EncounterId == 0)
            {
                context.Encounters.Add(encounter);
            }
            else
            {
                context.UpdateEntryMetadata(encounter);
            }

            await context.SaveChangesAsync();

            return encounter.EncounterId;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.Encounters
                .Any(x => x.StatusCD == thesaurusId || x.ClassCD == thesaurusId || x.TypeCD == thesaurusId || x.ServiceTypeCD == thesaurusId);
        }

        public async Task<List<Encounter>> GetByEOCIdAsync(int eocId)
        {
            return await context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId == eocId)
                .ToListAsync().ConfigureAwait(false);
        }

        public List<Encounter> GetByEncounterTypeAndEpisodeOfCareId(int encounterTypeId, int episodeOfCareId)
        {
            return context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId == episodeOfCareId 
                && x.TypeCD == encounterTypeId)
                .ToList();
        }

        public int CountAllEncounters(int episodeOfCareId)
        {
            return context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId == episodeOfCareId)
                .Count();
        }

        public async Task<int> CountAllEncountersAsync(int episodeOfCareId)
        {
            return await context.Encounters
                .WhereEntriesAreActive()
                .Where(x => x.EpisodeOfCareId == episodeOfCareId)
                .CountAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> GetAllEntriesCountAsync(EncounterFilter filter)
        {
            return await GetEncounterFiltered(filter).CountAsync()
                .ConfigureAwait(false);
        }

        public async Task<List<EncounterView>> GetAllAsync(EncounterFilter filter)
        {
            IQueryable<EncounterView> result = GetEncounterFiltered(filter);

            if (filter.ReloadEncounterFromPatient)
                result = GetAllEncountersForPatient(filter, result);
            else
                result = GetAllEncounters(filter, result);

            return await result.ToListAsync().ConfigureAwait(false);
        }

        private IQueryable<EncounterView> GetAllEncounters(EncounterFilter filter, IQueryable<EncounterView> result) 
        {
            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderByDescending(x => x.PatientId)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

            return result;
        }

        private IQueryable<EncounterView> GetAllEncountersForPatient(EncounterFilter filter, IQueryable<EncounterView> result)
        {
            if (filter.ColumnName != null)
                result = SortByField(result, filter, true);
            else
                result = result.OrderByDescending(x => x.PatientId);

            return result;
        }

        private IQueryable<EncounterView> GetEncounterFiltered(EncounterFilter filter)
        {
            IQueryable<EncounterView> encounterQuery = context.EncounterViews
                .WhereEntriesAreActive();

            if (filter.EncounterId != 0)
            {
                encounterQuery = encounterQuery.Where(x => x.EncounterId == filter.EncounterId);
            }
            if (filter.PatientId.HasValue)
            {
                encounterQuery = encounterQuery.Where(x => x.PatientId == filter.PatientId);
            }
            if (!string.IsNullOrEmpty(filter.Family))
            {
                encounterQuery = encounterQuery.Where(x => x.NameFamily != null && x.NameFamily.Contains(filter.Family));
            }
            if (!string.IsNullOrEmpty(filter.Given))
            {
                encounterQuery = encounterQuery.Where(x => x.NameGiven != null && x.NameGiven.Contains(filter.Given));
            }
            if (filter.Gender.HasValue)
            {
                encounterQuery = encounterQuery.Where(x => x.GenderCD == filter.Gender);
            }
            if (filter.BirthDate != null)
            {
                encounterQuery = encounterQuery.Where(x => x.BirthDate == filter.BirthDate);
            }
            if (filter.EpisodeOfCareTypeCD.HasValue)
            {
                encounterQuery = encounterQuery.Where(x => x.EpisodeOfCareTypeCD == filter.EpisodeOfCareTypeCD);
            }
            if (filter.TypeCD.HasValue)
            {
                encounterQuery = encounterQuery.Where(x => x.TypeCD == filter.TypeCD);
            }
            if (filter.StatusCD.HasValue)
            {
                encounterQuery = encounterQuery.Where(x => x.StatusCD == filter.StatusCD);
            }
            if (filter.AdmissionDate != null)
            {
                DateTimeOffset startDate = filter.AdmissionDate.Value.Date;
                DateTimeOffset endDate = startDate.AddDays(1);

                encounterQuery = encounterQuery.Where(x => x.AdmissionDate >= startDate && x.AdmissionDate < endDate);
            }
            if (filter.DischargeDate != null)
            {
                DateTimeOffset startDate = filter.DischargeDate.Value.Date;
                DateTimeOffset endDate = startDate.AddDays(1);

                encounterQuery = encounterQuery.Where(x => x.DischargeDate >= startDate && x.DischargeDate < endDate);
            }

            return encounterQuery;
        }

        private IQueryable<EncounterView> SortByField(IQueryable<EncounterView> result, EncounterFilter filterData, bool reloadEncounterFromPatient = false)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Gender:
                    GetCodeData(filterData.Genders, Gender.Male.ToString(), out int maleCodeId, out string maleCodeValue);
                    GetCodeData(filterData.Genders, Gender.Female.ToString(), out int femaleCodeId, out string femaleCodeValue);
                    GetCodeData(filterData.Genders, Gender.Other.ToString(), out int otherCodeId, out string otherCodeValue);
                    GetCodeData(filterData.Genders, Gender.Unknown.ToString(), out int unknownCodeId, out string unknownCodeValue);

                    if (filterData.IsAscending)
                        return result.OrderBy(x =>
                                x.GenderCD == maleCodeId ? maleCodeValue :
                                x.GenderCD == femaleCodeId ? femaleCodeValue :
                                x.GenderCD == otherCodeId ? otherCodeValue : unknownCodeValue
                            )
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x =>
                                x.GenderCD == maleCodeId ? maleCodeValue :
                                x.GenderCD == femaleCodeId ? femaleCodeValue :
                                x.GenderCD == otherCodeId ? otherCodeValue : unknownCodeValue
                            )
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
                case AttributeNames.PatientNameGiven:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.NameGiven);
                    else
                        return result.OrderByDescending(x => x.NameGiven);
                case AttributeNames.Status:
                    GetCodeData(filterData.Statuses, "Planned", out int plannedCodeId, out string plannedCodeValue);
                    GetCodeData(filterData.Statuses, "Arrived", out int arrivedCodeId, out string arrivedCodeValue);
                    GetCodeData(filterData.Statuses, "Triaged", out int triagedCodeId, out string triagedCodeValue);
                    GetCodeData(filterData.Statuses, "In progress", out int inProgressCodeId, out string inProgressCodeValue);
                    GetCodeData(filterData.Statuses, "Onleave", out int onleaveCodeId, out string onleaveCodeValue);
                    GetCodeData(filterData.Statuses, "Finished", out int finishedCodeId, out string finishedCodeValue);
                    GetCodeData(filterData.Statuses, "Cancelled", out int cancelledCodeId, out string cancelledCodeValue);

                    if (reloadEncounterFromPatient)
                        if (filterData.IsAscending)
                            return result.OrderBy(x =>
                                 x.StatusCD == plannedCodeId ? plannedCodeValue :
                                 x.StatusCD == arrivedCodeId ? arrivedCodeValue :
                                 x.StatusCD == triagedCodeId ? triagedCodeValue :
                                 x.StatusCD == inProgressCodeId ? inProgressCodeValue :
                                 x.StatusCD == onleaveCodeId ? finishedCodeValue :
                                 x.StatusCD == finishedCodeId ? finishedCodeValue : cancelledCodeValue
                             );
                        else
                            return result.OrderByDescending(x =>
                                 x.StatusCD == plannedCodeId ? plannedCodeValue :
                                 x.StatusCD == arrivedCodeId ? arrivedCodeValue :
                                 x.StatusCD == triagedCodeId ? triagedCodeValue :
                                 x.StatusCD == inProgressCodeId ? inProgressCodeValue :
                                 x.StatusCD == onleaveCodeId ? finishedCodeValue :
                                 x.StatusCD == finishedCodeId ? finishedCodeValue : cancelledCodeValue
                             );
                    else
                         if (filterData.IsAscending)
                            return result.OrderBy(x =>
                                 x.StatusCD == plannedCodeId ? plannedCodeValue :
                                 x.StatusCD == arrivedCodeId ? arrivedCodeValue :
                                 x.StatusCD == triagedCodeId ? triagedCodeValue :
                                 x.StatusCD == inProgressCodeId ? inProgressCodeValue :
                                 x.StatusCD == onleaveCodeId ? finishedCodeValue :
                                 x.StatusCD == finishedCodeId ? finishedCodeValue : cancelledCodeValue
                             )
                             .Skip((filterData.Page - 1) * filterData.PageSize)
                             .Take(filterData.PageSize);
                        else
                            return result.OrderByDescending(x =>
                                 x.StatusCD == plannedCodeId ? plannedCodeValue :
                                 x.StatusCD == arrivedCodeId ? arrivedCodeValue :
                                 x.StatusCD == triagedCodeId ? triagedCodeValue :
                                 x.StatusCD == inProgressCodeId ? inProgressCodeValue :
                                 x.StatusCD == onleaveCodeId ? finishedCodeValue :
                                 x.StatusCD == finishedCodeId ? finishedCodeValue : cancelledCodeValue
                             )
                             .Skip((filterData.Page - 1) * filterData.PageSize)
                             .Take(filterData.PageSize);
                case AttributeNames.PatientNameFamily:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.NameFamily);
                    else
                        return result.OrderByDescending(x => x.NameFamily);
                case AttributeNames.PatientBirthDate:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.BirthDate);
                    else
                        return result.OrderByDescending(x => x.BirthDate);
                case AttributeNames.PatientGender:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.GenderCD);
                    else
                        return result.OrderByDescending(x => x.GenderCD);
                default:
                    if (reloadEncounterFromPatient)
                        return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
                    else
                        return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                        .Skip((filterData.Page - 1) * filterData.PageSize)
                        .Take(filterData.PageSize);
            }
        }

        private void GetCodeData(Dictionary<string, Tuple<int, string>> dictionary, string code, out int codeId, out string codeValue)
        {
            codeId = 0;
            codeValue = string.Empty;
            if (dictionary.TryGetValue(code, out Tuple<int, string> genderTupple))
            {
                codeId = genderTupple.Item1;
                codeValue = genderTupple.Item2;
            }
        }

        public List<Encounter> GetAll()
        {
            return context.Encounters
                .WhereEntriesAreActive()
                .ToList();
        }

        public List<Encounter> GetAllInactive()
        {
            return context.Encounters
                .WhereEntriesAreInactive()
                .ToList();
        }
    }
}
