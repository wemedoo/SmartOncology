using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPatientBLL : IChildEntryBLL<PatientContactDataIn>, IChildEntryBLL<PatientAddressDataIn>, IChildEntryBLL<PatientIdentifierDataIn>, IChildEntryBLL<PatientContactAddressDataIn>, IChildEntryBLL<PatientTelecomDataIn>, IChildEntryBLL<PatientContactTelecomDataIn>, IExistBLL<PatientIdentifierDataIn>
    {
        ResourceCreatedDTO InsertOrUpdate(PatientDataIn patientDataIn, UserData userData);
        Task<PatientDataOut> GetByIdentifierAsync(PatientIdentifier identifier);
        PatientDataOut GetById(int id, bool loadClinicalTrials);
        PatientTableDataOut GetPreviewById(int id);
        Task<PatientDataOut> GetByIdAsync(int id);
        Task<ContactDTO> GetContactById(int id);
        Task Delete(PatientDataIn patientDataIn);
        Task<PaginationDataOut<T, PatientFilterDataIn>> GetAllFilteredAsync<T>(PatientFilterDataIn dataIn);
        List<PatientTableDataOut> GetPatientsByFirstAndLastName(PatientByNameFilterDataIn patientByNameFilterDataIn);
        AutocompleteResultDataOut GetAutocompletePatientData(AutocompleteDataIn dataIn, UserCookieData userCookieData);
    }
}
