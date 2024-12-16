using sReportsV2.Common.Entities;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPatientListDAL
    {
        Task<PaginationData<PatientList>> GetAllByFilter(PatientListFilter filter);
        Task<PatientList> GetByIdAsync(int id);
        PatientList GetById(int id);
        Task<PatientList> Create(PatientList patientList);
        Task<PatientList> Edit(PatientList patientList);
        Task Delete(int id);
        Task<PatientListPersonnelRelation> AddPersonnelRelation(PatientListPersonnelRelation patientListPersonnelRelation);
        Task<List<PatientListPersonnelRelation>> AddPersonnelRelations(List<PatientListPersonnelRelation> patientListPersonnelRelations);
        Task RemovePersonnelRelation(int patientListId, int personnelId);
        Task<PaginationData<AutoCompleteData>> GetTrialAutoCompleteNameAndCount(string term, EntityFilter filter);
        Task AddPatientRelation(PatientListPatientRelation patientListPatientRelation);
        Task RemovePatientRelation(PatientListPatientRelation patientListPatientRelation);
        Task<Dictionary<int, IEnumerable<int>>> GetListsContainingPatients(IEnumerable<int> patientIds);
    }
}
