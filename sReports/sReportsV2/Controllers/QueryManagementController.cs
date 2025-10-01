using sReportsV2.Common.CustomAttributes;
using System;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using AutoMapper;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.DTOs.DTOs.QueryManagement.DataIn;
using sReportsV2.DTOs.DTOs.QueryManagement.DataOut;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Controllers
{
    public class QueryManagementController : BaseController
    {
        private readonly IQueryManagementBLL queryManagementBLL;
        private readonly IMapper mapper;

        public QueryManagementController(IQueryManagementBLL queryManagementBLL, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.queryManagementBLL = queryManagementBLL;
            this.mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.QueryManagement)]
        public ActionResult GetAll(QueryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            ViewBag.FilterData = dataIn;
            SetViewBags();
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.QueryManagement)]
        public async Task<ActionResult> ReloadTable(QueryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            SetViewBags();
            PaginationDataOut<QueryDataOut, DataIn> result = await queryManagementBLL.GetAllFiltered(dataIn).ConfigureAwait(false);

            return PartialView("QueryEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.QueryManagement)]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await queryManagementBLL.Delete(id).ConfigureAwait(false);
            return NoContent();
        }


        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.QueryManagement)]
        public ActionResult ShowQueryModal(bool readOnly)
        {
            SetViewBags();
            ViewBag.IsReadOnly = readOnly;

            return PartialView("QueryModal");
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.QueryManagement)]
        public async Task<ActionResult> ShowQueryHistoryModal(QueryFilterDataIn dataIn, bool readOnly)
        {
            var result = await queryManagementBLL.GetByFieldId(dataIn).ConfigureAwait(false);
            ViewBag.IsReadOnly = readOnly;
            ViewBag.IsFormInstanceMode = true;

            return PartialView("QueryHistoryModal", result.FirstOrDefault());
        }

        [HttpGet]
        public async Task<ActionResult> LoadQueryHistoryTable(QueryFilterDataIn dataIn, bool isFormInstanceMode, bool readOnly)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            var result = new List<QueryDataOut>();

            if (isFormInstanceMode)
            {
                result = await queryManagementBLL.GetByFieldId(dataIn).ConfigureAwait(false);
            }
            else 
            {
                result = await queryManagementBLL.GetListById(dataIn.QueryId).ConfigureAwait(false);
            }

            SetViewBags();
            ViewBag.IsReadOnly = readOnly;
            ViewBag.IsFormInstanceMode = isFormInstanceMode;

            return PartialView("QueryHistoryTablePartial", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.QueryManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public Task<ActionResult> EditQuery(int queryId)
        {
            return GetViewEditResponse(queryId, false);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.QueryManagement)]
        public Task<ActionResult> ViewQuery(int queryId)
        {
            return GetViewEditResponse(queryId, true);
        }

        private async Task<ActionResult> GetViewEditResponse(int queryId, bool readOnly)
        {
            var result = await queryManagementBLL.GetListById(queryId).ConfigureAwait(false);
            ViewBag.IsReadOnly = readOnly;

            return PartialView("QueryHistoryModal", result.FirstOrDefault());
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.QueryManagement)]
        public async Task<ActionResult> Create(QueryDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            await queryManagementBLL.Create(dataIn, userCookieData.Id).ConfigureAwait(false);

            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.QueryManagement)]
        public async Task<ActionResult> Edit([FromBody] List<QueryDataIn> dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            await queryManagementBLL.Update(dataIn, userCookieData.Id).ConfigureAwait(false);

            return StatusCode(StatusCodes.Status201Created);
        }

        private void SetViewBags()
        {
            ViewBag.Reasons = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.QueryReason);
            ViewBag.Statuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.QueryStatus);
        }
    }
}