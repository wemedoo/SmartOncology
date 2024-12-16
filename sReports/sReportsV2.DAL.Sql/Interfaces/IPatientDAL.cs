using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPatientDAL : IChildEntryDAL<PatientIdentifier>, IChildEntryDAL<PatientAddress>, IChildEntryDAL<PatientContactAddress>, IChildEntryDAL<PatientTelecom>, IChildEntryDAL<PatientContactTelecom>, IChildEntryDAL<PatientContact>
    {
        void InsertOrUpdate(Patient patient, PatientIdentifier defaultIdentifier);
        int GetAllEntriesCount(PatientFilter filter);
        List<Patient> GetAll(PatientFilter filter);
        Patient GetById(int id);
        Task<Patient> GetByIdAsync(int id);
        Patient GetByIdentifier(PatientIdentifier identifier);
        Task<Patient> GetByIdentifierAsync(PatientIdentifier identifier);
        bool ExistsPatientByIdentifier(PatientIdentifier identifier);
        bool ExistsPatient(int id);
        Task Delete(Patient patient);
        List<Patient> GetAllByIds(List<int> ids);
        List<Patient> GetPatientsFilteredByFirstAndLastName(PatientByNameSearchFilter patientSearchFilter);
        Patient GetBy(Patient patient, PatientIdentifier mrnPatientIdentifier);
        IQueryable<Patient> GetPatientFiltered(PatientFilter filter);
        Task<PaginationData<Patient>> GetAllAndCount(PatientFilter filter);
    }
}
