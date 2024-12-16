using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.AdministrationApi.DataIn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.Controllers
{
    public class AdministrationApiController : BaseController
    {
        private readonly IAdministrationApiBLL administrationApiBLL;

        public AdministrationApiController(IAdministrationApiBLL administrationApiBLL, IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) :
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.administrationApiBLL = administrationApiBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public async Task<ActionResult> GetAll(AdministrationApiFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            SetAdministrationApiViewBag();
            return View();
        }

        [SReportsAuthorize]
        public async Task<ActionResult> ReloadTable(AdministrationApiFilterDataIn dataIn)
        {
            var result = await administrationApiBLL.ReloadTable(dataIn).ConfigureAwait(false);
            SetAdministrationApiViewBag();
            return PartialView("AdministrationApiEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public async Task<ActionResult> ViewLog(int apiRequestLogId)
        {
            var result = await administrationApiBLL.ViewLog(apiRequestLogId).ConfigureAwait(false);
            SetAdministrationApiViewBag();
            return View("AdministrationApiContent", result);
        }

        private void SetAdministrationApiViewBag()
        {
            ViewBag.HttpStatusCodes = new Dictionary<short, string>
            {
                { 0, "Unknown" },
                { StatusCodes.Status200OK, "OK" },
                { StatusCodes.Status201Created, "Created/Updated" },
                { StatusCodes.Status400BadRequest, "Bad Request" },
                { StatusCodes.Status401Unauthorized, "Unauthorized" },
                { StatusCodes.Status500InternalServerError, "Internal Server Error" }
            };
        }
    }
}