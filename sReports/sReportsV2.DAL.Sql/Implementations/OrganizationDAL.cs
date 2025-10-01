using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class OrganizationDAL : IOrganizationDAL
    {
        private readonly SReportsContext context;

        public OrganizationDAL(SReportsContext sReportsContext)
        {
            this.context = sReportsContext;
        }

        #region CRUD
        public void InsertOrUpdate(Organization organization)
        {
            if (organization.OrganizationId == 0)
            {
                context.Organizations.Add(organization);
            }
            else
            {
                context.UpdateEntryMetadata(organization);
            }

            context.SaveChanges();
        }

        public async Task InsertOrUpdate(OrganizationTelecom entry)
        {
            if (entry.OrganizationTelecomId == 0)
            {
                context.OrganizationTelecoms.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdate(OrganizationIdentifier entry)
        {
            if (entry.OrganizationIdentifierId == 0)
            {
                context.OrganizationIdentifiers.Add(entry);
            }
            else
            {
                context.UpdateEntryMetadata(entry);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Organization GetById(int id, bool noTracking = false)
        {
            var data = context.Organizations.Where(x => x.OrganizationId == id)
                .Include(x => x.OrganizationAddress)
                .Include(x => x.Telecoms)
                .Include(x => x.ClinicalDomains)
                .Include(x => x.OrganizationIdentifiers)
                .Include(x => x.OrganizationCommunicationEntities)
                .Include("OrganizationRelation")
                .Include("OrganizationRelation.Child")
                .Include("OrganizationRelation.Parent");

            if (noTracking)
            {
                data = data.AsNoTracking();
            }

            return data.FirstOrDefault();
        }

        public async Task<OrganizationIdentifier> GetById(QueryEntityParam<OrganizationIdentifier> queryEntityParams)
        {
            return await context.OrganizationIdentifiers
                .FirstOrDefaultAsync(x => x.OrganizationIdentifierId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public async Task<OrganizationTelecom> GetById(QueryEntityParam<OrganizationTelecom> queryEntityParams)
        {
            return await context.OrganizationTelecoms
                .FirstOrDefaultAsync(x => x.OrganizationTelecomId == queryEntityParams.Id).ConfigureAwait(false);
        }

        public void Delete(Organization organization)
        {
            Organization fromDb = GetById(organization.OrganizationId);
            DoDeleteCheck(fromDb);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, organization.RowVersion);
                context.SaveChanges();
            }
        }

        public async Task Delete(OrganizationTelecom entry)
        {
            OrganizationTelecom fromDb = await GetById(new QueryEntityParam<OrganizationTelecom>(entry.OrganizationTelecomId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(OrganizationIdentifier entry)
        {
            OrganizationIdentifier fromDb = await GetById(new QueryEntityParam<OrganizationIdentifier>(entry.OrganizationIdentifierId)).ConfigureAwait(false);
            if (fromDb != null)
            {
                context.UpdateEntryMetadataOnDelete(fromDb, entry.RowVersion);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion /CRUD

        public bool ExistIdentifier(OrganizationIdentifier identifier)
        {
            return context.OrganizationIdentifiers
                .WhereEntriesAreActive()
                .Any(x => x.IdentifierTypeCD == identifier.IdentifierTypeCD 
                && x.IdentifierValue == identifier.IdentifierValue && x.OrganizationIdentifierId != identifier.OrganizationIdentifierId 
                );
        }

        public List<Organization> GetAll(OrganizationFilter organizationFilter)
        {
            IQueryable<Organization> result = GetOrganizationsFiltered(organizationFilter);
            
            if (organizationFilter.ColumnName != null) 
                result = SortByField(result, organizationFilter);
            else
                result = result.OrderByDescending(x => x.OrganizationId)
                    .Skip(organizationFilter.GetHowManyElementsToSkip())
                    .Take(organizationFilter.PageSize);

            return result.ToList();
        }

        public long GetAllFilteredCount(OrganizationFilter organizationFilter)
        {
            return GetOrganizationsFiltered(organizationFilter).Count();
        }

        public IQueryable<Organization> FilterByName(string name)
        {
            return context.Organizations
                .WhereEntriesAreActive()
                .Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        public long GetAllCount()
        {
            return context.Organizations
                .WhereEntriesAreActive()
                .Count();
        }

        public long GetAllEntriesCountByCountry(int? countryCD)
        {
            return context.Organizations.Include(x => x.OrganizationAddress).Where(x => x.OrganizationAddress != null && x.OrganizationAddress.CountryCD == countryCD).Count();
        }

        public Organization GetByName(string name)
        {
            return context.Organizations
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.Name.Equals(name));
        }

        public List<int> GetClinicalDomainsForIds(List<int> ids)
        {
            return context.OrganizationClinicalDomains
                .Where(x => ids.Contains(x.OrganizationId))
                .Select(x => x.ClinicalDomainCD.Value)
                .Distinct()
                .ToList();
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, Dictionary<int, string> countries)
        {
            List<OrganizationUsersCount> result = context.Organizations
                .Include(x => x.OrganizationRelation)
                .Include(x => x.OrganizationAddress)
                .WhereEntriesAreActive()
                .Select(organization => new OrganizationUsersCount()
                {
                    OrganizationName = organization.Name,
                    UsersCount = organization.NumOfUsers,
                    PartOf = organization.OrganizationRelation != null ? organization.OrganizationRelation.ParentId : 0,
                    OrganizationId = organization.OrganizationId,
                    CountryCD = organization.OrganizationAddress != null ? organization.OrganizationAddress.CountryCD : null
                }) 
                .ToList();

            var ancestor = result.Where(x => x.PartOf == 0);

            SetOrganizationsChildren(result);

            return ancestor
                .Where(x => string.IsNullOrWhiteSpace(term) || x.FoundName(term))
                .Where(x => countries == null || countries.Count == 0 || x.FoundCountry(countries.Keys.ToList()))
                .ToList();
        }

        public List<PersonnelOrganization> GetUsersByOrganizationIds(List<int> ids)
        {
            return context.Personnel
                .SelectMany(x => x.Organizations)
                .Where(x => ids.Contains(x.OrganizationId))
                .Include(x => x.Personnel)
                .Include(x => x.Organization)
                .ToList();
        }

        public List<Organization> GetByIds(List<int> ids)
        {
            return context.Organizations
                .Include(x => x.OrganizationAddress)
                .WhereEntriesAreActive()
                .Where(x => ids.Contains(x.OrganizationId))
                .ToList();
        }

        public void InsertOrganizationRelation(OrganizationRelation relation)
        {

            if (relation.OrganizationRelationId == 0)
            {
                context.OrganizationRelations.Add(relation);
                context.SaveChanges();
            }
        }

        public List<OrganizationCommunicationEntity> GetOrgCommunicationByOrgId(OrganizationFilter organizationFilter)
        {
            IQueryable<OrganizationCommunicationEntity> query = context.OrganizationCommunicationEntities
                 .Where(org => org.OrganizationId == organizationFilter.OrganizationId);

            if (organizationFilter.ShowActive && !organizationFilter.ShowInactive)
            {
                query = query.WhereEntriesAreActive();
            }
            if (!organizationFilter.ShowActive && organizationFilter.ShowInactive)
            {
                query = query.WhereEntriesAreInactive();
            }

            if (organizationFilter.ColumnName != null)
                query = SortTableHelper.OrderByField(query, organizationFilter.ColumnName, organizationFilter.IsAscending);
            else
                query = query.OrderByDescending(x => x.EntryDatetime);

            return query.ToList();
        }

        public async Task<int> InsertOrganizationCommunication(OrganizationCommunicationEntity organizationCommunicationEntity)
        {
            if (organizationCommunicationEntity.OrgCommunicationEntityId == 0)
            {
                organizationCommunicationEntity.SetActiveFromAndToDatetime();
                context.OrganizationCommunicationEntities.Add(organizationCommunicationEntity);
            }
            else
            {
                OrganizationCommunicationEntity dbOrganizationCommunicationEntity = await this.GetOrgCommunicationEntityIdByIdAsync(organizationCommunicationEntity.OrgCommunicationEntityId);
                dbOrganizationCommunicationEntity.SetLastUpdate();
                dbOrganizationCommunicationEntity.Copy(organizationCommunicationEntity);
            }

            await context.SaveChangesAsync();

            return organizationCommunicationEntity.OrgCommunicationEntityId;
        }

        public async Task<OrganizationCommunicationEntity> GetOrgCommunicationEntityIdByIdAsync(int orgCommunicationEntityId)
        {
            return await context.OrganizationCommunicationEntities
                 .FirstOrDefaultAsync(x => x.OrgCommunicationEntityId == orgCommunicationEntityId)
                 .ConfigureAwait(false);
        }

        private void SetOrganizationsChildren(List<OrganizationUsersCount> allOrganization)
        {
            foreach (OrganizationUsersCount organization in allOrganization)
            {
                organization.Children = SetOrganizationChildren(organization.OrganizationId, allOrganization);
            }
        }

        private List<OrganizationUsersCount> SetOrganizationChildren(int organizationId, List<OrganizationUsersCount> allOrganizations)
        {
            List<OrganizationUsersCount> allChildren = new List<OrganizationUsersCount>();
            foreach (OrganizationUsersCount organization in allOrganizations.Where(x => x.PartOf != 0 && x.PartOf == organizationId))
            {
                if (organizationId == organization.PartOf)
                {
                    allChildren.Add(organization);
                }
            }
            return allChildren;
        }

        private IQueryable<Organization> GetOrganizationsFiltered(OrganizationFilter filterData)
        {
            IQueryable<Organization> query = context.Organizations
                .Include(x => x.OrganizationIdentifiers)
                .Include(x => x.OrganizationAddress)
                .WhereEntriesAreActive()
                .Where(org => (filterData.Name == null || org.Name.ToLower().Contains(filterData.Name.ToLower()))
              && (filterData.Alias == null || org.Alias.Equals(filterData.Alias))
              && (filterData.City == null || org.OrganizationAddress.City.Equals(filterData.City))
              && (filterData.State == null || org.OrganizationAddress.State.Equals(filterData.State))
              && (filterData.PostalCode == null || org.OrganizationAddress.PostalCode.Equals(filterData.PostalCode))
              && (filterData.Street == null || org.OrganizationAddress.Street.Equals(filterData.Street))
              && (filterData.CountryCD == null || org.OrganizationAddress.CountryCD == filterData.CountryCD)
              && (filterData.Type == null || org.TypesString.Contains(filterData.Type))
                );

            if (filterData.ClinicalDomainCD.HasValue)
            {
                query = query
                   .Join(
                      context.OrganizationClinicalDomains.WhereEntriesAreActive(),
                      org => org.OrganizationId,
                      cD => cD.OrganizationId,
                      (org, cD) => new { org, cD })
                   .Where(a => a.cD.ClinicalDomainCD == filterData.ClinicalDomainCD)
                   .Select(a => a.org);
            }

            if (filterData.Parent != null)
            {
                query = query.Where(org => org.OrganizationRelation.Parent.OrganizationId == filterData.Parent);
            }

            query = FilterByIdentifier(query, filterData.IdentifierType, filterData.IdentifierValue);

            return query;
        }

        private IQueryable<Organization> FilterByIdentifier(IQueryable<Organization> query, int? identifierType, string value)
        {
            IQueryable<Organization> result = query;
            if (identifierType.HasValue && !string.IsNullOrEmpty(value))
            {
                if (identifierType == -1)
                {
                    if (int.TryParse(value, out int O4PatientId))
                    {
                        result = query.Where(x => x.OrganizationId.Equals(O4PatientId));
                    }
                }
                else
                {
                    result = query
                        .Where(x => x.OrganizationIdentifiers.Any(y => y.IdentifierTypeCD == identifierType && y.IdentifierValue == value));
                }
            }
          
            return result;
        }

        private void DoDeleteCheck(Organization organization)
        {
            if (organization.NumOfUsers > 0)
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, $"Cannot delete because there are active users in {organization.Name}");
            }
        }

        private IQueryable<Organization> SortByField(IQueryable<Organization> result, OrganizationFilter organizationFilter) 
        {
            switch (organizationFilter.ColumnName)
            {
                case AttributeNames.Address:
                    if (organizationFilter.IsAscending)
                        return result.OrderBy(x => x.OrganizationAddress.City)
                                .ThenBy(x => x.OrganizationAddress.PostalCode)
                                .ThenBy(x => x.OrganizationAddress.Country)
                                .Skip(organizationFilter.GetHowManyElementsToSkip())
                                .Take(organizationFilter.PageSize);
                    else
                        return result.OrderByDescending(x => x.OrganizationAddress.City)
                                .ThenByDescending(x => x.OrganizationAddress.PostalCode)
                                .ThenByDescending(x => x.OrganizationAddress.Country)
                                .Skip(organizationFilter.GetHowManyElementsToSkip())
                                .Take(organizationFilter.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, organizationFilter.ColumnName, organizationFilter.IsAscending)
                                 .Skip(organizationFilter.GetHowManyElementsToSkip())
                                 .Take(organizationFilter.PageSize);
            }
        }
    }
}