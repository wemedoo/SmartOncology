using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Enums;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PatientDAL : IPatientDAL
    {
        private readonly SReportsContext context;
        public PatientDAL(SReportsContext context)
        {
            this.context = context;
        }

        #region CRUD
        public Patient GetById(int id)
        {
            return context.Patients
                .Include(x => x.PatientAddresses)
                .Include(x => x.PatientIdentifiers)
                .Include(x => x.MultipleBirth)
                .Include(x => x.PatientTelecoms)
                .Include(x => x.Communications)
                .Include(x => x.PatientChemotherapyData)
                .Include(x => x.EpisodeOfCares)
                .Include(x => x.Encounters)
                    .ThenInclude(x => x.Tasks)
                .Include(x => x.PatientContacts)
                    .ThenInclude(pc => pc.PatientContactAddresses)
                .Include(x => x.PatientContacts)
                    .ThenInclude(pc => pc.PatientContactTelecoms)
                .Include(x => x.ProjectPatientRelations)
                    .ThenInclude(ppr => ppr.Project)
                .FirstOrDefault(x => x.PatientId == id);
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            var patient = await context.Patients
                .Include(x => x.PatientAddresses)
                .Include(x => x.PatientIdentifiers)
                .Include(x => x.MultipleBirth)
                .Include(x => x.PatientTelecoms)
                .Include(x => x.Communications)
                .Include(x => x.EpisodeOfCares)
                .Include(x => x.Encounters)
                    .ThenInclude(x => x.Tasks)
                .Include(x => x.PatientContacts)
                    .ThenInclude(pc => pc.PatientContactAddresses)
                .Include(x => x.PatientContacts)
                    .ThenInclude(pc => pc.PatientContactTelecoms)
                .Include(x => x.ProjectPatientRelations)
                    .ThenInclude(ppr => ppr.Project)
                .FirstOrDefaultAsync(x => x.PatientId == id);

            return patient;
        }

        public async Task<PatientContact> GetById(QueryEntityParam<PatientContact> queryParams)
        {
            return await context.PatientContacts
                .Include(c => c.PatientContactAddresses)
                .Include(c => c.PatientContactTelecoms)
                .FirstOrDefaultAsync(x => x.PatientContactId == queryParams.Id);
        }

        public async Task<PatientIdentifier> GetById(QueryEntityParam<PatientIdentifier> queryEntityParams)
        {
            return await context.PatientIdentifiers
                .FirstOrDefaultAsync(x => x.PatientIdentifierId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<PatientAddress> GetById(QueryEntityParam<PatientAddress> queryEntityParams)
        {
            return await context.PatientAddresses
                .FirstOrDefaultAsync(x => x.PatientAddressId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<PatientContactAddress> GetById(QueryEntityParam<PatientContactAddress> queryEntityParams)
        {
            return await context.PatientContactAddresses
                .FirstOrDefaultAsync(x => x.PatientContactAddressId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<PatientTelecom> GetById(QueryEntityParam<PatientTelecom> queryEntityParams)
        {
            return await context.PatientTelecoms
                .FirstOrDefaultAsync(x => x.PatientTelecomId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<PatientContactTelecom> GetById(QueryEntityParam<PatientContactTelecom> queryEntityParams)
        {
            return await context.PatientContactTelecoms
                .FirstOrDefaultAsync(x => x.PatientContactTelecomId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public Patient GetBy(Patient patient, PatientIdentifier mrnPatientIdentifier)
        {
            return context.Patients
                .WhereEntriesAreActive()
                .FirstOrDefault(p =>
                    p.NameGiven == patient.NameGiven
                    && p.NameFamily == patient.NameFamily
                    && p.GenderCD == patient.GenderCD
                    && p.BirthDate == patient.BirthDate
                    && p.PatientIdentifiers.Any(
                            pI => pI.IdentifierValue != null && pI.IdentifierValue == mrnPatientIdentifier.IdentifierValue
                            && pI.IdentifierTypeCD != null && pI.IdentifierTypeCD == mrnPatientIdentifier.IdentifierTypeCD
                            && pI.IdentifierPoolCD != null && pI.IdentifierPoolCD == mrnPatientIdentifier.IdentifierPoolCD
                        )
                    );
        }


        public Patient GetByIdentifier(PatientIdentifier identifier)
        {
            Patient result = null;
            if (IsO4MtIdentifier(identifier.IdentifierTypeCD))
            {
                result = GetById(int.Parse(identifier.IdentifierValue));
            }
            else
            {
                result = context.Patients
                    .Include(x => x.PatientIdentifiers)
                    .Include(x => x.MultipleBirth)
                    .Include(x => x.PatientTelecoms)
                    .Include(x => x.Communications)
                    .WhereEntriesAreActive()
                    .Where(x => x.PatientIdentifiers
                        .Any(y => y.IdentifierValue == identifier.IdentifierValue && y.IdentifierTypeCD == identifier.IdentifierTypeCD))
                    .FirstOrDefault();
            }
            return result;
        }

        public async Task<Patient> GetByIdentifierAsync(PatientIdentifier identifier)
        {
            Patient result = null;
            if (IsO4MtIdentifier(identifier.IdentifierTypeCD))
            {
                result = await GetByIdAsync(int.Parse(identifier.IdentifierValue));
            }
            else
            {
                result = await context.Patients
                    .Include(x => x.PatientIdentifiers)
                    .Include(x => x.MultipleBirth)
                    .Include(x => x.PatientTelecoms)
                    .Include(x => x.Communications)
                    .WhereEntriesAreActive()
                    .Where(x => x.PatientIdentifiers
                        .Any(y => y.IdentifierValue == identifier.IdentifierValue && y.IdentifierTypeCD == identifier.IdentifierTypeCD))
                    .FirstOrDefaultAsync();
            }
            return result;
        }

        public void InsertOrUpdate(Patient patient, PatientIdentifier defaultPatientIdentifier)
        {
            if (patient.PatientId == 0)
            {
                context.Patients.Add(patient);
                if (defaultPatientIdentifier != null && defaultPatientIdentifier.IdentifierTypeCD.HasValue)
                {
                    context.SaveChanges();
                    defaultPatientIdentifier.IdentifierValue = patient.PatientId.ToString();
                    patient.PatientIdentifiers.Add(defaultPatientIdentifier);
                }
            }
            else
            {
                patient.SetLastUpdate();
                int patientChemotherapyDataId = patient.PatientChemotherapyData != null ? patient.PatientChemotherapyData.PatientChemotherapyDataId : 0;
                if (patientChemotherapyDataId > 0)
                {
                    var patientChemotherapyData = context.PatientChemotherapyDatas.Find(patientChemotherapyDataId);
                    context.UpdateEntryMetadata(patientChemotherapyData, setRowVersion: false);
                }

            }

            context.SaveChanges();
        }

        public async Task InsertOrUpdate(PatientContact patientContact)
        {
            if (patientContact.PatientContactId == 0)
            {
                context.PatientContacts.Add(patientContact);
            }
            else
            {
                context.UpdateEntryMetadata(patientContact);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PatientIdentifier entry)
        {
            if (entry.PatientIdentifierId == 0)
            {
                context.PatientIdentifiers.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PatientAddress entry)
        {
            if (entry.PatientAddressId == 0)
            {
                context.PatientAddresses.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PatientContactAddress entry)
        {
            if (entry.PatientContactAddressId == 0)
            {
                context.PatientContactAddresses.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PatientTelecom entry)
        {
            if (entry.PatientTelecomId == 0)
            {
                context.PatientTelecoms.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PatientContactTelecom entry)
        {
            if (entry.PatientContactTelecomId == 0)
            {
                context.PatientContactTelecoms.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }


        public async Task Delete(Patient patient)
        {
            Patient fromDb = await this.GetByIdAsync(patient.PatientId).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, patient.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientContact entry)
        {
            PatientContact fromDb = await GetById(new QueryEntityParam<PatientContact>(entry.PatientContactId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientIdentifier entry)
        {
            PatientIdentifier fromDb = await GetById(new QueryEntityParam<PatientIdentifier>(entry.PatientIdentifierId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientAddress entry)
        {
            PatientAddress fromDb = await GetById(new QueryEntityParam<PatientAddress>(entry.PatientAddressId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientContactAddress entry)
        {
            PatientContactAddress fromDb = await GetById(new QueryEntityParam<PatientContactAddress>(entry.PatientContactAddressId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientTelecom entry)
        {
            PatientTelecom fromDb = await GetById(new QueryEntityParam<PatientTelecom>(entry.PatientTelecomId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PatientContactTelecom entry)
        {
            PatientContactTelecom fromDb = await GetById(new QueryEntityParam<PatientContactTelecom>(entry.PatientContactTelecomId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        #endregion /CRUD

        public bool ExistsPatientByIdentifier(PatientIdentifier identifier)
        {
            if (IsO4MtIdentifier(identifier.IdentifierTypeCD))
            {
                return ExistsPatietById(identifier.IdentifierValue);
            }

            return context.Patients
                .Include(x => x.PatientIdentifiers)
                .WhereEntriesAreActive()
                .Where(x => x.PatientIdentifiers
                    .Any(y => y.IdentifierValue == identifier.IdentifierValue && y.IdentifierTypeCD == identifier.IdentifierTypeCD && y.PatientIdentifierId != identifier.PatientIdentifierId))
                .Count() > 0;
        }
        public bool ExistsPatietById(string umcn)
        {
            return this.context.Patients
                .Include(x => x.PatientIdentifiers)
                .WhereEntriesAreActive()
                .Any(x => x.PatientIdentifiers.Any(y => y.IdentifierValue == umcn));
        }

        public bool ExistsPatient(int id)
        {
            return this.context.Patients
                .WhereEntriesAreActive()
                .Any(x => x.PatientId.Equals(id));
        }

        public List<Patient> GetAll(PatientFilter filter)
        {
            IQueryable<Patient> result = GetPatientFiltered(filter);

            if (filter.ApplyOrderByAndPagination)
            {
                result = ApplyOrderByAndPagination(filter, result);
            }

            return result.ToList();
        }

        public async Task<PaginationData<Patient>> GetAllAndCount(PatientFilter filter)
        {
            IQueryable<Patient> result = GetPatientFiltered(filter);

            int count = await result.CountAsync().ConfigureAwait(false);

            result = ApplyOrderByAndPagination(filter, result);

            return new PaginationData<Patient>(count, await result.ToListAsync().ConfigureAwait(false));
        }

        public int GetAllEntriesCount(PatientFilter filter)
        {
            return this.GetPatientFiltered(filter).Count();
        }

        public List<Patient> GetAllByIds(List<int> ids)
        {
            return context.Patients.Where(x => ids.Contains(x.PatientId)).ToList();
        }

        public IQueryable<Patient> GetPatientFiltered(PatientFilter filter)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            IQueryable<Patient> patientQuery = this.context.Patients
                .Include(x => x.PatientIdentifiers)
                .WhereEntriesAreActive()
                .Where(x => x.OrganizationId == filter.OrganizationId);

            if (filter.PatientList !=  null)
            {
                patientQuery = FilterByPatientList(filter, patientQuery);
            } 

            if (filter.BirthDate != null)
            {
                patientQuery = patientQuery.Where(x => x.BirthDate == filter.BirthDate);
            }

            if (filter.SimpleNameSearch)
            {
                if (!string.IsNullOrEmpty(filter.Given))
                {
                    patientQuery = patientQuery.Where(x => (x.NameFamily != null && x.NameFamily.Contains(filter.Given)) || (x.NameGiven != null && x.NameGiven.Contains(filter.Given)));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filter.Family))
                {
                    patientQuery = patientQuery.Where(x => x.NameFamily != null && x.NameFamily.Contains(filter.Family));
                }

                if (!string.IsNullOrEmpty(filter.Given))
                {
                    patientQuery = patientQuery.Where(x => x.NameGiven != null && x.NameGiven.Contains(filter.Given));
                }
            }
            

            if (!string.IsNullOrEmpty(filter.City))
            {
                patientQuery = patientQuery
                    .Where(patient => patient.PatientAddresses
                        .Any(a => a.EntityStateCD != (int)EntityStateCode.Deleted && a.ActiveFrom <= now && a.ActiveTo >= now && a.City != null && a.City.Contains(filter.City))
                        );
            }

            if (filter.CountryCD.HasValue)
            {
                patientQuery = patientQuery
                    .Where(patient => patient.PatientAddresses
                        .Any(a => a.EntityStateCD != (int)EntityStateCode.Deleted && a.ActiveFrom <= now && a.ActiveTo >= now && a.CountryCD == filter.CountryCD));
            }

            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                patientQuery = patientQuery
                    .Where(patient => patient.PatientAddresses
                        .Any(a => a.EntityStateCD != (int)EntityStateCode.Deleted && a.ActiveFrom <= now && a.ActiveTo >= now && a.PostalCode != null && a.PostalCode.Contains(filter.PostalCode)));
            }

            if (filter.EntryDatetime.HasValue)
            {
                patientQuery = patientQuery
                    .AsEnumerable()
                    .Where(x => x.EntryDatetime.ToLocalTime().Date == filter.EntryDatetime.Value.Date)
                    .AsQueryable();
            }

            patientQuery = FilterByIdentifier(patientQuery, filter.IdentifierType, filter.IdentifierValue);

            return patientQuery;
        }

        public List<Patient> GetPatientsFilteredByFirstAndLastName(PatientByNameSearchFilter patientSearchFilter)
        {
            var splitedSearchValue = patientSearchFilter.SearchValue.Split(Delimiters.delimiterCharacters);
            return context.Patients
                .WhereEntriesAreActive()
                .Where(x => x.OrganizationId == patientSearchFilter.OrganizationId)
                .AsEnumerable()
                .Select(x => new { Given = x.NameGiven != null ? x.NameGiven.ToLower() : "", Family = x.NameFamily != null ?x.NameFamily.ToLower() : "", x.PatientId, x.BirthDate, GivenName = x.NameGiven, FamilyName = x.NameFamily })
                .Where(x => splitedSearchValue.Length > 1 ? 
                                         x.Given.Contains(splitedSearchValue[0])
                                          && x.Family != null ? 
                                            x.Family.Contains(splitedSearchValue[1]) 
                                             : (x.Given.Contains(splitedSearchValue[1]) && x.Family != null) && x.Family.Contains(splitedSearchValue[0])
                                        : x.Family == null ? x.Given.Contains(patientSearchFilter.SearchValue)
                                                             : x.Given.Contains(patientSearchFilter.SearchValue) || x.Family.Contains(patientSearchFilter.SearchValue))
                .OrderBy(x => x.GivenName)
                .ThenBy(x => x.FamilyName)
                .Skip((patientSearchFilter.Page - 1) * patientSearchFilter.PageSize)
                .Take(patientSearchFilter.PageSize)
                .Select(x => new Patient { PatientId = x.PatientId, NameGiven = x.GivenName, NameFamily = x.FamilyName, BirthDate = x.BirthDate })
                .ToList();
        }

        private IQueryable<Patient> SortByField(IQueryable<Patient> result, PatientFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Gender:
                    GetGenderCodeData(filterData.Genders, Gender.Male.ToString(), out int maleCodeId, out string maleCodeValue);
                    GetGenderCodeData(filterData.Genders, Gender.Female.ToString(), out int femaleCodeId, out string femaleCodeValue);
                    GetGenderCodeData(filterData.Genders, Gender.Other.ToString(), out int otherCodeId, out string otherCodeValue);
                    GetGenderCodeData(filterData.Genders, Gender.Unknown.ToString(), out int unknownCodeId, out string unknownCodeValue);

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
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                            .Skip((filterData.Page - 1) * filterData.PageSize)
                            .Take(filterData.PageSize);
            }
        }

        private void GetGenderCodeData(Dictionary<string, Tuple<int, string>> genders, string gender, out int genderCodeId, out string genderCodeValue)
        {
            genderCodeId = 0;
            genderCodeValue = string.Empty;
            if (genders.TryGetValue(gender, out Tuple<int, string> genderTupple))
            {
                genderCodeId = genderTupple.Item1;
                genderCodeValue = genderTupple.Item2;
            }
        }

        private bool IsO4MtIdentifier(int? identifierTypeCD)
        {
            return identifierTypeCD == -1;
        }

        private IQueryable<Patient> ApplyOrderByAndPagination(PatientFilter filter, IQueryable<Patient> query)
        {
            if (filter.ColumnName != null)
                query = SortByField(query, filter);
            else
                query = query.OrderByDescending(x => x.PatientId)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);
            return query;
        }

        private IQueryable<Patient> FilterByPatientList(PatientFilter filter, IQueryable<Patient> patientQuery)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();

            //if (filter.PatientList.ShowOnlyDischargedPatient)
            //{
            //    patientQuery = patientQuery.WhereEntriesAreInactive();
            //}
            //else if (!filter.PatientList.IncludeDischargedPatient)
            //{
            //    patientQuery = patientQuery.WhereEntriesAreActive();
            //}

            if (filter.PatientList.ExcludeDeceasedPatient)
            {
                patientQuery = patientQuery.Where(p => p.Deceased == null || p.Deceased == false);
            }

            if (filter.PatientList.ArePatientsSelected)
            {
                IEnumerable<int> patientIdsSelectedInList = filter.PatientList.PatientListPatientRelations
                    .Where(x => x.IsActive())
                    .Select(y => y.PatientId);
                patientQuery = patientQuery
                    .Where(x => patientIdsSelectedInList.Contains(x.PatientId));
            }
            else
            {
                if (filter.PatientList.EpisodeOfCareTypeCD != null)
                {
                    patientQuery = patientQuery.Where(x =>
                        x.EpisodeOfCares.Any(y => y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now && y.TypeCD == filter.PatientList.EpisodeOfCareTypeCD));
                }
                if (filter.PatientList.EncounterTypeCD != null)
                {
                    patientQuery = patientQuery.Where(x =>
                        x.Encounters.Any(e => e.EntityStateCD != (int)EntityStateCode.Deleted && e.ActiveFrom <= now && e.ActiveTo >= now && e.TypeCD != null && e.TypeCD == filter.PatientList.EncounterTypeCD));
                }
                if (filter.PatientList.EncounterStatusCD != null)
                {
                    patientQuery = patientQuery.Where(x =>
                        x.Encounters.Any(e => e.EntityStateCD != (int)EntityStateCode.Deleted && e.ActiveFrom <= now && e.ActiveTo >= now && e.TypeCD != null && e.StatusCD == filter.PatientList.EncounterStatusCD));
                }
                if (filter.PatientList.PersonnelTeamId != null)
                {
                    patientQuery = patientQuery
                        .Where(x => x.EpisodeOfCares.Any(y => y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now
                        && y.PersonnelTeamId != null && y.PersonnelTeamId == filter.PatientList.PersonnelTeamId.Value));
                }
                if (filter.PatientList.AttendingDoctorId != null && filter.AttendingDoctorCD != null)
                {
                    patientQuery = patientQuery
                        .Include(x => x.Encounters)
                        .ThenInclude(y => y.PersonnelEncounterRelations)
                        .Where(x => x.Encounters
                            .Any(e => e.EntityStateCD != (int)EntityStateCode.Deleted && e.ActiveFrom <= now && e.ActiveTo >= now && e.PersonnelEncounterRelations
                                .Any(per => per.PersonnelId == filter.PatientList.AttendingDoctorId && per.RelationTypeCD == filter.AttendingDoctorCD
                                    && per.EntityStateCD != (int)EntityStateCode.Deleted && per.ActiveFrom <= now && per.ActiveTo >= now)));
                }
                if (filter.PatientList.AdmissionDate != null)
                {
                    patientQuery = patientQuery.Where(x => x.Encounters.Any(e => e.EntityStateCD != (int)EntityStateCode.Deleted && e.ActiveFrom <= now && e.ActiveTo >= now
                        && e.AdmissionDate >= filter.PatientList.AdmissionDate));
                }
                if (filter.PatientList.DischargeDate != null)
                {
                    patientQuery = patientQuery.Where(x => x.Encounters.Any(e => e.EntityStateCD != (int)EntityStateCode.Deleted && e.ActiveFrom <= now && e.ActiveTo >= now
                        && e.DischargeDate <= filter.PatientList.DischargeDate));
                }
            }

            return patientQuery;
        }

        private IQueryable<Patient> FilterByIdentifier(IQueryable<Patient> patientEntities, int? identifierType, string value)
        {
            IQueryable<Patient> result = patientEntities;
            if (identifierType.HasValue && !string.IsNullOrEmpty(value))
            {
                if (IsO4MtIdentifier(identifierType))
                {
                    if (int.TryParse(value, out int O4PatientId))
                    {
                        result = patientEntities.Where(x => x.PatientId.Equals(O4PatientId));
                    }
                }
                else
                {
                    result = patientEntities.Where(x => x.PatientIdentifiers.Any(y => y.IdentifierTypeCD == identifierType && y.IdentifierValue == value));
                }
            }

            return result;
        }
    }
}
