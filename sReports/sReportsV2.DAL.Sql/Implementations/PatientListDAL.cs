using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Entities;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PatientListDAL : BaseDisposalDAL, IPatientListDAL
    {
        public PatientListDAL(SReportsContext context) : base(context)
        {
        }

        public async Task<PatientList> GetByIdAsync(int id)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return await context.PatientLists
                .Include(x => x.PatientListPatientRelations)
                .Include(x => x.PatientListPersonnelRelations)
                .Include(x => x.PersonnelTeam)
                .Include(x => x.AttendingDoctor)
                .SingleOrDefaultAsync(x => x.PatientListId == id
                    && x.EntityStateCD != (int)EntityStateCode.Deleted && x.ActiveFrom <= now && x.ActiveTo >= now).ConfigureAwait(false);
        }

        public PatientList GetById(int id)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return context.PatientLists
                .Include(x => x.PatientListPatientRelations)
                .SingleOrDefault(x => x.PatientListId == id
                    && x.EntityStateCD != (int)EntityStateCode.Deleted && x.ActiveFrom <= now && x.ActiveTo >= now);
        }

        public async Task<PaginationData<PatientList>> GetAllByFilter(PatientListFilter filter)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            IQueryable<PatientList> query = context.PatientLists
                .WhereEntriesAreActive()
                .Include(x => x.PatientListPersonnelRelations);
            
            if(filter.PersonnelId > 0)
            {
                query = query.Where(x => x.PatientListPersonnelRelations.Any(y => y.PersonnelId == filter.PersonnelId && y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now));
            }
            if (filter.ListWithSelectedPatients.HasValue && filter.ListWithSelectedPatients.Value)
            {
                query = query.Where(x => x.ArePatientsSelected);
            }

            int count = await query.CountAsync().ConfigureAwait(false);

            query = ApplyOrderByAndPaging(filter, query);

            return new PaginationData<PatientList>(count, await query.ToListAsync().ConfigureAwait(false));
        }

        public async Task<PatientList> Create(PatientList patientList)
        {
            if(patientList!= null && patientList.PatientListId == 0)
            {
                context.PatientLists.Add(patientList);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            return patientList; 
        }

        public async Task<PatientList> Edit(PatientList patientList)
        {
            PatientList patientListFromDB = await GetByIdAsync(patientList.PatientListId);
            if (patientListFromDB != null)
            {
                patientListFromDB.Copy(patientList);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            return patientListFromDB;
        }

        public async Task Delete(int id)
        {
            PatientList patientListFromDB = await GetByIdAsync(id);
            if (patientListFromDB != null)
            {
                patientListFromDB.PatientListPersonnelRelations.ForEach(x => x.Delete());
                patientListFromDB.Delete();
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<PatientListPersonnelRelation> AddPersonnelRelation(PatientListPersonnelRelation patientListPersonnelRelation)
        {
            context.PatientListPersonnelRelations.Add(patientListPersonnelRelation);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return patientListPersonnelRelation;
        }

        public async Task<List<PatientListPersonnelRelation>> AddPersonnelRelations(List<PatientListPersonnelRelation> patientListPersonnelRelations)
        {
            context.PatientListPersonnelRelations.AddRange(patientListPersonnelRelations);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return patientListPersonnelRelations;
        }

        public async Task RemovePersonnelRelation(int patientListId, int personnelId)
        {
            List<PatientListPersonnelRelation> query = await context.PatientListPersonnelRelations
                .WhereEntriesAreActive()
                .Where(x => x.PatientListId == patientListId && personnelId == x.PersonnelId)
                .ToListAsync();

            query.ForEach(x =>
            {
                x.Delete();
            });

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PaginationData<AutoCompleteData>> GetTrialAutoCompleteNameAndCount(string term, EntityFilter filter)
        {
            IQueryable<PatientList> query = context.PatientLists
                .WhereEntriesAreActive()
                .Where(x => string.IsNullOrEmpty(term) || x.PatientListName.ToLower().Contains(term.ToLower()));

            int count = await query.CountAsync().ConfigureAwait(false);

            query = ApplyOrderByAndPaging(filter, query);

            List<AutoCompleteData> result = await query.Select(x => new AutoCompleteData() { Id = x.PatientListId.ToString(), Text = x.PatientListName })
                .ToListAsync().ConfigureAwait(false);

            return new PaginationData<AutoCompleteData>(count, result);
        }

        public async Task AddPatientRelation(PatientListPatientRelation patientListPatientRelation)
        {
            context.PatientListPatientRelations.Add(patientListPatientRelation);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemovePatientRelation(PatientListPatientRelation patientListPatientRelation)
        {
            List<PatientListPatientRelation> query = await context.PatientListPatientRelations
                .WhereEntriesAreActive()
                .Where(x => x.PatientListId == patientListPatientRelation.PatientListId && patientListPatientRelation.PatientId == x.PatientId)
                .ToListAsync();

            query.ForEach(x =>
            {
                x.Delete();
            });

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Dictionary<int, IEnumerable<int>>> GetListsContainingPatients(IEnumerable<int> patientIds)
        {
            return await context.PatientListPatientRelations
                .WhereEntriesAreActive()
                .Where(x => patientIds.Contains(x.PatientId))
                .GroupBy(x => x.PatientId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.PatientListId)).ConfigureAwait(false);
        }

        private IQueryable<PatientList> ApplyOrderByAndPaging(EntityFilter entityFilter, IQueryable<PatientList> query)
        {
            query = query.OrderBy(x => x.EntryDatetime)
                .Skip((entityFilter.Page - 1) * entityFilter.PageSize)
                .Take(entityFilter.PageSize);
            return query;
        }
    }
}
