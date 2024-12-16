using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IProjectManagementBLL
    {
        Task<PaginationDataOut<ProjectDataOut, DataIn>> ReloadTable(ProjectFilterDataIn dataIn);
        Task<PaginationDataOut<ProjectDataOut, DataIn>> ReloadUserProjectTable(ProjectFilterDataIn dataIn, int personnelId);
        Task<PaginationDataOut<UserDataOut, DataIn>> ReloadPersonnelTable(ProjectFilterDataIn dataIn);
        Task<PaginationDataOut<FormDataOut, FormFilterDataIn>> ReloadDocumentsTable(FormFilterDataIn dataIn, int projectId, UserCookieData userCookieData);
        Task<ProjectDataOut> InsertOrUpdate(ProjectDataIn dataIn);
        Task<ProjectDataOut> GetById(int id);
        Task<int> Delete(int id);
        Task<int> Archive(int id);
        Task<int> AddPersonnelsToProject(List<ProjectPersonnelDataIn> personnelProjects);
        Task<int> RemovePersonnelsFromProject(int personnelId, int projectId);
        Task<int> AddDocumentToProject(ProjectDocumentDataIn dataIn);
        Task<int> RemoveDocumentFromProject(string formId, int projectId);
        Task<int> AddPatientToProjects(List<PatientProjectDataIn> dataIn);
        Task<int> RemovePatientFromProject(int patientId, int projectId);
        Task<AutocompleteResultDataOut> GetDocumentTitleForAutocomplete(AutocompleteDataIn dataIn, UserCookieData userCookieData, int projectId);
        Task<AutocompleteResultDataOut> GetProjectAutoCompleteTitle(AutocompleteDataIn dataIn, int clinicalTrialTypeId);
        Task<AutocompleteResultDataOut> GetProjectAutoCompleteTitleForPatient(AutocompleteDataIn dataIn, int patientId, int clinicalTrialTypeId);
        string GetNameById(int projectId);
    }
}
