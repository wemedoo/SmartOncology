using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Organization.DataIn;
using sReportsV2.DTOs.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IOrganizationBLL : IChildEntryBLL<OrganizationIdentifierDataIn>, IChildEntryBLL<OrganizationTelecomDataIn>, IExistBLL<OrganizationIdentifierDataIn>
    {
        PaginationDataOut<OrganizationDataOut, DataIn> ReloadTable(OrganizationFilterDataIn dataIn);
        OrganizationDataOut GetOrganizationById(int organizationId);
        CreateResponseResult Insert(OrganizationDataIn organization);
        void Delete(OrganizationDataIn organization);
        OrganizationDataOut GetOrganizationForEdit(int organizationId);
        AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn autocompleteDataIn);
        List<OrganizationDataOut> ReloadHierarchy(int? parent);
        List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries);
        List<OrganizationUsersDataOut> GetUsersByOrganizationsIds(List<int> ids);
        List<OrganizationCommunicationEntityDataOut> GetOrgCommunicationByOrgId(OrganizationFilterDataIn dataIn);
        Task<int> InsertOrganizationCommunication(OrganizationCommunicationEntityDataIn dataIn);
        Task<OrganizationCommunicationEntityDataOut> GetOrgCommunicationEntityIdByIdAsync(int orgCommunicationEntityId);
        string GetTimeZoneOffset(int organizationId);
    }
}
