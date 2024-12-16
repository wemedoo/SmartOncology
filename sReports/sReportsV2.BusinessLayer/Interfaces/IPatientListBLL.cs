using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.PatientList;
using sReportsV2.DTOs.DTOs.PatientList.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPatientListBLL
    {
        Task<PaginationDataOut<PatientListDTO, PatientListFilterDataIn>> GetAll(PatientListFilterDataIn dataIn);
        Task<PatientListDTO> GetById(int? id);
        Task<PatientListDTO> Create(PatientListDTO patientListDTO);
        Task<PatientListDTO> Edit(PatientListDTO patientListDTO);
        Task Delete(int? id);
        Task<PatientListPersonnelRelationDTO> AddPersonnelRelation(PatientListPersonnelRelationDTO patientListPersonnelRelationDTO);
        Task<List<PatientListPersonnelRelationDTO>> AddPersonnelRelations(List<PatientListPersonnelRelationDTO> patientListPersonnelRelationDTOs);
        Task RemovePersonnelRelation(int patientListId, int personnelId);
        Task<AutocompleteResultDataOut> GetAutoCompleteName(AutocompleteDataIn dataIn);
        Task<PaginationDataOut<UserDataOut, DataIn>> ReloadPersonnelTable(PatientListFilterDataIn dataIn);
        Task AddPatientRelations(PatientListPatientRelationDTO patientListPatientRelationDTO);
        Task RemovePatientRelation(PatientListPatientRelationDTO patientListPatientRelationDTO);
        Task<Dictionary<int, IEnumerable<PatientListDTO>>> GetListsAvailableForPatients(List<PatientDataOut> patients, int activePersonnelId);
    }
}
