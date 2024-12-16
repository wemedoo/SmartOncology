using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPersonnelTeamBLL
    {
        PersonnelTeamDataOut GetById(int personnelTeamId);
        void InsertOrUpdate(PersonnelTeamDataIn personnelTeamDataIn);
        PaginationDataOut<PersonnelTeamDataOut, DataIn> GetAllFiltered(PersonnelTeamFilterDataIn dataIn);
        AutocompleteResultDataOut GetNameForAutocomplete(AutocompleteDataIn autocompleteDataIn, int organizationId);
        bool IsNameUsedCheck(string name, int organizationId, int personnelTeamId);
        AutocompleteResultDataOut GetTeamTypeCodes(string activeLanguage, List<CodeDataOut> CodesDataOut);
        void Delete(int id);
        int CountTeamsPerOrganization(int organizationId);
        bool IsPersonnelUnique(int userId, int personnelTeamId);
        bool IsLeaderUnique(int roleCD, int? leaderRoleCD, int personnelTeamId);
    }
}
