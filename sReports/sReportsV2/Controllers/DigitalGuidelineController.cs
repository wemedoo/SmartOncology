using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class DigitalGuidelineController : BaseController
    {
        private readonly IDigitalGuidelineBLL digitalGuidelineBLL;

        public DigitalGuidelineController(IDigitalGuidelineBLL digitalGuidelineBLL, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.digitalGuidelineBLL = digitalGuidelineBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalPathway)]
        [SReportsAuditLog]
        public ActionResult GetAll(GuidelineFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;

            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalPathway)]
        public ActionResult ReloadTable(GuidelineFilterDataIn dataIn)
        {
            return PartialView("DigitalGuidelineTable", digitalGuidelineBLL.GetAll(dataIn));
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ClinicalPathway)]
        public async Task<ActionResult> Edit(string id)
        {
            return View("Index", await digitalGuidelineBLL.GetById(id).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ClinicalPathway)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Create([ModelBinder(typeof(JsonNetModelBinder))]GuidelineDataIn dataIn) 
        {
            ResourceCreatedDTO resourceCreatedDTO =  await digitalGuidelineBLL.InsertOrUpdate(dataIn).ConfigureAwait(false);
            return Json(resourceCreatedDTO);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ClinicalPathway)]
        [HttpPost]
        public ActionResult PreviewNode(GuidelineElementDataDataIn dataIn)
        {            
            return PartialView(digitalGuidelineBLL.PreviewNode(dataIn));
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ClinicalPathway)]
        public ActionResult Create()
        {
            return View("Index");
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.ClinicalPathway)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult Delete(string id, DateTime lastUpdate)
        {
            digitalGuidelineBLL.Delete(id, lastUpdate);
            return NoContent();
        }
    }
}