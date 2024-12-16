using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Constants;
using System.Threading.Tasks;
using System;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.Common.Entities;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class PersonnelDAL : IPersonnelDAL
    {
        private readonly SReportsContext context;
        public PersonnelDAL(SReportsContext context)
        {
            this.context = context;
        }

        #region CRUD

        public Personnel GetById(int id)
        {
            return context.Personnel
                .Include(x => x.PersonnelAdresses)
                .Include(x => x.Organizations)
                .Include("Organizations.Organization")
                .Include(x => x.PersonnelConfig)
                .Include(x => x.PersonnelAcademicPositions)
                .Include(x => x.PersonnelIdentifiers)
                .Include(x => x.PersonnelOccupation)
                .Include(x => x.PersonnelPositions)
                .FirstOrDefault(x => x.PersonnelId == id);
        }

        public async Task<PersonnelIdentifier> GetById(QueryEntityParam<PersonnelIdentifier> queryEntityParams)
        {
            return await context.PersonnelIdentifiers
                .FirstOrDefaultAsync(x => x.PersonnelIdentifierId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<PersonnelAddress> GetById(QueryEntityParam<PersonnelAddress> queryEntityParams)
        {
            return await context.PersonnelAddresses
                .FirstOrDefaultAsync(x => x.PersonnelAddressId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public Personnel GetByEmail(string email)
        {
            return context.Personnel
                .Include("Organizations")
                .Include("Organizations.Organization")
                .Include(x => x.PersonnelConfig)
                .Include(x => x.PersonnelPositions)
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.Email.Equals(email));
        }

        public Personnel GetByUsername(string username)
        {
            return context.Personnel
                .Include("Organizations")
                .Include("Organizations.Organization")
                .Include(x => x.PersonnelConfig)
                .Include(x => x.PersonnelPositions)
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.Username.Equals(username));
        }

        public void InsertOrUpdate(Personnel user)
        {
            if (user.PersonnelId == 0)
            {
                user.PersonnelOccupation = null;
                context.Personnel.Add(user);
            }
            else 
            {
                user.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public async Task InsertOrUpdate(PersonnelIdentifier entry)
        {
            if (entry.PersonnelIdentifierId == 0)
            {
                context.PersonnelIdentifiers.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(PersonnelAddress entry)
        {
            if (entry.PersonnelAddressId == 0)
            {
                context.PersonnelAddresses.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Delete(PersonnelIdentifier entry)
        {
            PersonnelIdentifier fromDb = await GetById(new QueryEntityParam<PersonnelIdentifier>(entry.PersonnelIdentifierId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(PersonnelAddress entry)
        {
            PersonnelAddress fromDb = await GetById(new QueryEntityParam<PersonnelAddress>(entry.PersonnelAddressId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void Delete(int id)
        {
            var fromDb = GetById(id);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        #endregion /CRUD

        public bool IsValidUser(string username, string password)
        {
            return context.Personnel
                .WhereEntriesAreActive()
                .Any(x => x.Username.Equals(username) && x.Password.Equals(password));
        }

        public bool IsUserStillValid(int id, int? activeStateCD)
        {
            Personnel user = this.GetById(id);
            return user != null && user.Organizations.Find(x => x.OrganizationId == user.PersonnelConfig.ActiveOrganizationId)?.StateCD == activeStateCD;
        }

        public int CountAll()
        {
            return this.context.Personnel.Count();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public List<Personnel> GetAllByIds(List<int> ids)
        {
            return context.Personnel.Where(x => ids.Contains(x.PersonnelId)).ToList();
        }

        public async Task<List<Personnel>> GetAllByIdsAsync(List<int> ids)
        {
            return await context.Personnel.Where(x => ids.Contains(x.PersonnelId)).ToListAsync().ConfigureAwait(false);
        }

        public long GetAllCount()
        {
            return context.Personnel
                .WhereEntriesAreActive()
                .Count();
        }

        public List<Personnel> GetAllByOrganizationIds(List<int> organizationIds)
        {
            return context.Personnel
                .Include(x => x.Organizations)
                .Include(x => x.PersonnelConfig)
                .Where(x => x.Organizations.Select(o => o.PersonnelOrganizationId).Any(i => organizationIds.Contains(i))).ToList();
        }

        public bool IsUsernameValid(string username, int? userId)
        {
            return !context.Personnel
                .WhereEntriesAreActive()
                .Any(x => x.Username == username && x.PersonnelId != userId);
        }

        public bool IsEmailValid(string email, int? userId)
        {
            return !context.Personnel
                .WhereEntriesAreActive()
                .Any(x => x.Email == email && x.PersonnelId != userId);
        }

        public bool UserExist(int id)
        {
            return context.Personnel
                .WhereEntriesAreActive()
                .Any(x => x.PersonnelId == id);
        }

        public void SetState(int id, int? state, int organizationId, int? archivedUserStateCD)
        {
            if (state == archivedUserStateCD)
            {
                IncrementOrganizationsUserCount(new List<int>() { organizationId }, -1);
            }
            Personnel user = this.GetById(id);
            user.Organizations.Find(x => x.OrganizationId == organizationId).StateCD = state;
            this.InsertOrUpdate(user);
        }

        public void UpdateOrganizationsUserCounts(Personnel user, Personnel dbUser)
        {
            IncrementOrganizationsUserCount(user.GetOrganizationRefs().Where(x => !dbUser.GetOrganizationRefs().Contains(x)).ToList(), 1);
        }

        public void UpdateUsersCountForAllOrganization(int? archivedUserStateCD)
        {
            var organizationIds = GetOrganizationIds();
            Dictionary<int, int> numOfUsersPerOrganization = MapNumOfUsersPerOrganization(organizationIds, archivedUserStateCD);

            foreach (var organization in context.Organizations.WhereEntriesAreActive())
            {
                organization.NumOfUsers = numOfUsersPerOrganization[organization.OrganizationId];
            }

            context.SaveChanges();
        }

        public void UpdatePassword(Personnel user, string newPassword)
        {
            Personnel userDb = GetById(user.PersonnelId);
            userDb.Password = newPassword;
            userDb.SetLastUpdate();
            context.SaveChanges();
        }

        public List<Personnel> GetUsersForCommentTag(string searchWord)
        {
            return context.Personnel
                .Join(
                      context.PersonnelPositionPermissionViews,
                      personnel => personnel.PersonnelId,
                      personnelPositionPermissions => personnelPositionPermissions.PersonnelId,
                      (personnel, personnelPositionPermissions) => new { personnel, personnelPositionPermissions })
                   .Where(x => x.personnel.FirstName.Contains(searchWord) && ((x.personnelPositionPermissions.ModuleName == ModuleNames.Designer && x.personnelPositionPermissions.PermissionName == PermissionNames.View) || (x.personnelPositionPermissions.ModuleName == ModuleNames.Designer && x.personnelPositionPermissions.PermissionName == PermissionNames.ViewComments)))
                   .Select(x => x.personnel)
                   .GroupBy(x => x.PersonnelId)
                   .Select(x => x.FirstOrDefault())
                   .ToList();
        }

        public IQueryable<AutoCompleteUserData> GetUsersFilteredByName(string searchValue, int organizationId, int? archivedUserStateCD)
        {
            var personnelQuery = context.Personnel
                .WhereEntriesAreActive()
                .Where(x =>
                        (searchValue.Contains(" ") && (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(searchValue.ToLower()))
                        || (searchValue.Contains(" ") && (x.LastName.ToLower() + " " + x.FirstName.ToLower()).Contains(searchValue.ToLower()))
                        || (!searchValue.Contains(" ") && (x.FirstName.ToLower().Contains(searchValue.ToLower()) || x.LastName.ToLower().Contains(searchValue.ToLower())))
                );

            if (organizationId != 0)
            {
                personnelQuery = personnelQuery.Where(x => x.Organizations.Any(y => y.OrganizationId == organizationId && y.StateCD != archivedUserStateCD));
            }

            return personnelQuery
                .Select(x => new AutoCompleteUserData { PersonnelId = x.PersonnelId, FirstName = x.FirstName, LastName = x.LastName, UserName = x.Username });
        }

        public List<PersonnelView> GetAll(PersonnelFilter userFilter, int? archivedUserStateCD)
        {
            IQueryable<PersonnelView> result = GetPersonnelFiltered(userFilter, archivedUserStateCD);

            if (userFilter.ColumnName != null)
            {
                result = SortTableHelper.OrderByField(result, userFilter.ColumnName, userFilter.IsAscending)
                     .Skip((userFilter.Page - 1) * userFilter.PageSize)
                     .Take(userFilter.PageSize);
            }
            else
            {
                result = result.OrderBy(x => x.PersonnelId)
                     .Skip((userFilter.Page - 1) * userFilter.PageSize)
                     .Take(userFilter.PageSize);
            }

            return result.ToList();
        }

        public long GetAllFilteredCount(PersonnelFilter userFilter, int? archivedUserStateCD)
        {
            return GetPersonnelFiltered(userFilter, archivedUserStateCD).Count();
        }

        public bool ExistIdentifier(PersonnelIdentifier identifier)
        {
            return context.PersonnelIdentifiers
                .WhereEntriesAreActive()
                .Any(x => x.IdentifierTypeCD == identifier.IdentifierTypeCD && x.IdentifierValue == identifier.IdentifierValue && x.PersonnelIdentifierId != identifier.PersonnelIdentifierId);
        }

        public bool UserHasPermission(int userId, string moduleName, string permissionName)
        {
            return context.PersonnelPositionPermissionViews.Any(p => p.PersonnelId == userId && p.ModuleName == moduleName && p.PermissionName == permissionName);
        }

        public List<PersonnelPositionPermissionView> GetPermissions(int userId)
        {
            IQueryable<PersonnelPositionPermissionView> queryResult = context.PersonnelPositionPermissionViews.Where(p => p.PersonnelId == userId);
            return queryResult.ToList();
        }

        public string GetNameInfoById(int id)
        {
            return context.Personnel
                        .WhereEntriesAreActive()
                        .Where(x => x.PersonnelId == id)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault();
        }

        public async Task<IDictionary<int, string>> GetUsersDictionaryAsync(IEnumerable<int> userIds)
        {
            return await context.Personnel
                .WhereEntriesAreActive()
                .Where(x => userIds.Any(y => y == x.PersonnelId))
                .ToDictionaryAsync(x => x.PersonnelId, x => x.FirstName + " " + x.LastName)
                .ConfigureAwait(false);
        }

        public int? GetDoctorId(List<PersonnelIdentifier> personnelIdentifiers)
        {
            var personnel = context.Personnel
                .Include(x => x.PersonnelIdentifiers)
                .WhereEntriesAreActive()
                .ToList()
                ;
            return personnel
                .Find(p =>
                personnelIdentifiers.Exists(i => 
                    p.PersonnelIdentifiers.Exists(pI => 
                        pI.IdentifierValue != null && pI.IdentifierValue == i.IdentifierValue && 
                        pI.IdentifierTypeCD != null && pI.IdentifierTypeCD == i.IdentifierTypeCD && 
                        pI.IdentifierPoolCD != null && pI.IdentifierPoolCD == i.IdentifierPoolCD
                        )
                    )
                )
                ?.PersonnelId;
        }

        public IQueryable<Personnel> FilterForAutocomplete(PersonnelAutocompleteFilter personnelAutocompleteFilter, int? archivedUserStateCD)
        {
            string name = personnelAutocompleteFilter.Name;

            IQueryable<Personnel> query =
                context.Personnel
                    .WhereEntriesAreActive()
                    .Where(x =>
                         (x.IsDoctor == personnelAutocompleteFilter.FilterByDoctors || !personnelAutocompleteFilter.FilterByDoctors)
                        && x.Organizations.Any(y => y.OrganizationId == personnelAutocompleteFilter.OrganizationId && y.StateCD != archivedUserStateCD)
                        );

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => 
                    x.FirstName.ToLower().Contains(name.ToLower()) 
                        || x.LastName.ToLower().Contains(name.ToLower())
                );

            return query;
        }

        public void InsertOrUpdatePersonnelOccupation(Personnel user, PersonnelOccupation personnelOccupation)
        {
            if (user.PersonnelOccupationId == null)
            {
                personnelOccupation.PersonnelId = user.PersonnelId;
                context.PersonnelOccupations.Add(personnelOccupation);
                context.SaveChanges();
                user.CopyPersonnelOccupationId(personnelOccupation.PersonnelOccupationId);
            }
            else 
            {
                PersonnelOccupation personnelOccupationDb = GetPersonnelOccupationById(user.PersonnelOccupationId);
                personnelOccupationDb.Copy(personnelOccupation, user.PersonnelId);
                personnelOccupationDb.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public async Task<PaginationData<Personnel>> FilterAndCountPersonnelByProject(ProjectFilter filter, int? archivedUserStateCD)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            IQueryable<Personnel> query = context.Personnel
                .Include(x => x.PersonnelTeams)
                    .ThenInclude(y => y.PersonnelTeam)
                .WhereEntriesAreActive();

            if (filter.ProjectId != null)
            {
                var projectPersonnelIds = GetProjectPersonnels(filter.ProjectId.Value).Select(p => p.PersonnelId);
                if (filter.ShowAddedPersonnels)
                    query = query.Where(x => projectPersonnelIds.Contains(x.PersonnelId));
                else
                    query = query.Where(x => !projectPersonnelIds.Contains(x.PersonnelId));
            }

            if (filter.PersonnelId != null)
            {
                query = query.Where(x => x.PersonnelId == filter.PersonnelId.Value);
            }
            if (filter.PersonnelTeamId != null)
            {
                query = query.Where(x => 
                    x.PersonnelTeams.Any(y => y.PersonnelTeamId == filter.PersonnelTeamId.Value 
                    && y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now));
            }
            if (filter.OrganizationId != null)
            {
                query = query.Where(x => x.Organizations.Any(y => y.OrganizationId == filter.OrganizationId.Value && y.StateCD != archivedUserStateCD));
            }
            if (filter.ActiveOrganizationId != null)
            {
                query = query.Where(x => x.Organizations.Any(y => y.OrganizationId == filter.ActiveOrganizationId.Value && y.StateCD != archivedUserStateCD));
            }
            if (filter.OccupationCD != null)
            {
                query = query.Where(x => x.PersonnelOccupation != null && x.PersonnelOccupation.OccupationCD == filter.OccupationCD.Value);
            }

            int count = await query.CountAsync().ConfigureAwait(false);

            query = ApplyOrderByAndPaging(filter, query);

            return new PaginationData<Personnel>(count, await query.ToListAsync().ConfigureAwait(false));
        }

        public async Task<PaginationData<Personnel>> GetAllPatientListPersonnelsAndCount(PatientListFilter filter, int? archivedUserStateCD)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            IQueryable<Personnel> query = context.Personnel
                .Include(x => x.PersonnelTeams)
                    .ThenInclude(y => y.PersonnelTeam)
                .WhereEntriesAreActive();

            if (filter.PersonnelId != null)
            {
                query = query.Where(x => x.PersonnelId == filter.PersonnelId.Value);
            }
            if (filter.PersonnelTeamId != null)
            {
                query = query.Where(x => x.PersonnelTeams.Any(y => y.PersonnelTeamId == filter.PersonnelTeamId.Value
                    && y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now));
            }
            if (filter.OrganizationId != null)
            {
                query = query.Where(x => x.Organizations.Any(y => y.OrganizationId == filter.OrganizationId.Value && y.StateCD != archivedUserStateCD));
            }
            if (filter.OccupationCD != null)
            {
                query = query.Where(x => x.PersonnelOccupation != null && x.PersonnelOccupation.OccupationCD == filter.OccupationCD.Value);
            }

            if ((bool)filter.LoadSelectedPersonnel)
                query = query.Where(x => x.PatientListPersonnelRelations.Any(y => y.PatientListId == filter.PatientListId
                    && y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now));
            else if(filter.PatientListId > 0)
                query = query.Where(x => !x.PatientListPersonnelRelations.Any(y => y.PatientListId == filter.PatientListId
                        && y.EntityStateCD != (int)EntityStateCode.Deleted && y.ActiveFrom <= now && y.ActiveTo >= now));

            int count = await query.CountAsync().ConfigureAwait(false);

            query = ApplyOrderByAndPaging(filter, query);

            return new PaginationData<Personnel>(count, await query.ToListAsync().ConfigureAwait(false));
        }

        private IQueryable<Personnel> ApplyOrderByAndPaging(EntityFilter filter, IQueryable<Personnel> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.ColumnName))
            {
                switch (filter.ColumnName)
                {
                    case AttributeNames.Occupation:
                        if (filter.IsAscending)
                        {
                            query = query.OrderBy(x => x.PersonnelOccupation.Occupation.ThesaurusEntry.Translations.FirstOrDefault(y => y.Language == LanguageConstants.EN).PreferredTerm);
                            break;
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.PersonnelOccupation.Occupation.ThesaurusEntry.Translations.FirstOrDefault(y => y.Language == LanguageConstants.EN).PreferredTerm);
                            break;
                        }
                    default:
                        query = SortTableHelper.OrderByField(query, filter.ColumnName, filter.IsAscending);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.PersonnelId);
            }

            query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);
            return query;
        }

        private IQueryable<ProjectPersonnelRelation> GetProjectPersonnels(int projectId)
        {
            return context.ProjectPersonnelRelations
                .WhereEntriesAreActive()
                .Where(x => x.ProjectId == projectId);
        }

        private PersonnelOccupation GetPersonnelOccupationById(int? id)
        {
            return context.PersonnelOccupations
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.PersonnelOccupationId == id);
        }

        private List<int> GetOrganizationIds()
        {
            return context.Organizations
                .WhereEntriesAreActive()
                .Select(x => x.OrganizationId).ToList();
        }

        private Dictionary<int, int> MapNumOfUsersPerOrganization(List<int> organizationIds, int? archivedUserStateCD)
        {
            Dictionary<int, int> numOfUsersPerOrganization = new Dictionary<int, int>();
            foreach (var organizationId in organizationIds)
            {
                numOfUsersPerOrganization[organizationId] = GetAllByActiveOrganization(organizationId, archivedUserStateCD).Count();
            }
            return numOfUsersPerOrganization;
        }

        private void IncrementOrganizationsUserCount(List<int> organizationIds, int value)
        {
            foreach (var organization in context.Organizations.Where(x => organizationIds.Contains(x.OrganizationId)))
            {
                organization.NumOfUsers += value;
            }

            context.SaveChanges();
        }

        private IQueryable<PersonnelView> GetPersonnelFiltered(PersonnelFilter filterData, int? archivedUserStateCD)
        {
            int activeOrganizationFilter = filterData.ShowUnassignedUsers ? -1 : filterData.ActiveOrganization;
            string organizationIdFilter = PrepareSearchWordForComplexProperty(filterData.OrganizationId.HasValue ? filterData.OrganizationId.Value.ToString() : string.Empty);
            string roleIdFilter = PrepareSearchWordForComplexProperty(filterData.RoleCD.HasValue ? filterData.RoleCD.Value.ToString() : string.Empty);
            string personnelIdentifierFilter = PrepareSearchWordForComplexProperty(PrepareIdentifierSearchWord(filterData));
            string personnelAddressLikePattern = PrepareAddressSearchLikePattern(filterData);
            bool isEmptyAddressFilter = IsEmptyAddressFilter(filterData);

            IQueryable<PersonnelView> query = GetAllByActiveOrganization(activeOrganizationFilter, archivedUserStateCD)
                .Where(u =>
                    (filterData.Family == null || u.LastName.ToLower().Contains(filterData.Family.ToLower()))
                    && (filterData.Given == null || u.FirstName.ToLower().Contains(filterData.Given.ToLower()))
                    && (filterData.BusinessEmail == null || filterData.BusinessEmail == u.Email)
                    && (filterData.Username == null || filterData.Username == u.Username)
                    && (filterData.PersonnelTypeCD == null || filterData.PersonnelTypeCD == u.PersonnelTypeCD)
                    && (filterData.BirthDate == null || filterData.BirthDate == u.DayOfBirth)
                    && (string.IsNullOrEmpty(organizationIdFilter) || u.PersonnelOrganizationIds.Contains(organizationIdFilter))
                    && (string.IsNullOrEmpty(roleIdFilter) || u.PersonnelPositionIds.Contains(roleIdFilter))
                    && (string.IsNullOrEmpty(personnelIdentifierFilter) || u.PersonnelIdentifiers.Contains(personnelIdentifierFilter))
                    && (isEmptyAddressFilter || EF.Functions.Like(u.PersonnelAddresses, personnelAddressLikePattern))
                )
            ;

            return query;
        }

        private IQueryable<PersonnelView> GetAllByActiveOrganization(int activeOrganization, int? archivedUserStateCD)
        {
            IQueryable<PersonnelView> queryable = context.PersonnelViews;
            if (activeOrganization > 0)
            {
                return queryable
                    .WhereEntriesAreActive()
                    .Where(x => x.OrganizationId == activeOrganization && x.StateCD != archivedUserStateCD
                );
            }
            else
            {
                return queryable
                     .WhereEntriesAreActive()
                     .AsEnumerable()
                     .GroupBy(x => x.PersonnelId)
                     .Where(group => group.All(n => n.StateCD == archivedUserStateCD || n.StateCD == null))
                     .Select(group => group.FirstOrDefault())
                     .AsQueryable();
            }
        }

        private string PrepareIdentifierSearchWord(PersonnelFilter userFilter)
        {
            return userFilter.IdentifierType.HasValue && !string.IsNullOrEmpty(userFilter.IdentifierValue) ? $"{userFilter.IdentifierType}{Delimiters.ComplexSegmentDelimiter}{userFilter.IdentifierValue}" : string.Empty;
        }

        private string PrepareSearchWordForComplexProperty(string searchWord)
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                return searchWord;
            }
            else
            {
                return $"<{Delimiters.ComplexColumnDelimiter}>{searchWord}</{Delimiters.ComplexColumnDelimiter}>";
            }
        }

        #region Prepare Address Search Filter

        private string PrepareAddressSearchLikePattern(PersonnelFilter personnelFilter)
        {
            Dictionary<int, bool> conditions = ExamineConditions(personnelFilter);
            return GetMatchedPattern(GetMatchedCondition(conditions), personnelFilter);
        }

        private Dictionary<int, bool> ExamineConditions(PersonnelFilter filterData)
        {
            return new Dictionary<int, bool> { 
                { 1, IsEmptyAddressFilter(filterData) },
                { 2, filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode) },
                { 3, !filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode) },
                { 4, !filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode)},
                { 5, !filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode) },
                { 6, filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode)},
                { 7, filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode)},
                { 8, filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 9, !filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode)},
                { 10, !filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 11, filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 12, filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode)},
                { 13, !filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 14, filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 15, !filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)},
                { 16, filterData.CountryCD.HasValue && !string.IsNullOrEmpty(filterData.City) && !string.IsNullOrEmpty(filterData.Street) && !string.IsNullOrEmpty(filterData.PostalCode)}
            };
        }

        private int GetMatchedCondition(Dictionary<int, bool> conditions)
        {
            return conditions.Where(kV => kV.Value).Select(kV => kV.Key).FirstOrDefault();
        }

        private string GetMatchedPattern(int indexOfMatchedCondition, PersonnelFilter filterData)
        {
            if(GetPatterns(filterData).TryGetValue(indexOfMatchedCondition, out string result))
            {
                return result;
            } 
            else
            {
                return Delimiters.LIKE_CHAR;
            }
        }

        private Dictionary<int, string> GetPatterns(PersonnelFilter filterData)
        {
            return new Dictionary<int, string>() {
                { 1, $"{Delimiters.LIKE_CHAR}" },
                { 2, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 3, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 4, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 5, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 6, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 7, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 8, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 9, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 10, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 11, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 12, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}" },
                { 13, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 14, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 15, $"{Delimiters.LIKE_CHAR}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" },
                { 16, $"{Delimiters.LIKE_CHAR}<{Delimiters.ComplexColumnDelimiter}>{filterData.CountryCD}{Delimiters.ComplexSegmentDelimiter}{filterData.City}{Delimiters.ComplexSegmentDelimiter}{filterData.Street}{Delimiters.ComplexSegmentDelimiter}{filterData.PostalCode}</{Delimiters.ComplexColumnDelimiter}>{Delimiters.LIKE_CHAR}" }
            };
        }

        private bool IsEmptyAddressFilter(PersonnelFilter filterData)
        {
            return !filterData.CountryCD.HasValue && string.IsNullOrEmpty(filterData.City) && string.IsNullOrEmpty(filterData.Street) && string.IsNullOrEmpty(filterData.PostalCode); 
        }

        #endregion /Prepare Address Search Filter
    }
}
