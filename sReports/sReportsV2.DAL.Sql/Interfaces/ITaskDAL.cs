using sReportsV2.Domain.Sql.Entities.TaskEntry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ITaskDAL
    {
        Task<Domain.Sql.Entities.TaskEntry.Task> GetByIdAsync(int taskId);
        Task<int> InsertOrUpdateAsync(Domain.Sql.Entities.TaskEntry.Task task);
        Task<List<Domain.Sql.Entities.TaskEntry.Task>> GetAllAsync(TaskFilter filter);
        Task<int> GetAllEntriesCountAsync(TaskFilter filter);
        int InsertTaskDocument(TaskDocument taskDocument, string organizationTimeZone = null);
        bool ExistTaskDocument(string formId);
        TaskDocument GetTaskDocumentByFormId(string formId);
        void SetTaskDocumentToInactive(TaskDocument taskDocument, string organizationTimeZone = null);
        TaskDocument GetTaskDocumentByCodeIdAndFormId(int codeId, string formId);
        List<TaskDocument> GetAllTaskDocuments();
    }
}