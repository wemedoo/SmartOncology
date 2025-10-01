using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using System;
using System.Threading.Tasks;

namespace sReportsV2.Controllers
{
    public class PatientQueryController : BaseController
    {
        private readonly IFormInstanceBLL formInstanceBLL;
        public PatientQueryController(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService,
            IFormInstanceBLL formInstanceBLL) :
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.formInstanceBLL = formInstanceBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public async Task<ActionResult> GetPatientQuery(PatientQueryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.FilterData = dataIn;
            ViewBag.ReadOnly = false;
            dataIn.InitialLoad = true;
            return View(await formInstanceBLL.GetPatientSemanticResult(dataIn, userCookieData).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public async Task<ActionResult> GetPatientSemanticQuery(PatientQueryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.CustomHeaders = CustomHeaderFieldDataOut.GetDefaultHeaders();
            dataIn.InitialLoad = false;
            return PartialView("PatientSemanticQueryResult", await formInstanceBLL.GetPatientSemanticResult(dataIn, userCookieData).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public async Task<ActionResult> GetAutocompleteData(AutocompleteDataIn dataIn)
        {
            var result = await formInstanceBLL.GetDataForAutocomplete(dataIn).ConfigureAwait(false);
            return Json(result);
        }
    }
}