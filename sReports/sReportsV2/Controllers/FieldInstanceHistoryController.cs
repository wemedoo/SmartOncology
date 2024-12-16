using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn;
using sReportsV2.DTOs.Pagination;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class FieldInstanceHistoryController : BaseController
    {
        private readonly IFormInstanceBLL formInstanceBLL;
        private readonly ICodeAssociationBLL codeAssociationBLL;

        public FieldInstanceHistoryController(IFormInstanceBLL fieldInstanceHistoryBLL, 
            ICodeAssociationBLL codeAssociationBLL,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.formInstanceBLL = fieldInstanceHistoryBLL;
            this.codeAssociationBLL = codeAssociationBLL;
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public async Task<ActionResult> ReloadData(FieldInstanceHistoryFilterDataIn fieldInstanceHistoryFilter)
        {
            PaginationDataOut<FieldInstanceHistoryDataOut, FieldInstanceHistoryFilterDataIn> fieldInstanceHistories = await formInstanceBLL.GetAllFieldHistoriesFiltered(fieldInstanceHistoryFilter).ConfigureAwait(false);
            ViewBag.MissingValues = codeAssociationBLL.InitializeMissingValueList(userCookieData.ActiveLanguage);
            return PartialView("FormInstanceHistory", fieldInstanceHistories);
        }
    }
}