using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IProjectManagementDAL
    {
        Task<List<Project>> GetAllAsync(ProjectFilter filter);
        Task<List<Project>> GetAllUserProjectsAsync(ProjectFilter filter, int personnelId);
        Task<int> GetAllEntriesCountAsync(ProjectFilter filter);
        Task<int> GetAllUserProjectsCountAsync(ProjectFilter filter, int personnelId);
        Task<Project> InsertOrUpdate(Project project);
        Task<Project> GetByIdAsync(int id);
        Project GetById(int id);
        Task<int> Delete(int id);
        Task<int> Archive(int id);
        Task<int> AddPersonnels(List<ProjectPersonnelRelation> personnelProjects);
        Task<int> RemovePersonnelFromProject(int personnelId, int projectId);
        Task<List<string>> GetFormIdsByProject(int projectId);
        Task<int> AddDocumentToProject(ProjectDocumentRelation projectDocumentRelation);
        Task<int> RemoveDocumentFromProject(string formId, int projectId);
        Task<int> AddPatientToProjects(List<ProjectPatientRelation> projectPatientRelations);
        Task<int> RemovePatientFromProject(int patientId, int projectId);
        Task<PaginationData<AutoCompleteData>> GetProjectAutoCompleteTitleAndCount(ProjectFilter filter);
        List<Project> GetProjectByIds(List<int> ids);
        Task<List<int>> GetProjectIdsByPatient(int patientId);
        string GetNameById(int projectId);
        List<Project> GetAllByIds(List<int?> ids);
        List<int> GetAllProjectsIdsFor(int personnedId);
    }
}
