using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface IOrganizationDAL : IChildEntryDAL<OrganizationTelecom>, IChildEntryDAL<OrganizationIdentifier>
    {
        List<int> GetClinicalDomainsForIds(List<int> ids);
        Organization GetByName(string name);
        Organization GetById(int id, bool noTracking = false);
        void InsertOrUpdate(Organization organization);
        List<Organization> GetAll(OrganizationFilter organizationFilter);
        void Delete(Organization organization);
        bool ExistIdentifier(OrganizationIdentifier identifier);
        IQueryable<Organization> FilterByName(string name);
        long GetAllCount();
        long GetAllEntriesCountByCountry(int? countryCD);
        List<OrganizationUsersCount> GetOrganizationUsersCount(string term, Dictionary<int, string> countries);
        List<Organization> GetByIds(List<int> ids);
        void InsertOrganizationRelation(OrganizationRelation relation);
        long GetAllFilteredCount(OrganizationFilter organizationFilter);
        List<PersonnelOrganization> GetUsersByOrganizationIds(List<int> ids);
        List<OrganizationCommunicationEntity> GetOrgCommunicationByOrgId(OrganizationFilter organizationFilter);
        Task<int> InsertOrganizationCommunication(OrganizationCommunicationEntity organizationCommunicationEntity);
        Task<OrganizationCommunicationEntity> GetOrgCommunicationEntityIdByIdAsync(int orgCommunicationEntityId);
        string GetTimeZoneOffset(int organizationId);
    }
}
