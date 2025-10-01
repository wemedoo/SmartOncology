using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using System;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataIn;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataOut;
using System.Collections.Generic;

namespace sReportsV2.Controllers
{
    public class UploadPatientDataController : BaseController
    {
        private readonly IUploadPatientDataBLL uploadPatientDataBLL;
        public UploadPatientDataController(IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService,
            IUploadPatientDataBLL uploadPatientDataBLL) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.uploadPatientDataBLL = uploadPatientDataBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult GetAll(UploadPatientFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ReloadTable(UploadPatientFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<UploadPatientDataOut, UploadPatientFilterDataIn> result = await uploadPatientDataBLL.ReloadData(dataIn).ConfigureAwait(false);
            
            return PartialView("UploadPatientDataEntryTable", result);
        }

        /// TODO: Enable auditog for multiple files
        //[SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> UploadPatientData([FromForm] IFormFileCollection files, string domain)
        {
            if (files != null && files.Count > 0)
            {
                await uploadPatientDataBLL.UploadPatientData(files, domain).ConfigureAwait(false);
                return Content("");
            }
            else
            {
                return BadRequest(new ErrorDTO("No file uploaded"));
            }
        }

        public async Task<ActionResult> ProceedLLM(int uploadPatientDataId)
        {
            List<PromptResultDataOut> result = await uploadPatientDataBLL.ProceedLLM(uploadPatientDataId).ConfigureAwait(false);
            return PartialView("UploadPatientDataResult", result);
        }
    }
}
