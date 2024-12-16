using sReportsV2.DAL.Sql.Sql;
using sReportsV2.SqlDomain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using System.Collections.Generic;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class TaskDAL : ITaskDAL
    {
        private readonly SReportsContext context;
        public TaskDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Sql.Entities.TaskEntry.Task> GetByIdAsync(int taskId)
        {
            return await context.Tasks
                 .Include(x => x.TaskDocument)
                 .WhereEntriesAreActive()
                 .FirstOrDefaultAsync(x => x.TaskId == taskId).ConfigureAwait(false);
        }

        public async Task<int> InsertOrUpdateAsync(Domain.Sql.Entities.TaskEntry.Task task)
        {
            SetDocumentId(task);

            if (task.TaskId == 0)
            {
                context.Tasks.Add(task);
            }
            else
            {
                Domain.Sql.Entities.TaskEntry.Task dbTask = await this.GetByIdAsync(task.TaskId);
                dbTask.SetLastUpdate();
                dbTask.Copy(task);
            }

            await context.SaveChangesAsync();

            return task.TaskId;
        }

        public async Task<List<Domain.Sql.Entities.TaskEntry.Task>> GetAllAsync(TaskFilter filter)
        {
            IQueryable<Domain.Sql.Entities.TaskEntry.Task> result = GetTaskFiltered(filter);

            result = GetAllTasks(filter, result);

            return await result.ToListAsync().ConfigureAwait(false);
        }

        public async Task<int> GetAllEntriesCountAsync(TaskFilter filter)
        {
            return await GetTaskFiltered(filter).CountAsync()
                .ConfigureAwait(false);
        }

        private IQueryable<Domain.Sql.Entities.TaskEntry.Task> GetTaskFiltered(TaskFilter filter)
        {
            IQueryable<Domain.Sql.Entities.TaskEntry.Task> taskQuery = context.Tasks
                .Include(x => x.TaskType.ThesaurusEntry)
                .Include(x => x.TaskType.ThesaurusEntry.Translations)
                .Include(x => x.TaskStatus.ThesaurusEntry)
                .Include(x => x.TaskStatus.ThesaurusEntry.Translations)
                .Include(x => x.TaskDocument)
                .WhereEntriesAreActive();

            if (filter.PatientId.HasValue)
            {
                taskQuery = taskQuery.Where(x => x.PatientId == filter.PatientId);
            }
            if (filter.TaskStatus.HasValue)
            {
                taskQuery = taskQuery.Where(x => x.TaskStatusCD == filter.TaskStatus);
            }

            return taskQuery;
        }

        private IQueryable<Domain.Sql.Entities.TaskEntry.Task> GetAllTasks(TaskFilter filter, IQueryable<Domain.Sql.Entities.TaskEntry.Task> result)
        {
            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderByDescending(x => x.TaskStartDateTime);

            return result;
        }

        private IQueryable<Domain.Sql.Entities.TaskEntry.Task> SortByField(IQueryable<Domain.Sql.Entities.TaskEntry.Task> result, TaskFilter filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.TaskType:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.TaskTypeCD);
                    else
                        return result.OrderByDescending(x => x.TaskTypeCD);
                case AttributeNames.TaskStatus:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.TaskStatusCD);
                    else
                        return result.OrderByDescending(x => x.TaskStatusCD);
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
            }
        }

        private void SetDocumentId(Domain.Sql.Entities.TaskEntry.Task task) 
        {
            TaskDocument taskDocument = context.TaskDocuments.FirstOrDefault(t => t.TaskDocumentCD.ToString() == task.TaskEntityId);
            if (taskDocument != null)
            {
                task.TaskDocumentId = taskDocument.TaskDocumentId;
                task.TaskEntityId = taskDocument.FormId;
            }
        }

        public int InsertTaskDocument(TaskDocument taskDocument, string organizationTimeZone = null)
        {
            TaskDocument taskDocumentFromDb = GetTaskDocumentByCodeIdAndFormId(taskDocument.TaskDocumentCD, taskDocument.FormId);

            if (taskDocumentFromDb == null)
            {
                taskDocument.SetEntryDatetime(organizationTimeZone);
                context.TaskDocuments.Add(taskDocument);
            }
            else
            {
                taskDocumentFromDb.SetActiveFromAndTo(organizationTimeZone);
            }
            context.SaveChanges();

            return taskDocument.TaskDocumentId;
        }

        public void SetTaskDocumentToInactive(TaskDocument taskDocument, string organizationTimeZone = null)
        {
            taskDocument.Delete(setLastUpdateProperty: false, organizationTimeZone: organizationTimeZone);
            context.SaveChanges();
        }

        public bool ExistTaskDocument(string formId)
        {
            return this.context.TaskDocuments
                .WhereEntriesAreActive()
                .Any(x => x.FormId.Equals(formId));
        }

        public TaskDocument GetTaskDocumentByFormId(string formId)
        {
            return context.TaskDocuments
                .WhereEntriesAreActive()
               .FirstOrDefault(x => x.FormId == formId);
        }

        public TaskDocument GetTaskDocumentByCodeIdAndFormId(int codeId, string formId)
        {
            return context.TaskDocuments
                .Where(x => x.TaskDocumentCD == codeId && x.FormId == formId)
                .FirstOrDefault();
        }

        public List<TaskDocument> GetAllTaskDocuments()
        {
            return context.TaskDocuments
               .WhereEntriesAreActive()
               .ToList();
        }
    }
}
