using AutoMapper;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPersonnelTeamRelationBLL
    {
        PersonnelTeamRelationDataOut GetById(int id);
        void InsertOrUpdate(PersonnelTeamRelationDataIn personnelTeamRelationDataIn);
        void InsertMany(List<PersonnelTeamRelationDataIn> personnelTeamRelationDataIns);
        void Delete(int personnelTeamRelationId);
        PaginationDataOut<PersonnelTeamRelationDataOut, DataIn> GetAllFiltered(PersonnelTeamRelationFilterDataIn dataIn);
        AutocompleteResultDataOut GetNameForAutocomplete(AutocompleteDataIn autocompleteDataIn, int personnelTeamId);
        AutocompleteResultDataOut GetPersonnelTeamRelationshipTypeCodes(string activeLanguage, List<CodeDataOut> CodesDataOut);
    }
}
