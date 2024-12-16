using sReportsV2.Cache.Singleton;
using System.Threading.Tasks;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.DTOs.TaskEntry.DataOut;
using sReportsV2.DTOs.DTOs.TaskEntry.DataIn;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskBLL taskBLL;
        private readonly ICodeBLL codeBLL;

        public TaskController(ITaskBLL taskBLL, ICodeBLL codeBLL, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.taskBLL = taskBLL;
            this.codeBLL = codeBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ShowTaskModal(int taskId, bool isReadOnlyViewMode)
        {
            TaskDataOut taskData = await taskBLL.GetByIdAsync(taskId).ConfigureAwait(false) ?? new TaskDataOut();
            SetTaskViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("TaskModal", taskData);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEncounter, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Create(TaskDataIn task)
        {
            int taskId = await taskBLL.InsertOrUpdateAsync(task).ConfigureAwait(false);

            return Json(new CreateResponseResult { Id = taskId });
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ReloadTable(TaskFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            SetTaskViewBags();
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            PaginationDataOut<TaskDataOut, DataIn> result = await taskBLL.GetAllFiltered(dataIn).ConfigureAwait(false);

            return PartialView("TaskEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult ShowTaskFilterGroup()
        {
            SetTaskViewBags();
            return PartialView("TaskFilterGroup");
        }

        private void SetTaskViewBags()
        {
            ViewBag.TaskTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskType);
            ViewBag.TaskStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskStatus);
            ViewBag.TaskPriorities = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskPriority);
            ViewBag.TaskClasses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskClass);
            ViewBag.TaskDocuments = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskDocument);
        }
    }
}