using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.Patient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;

namespace sReportsV2.Controllers
{
    public class DigitalGuidelineInstanceController : BaseController
    {
        private readonly IDigitalGuidelineInstanceBLL digitalGuidelineInstanceBLL;
        private readonly IDigitalGuidelineBLL digitalGuidelineBLL;
        private readonly IFormInstanceBLL formInstanceBLL;
         
        public DigitalGuidelineInstanceController(IFormInstanceBLL formInstanceBLL, 
            IDigitalGuidelineInstanceBLL digitalGuidelineInstanceBLL, 
            IDigitalGuidelineBLL digitalGuidelineBLL,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.formInstanceBLL = formInstanceBLL;
            this.digitalGuidelineInstanceBLL = digitalGuidelineInstanceBLL;
            this.digitalGuidelineBLL = digitalGuidelineBLL;
        }

        [SReportsAuthorize]
        public ActionResult GuidelineInstance(int episodeOfCareId)
        {
            PatientDataOut data = digitalGuidelineInstanceBLL.GetGuidelineInstance(episodeOfCareId);
            ViewBag.EocId = episodeOfCareId;
            return View("GuidelineInstance", data);
        }

        [SReportsAuthorize]
        public ActionResult LoadGraph(string guidelineInstanceId, string guidelineId)
        {
            ViewBag.GuidelineInstanceId = guidelineInstanceId;
            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, guidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }

        [SReportsAuthorize]
        public ActionResult GuidelineInstanceTable(int episodeOfCareId)
        {
            List<GuidelineInstanceDataOut> data = digitalGuidelineInstanceBLL.GetGuidelineInstancesByEOC(episodeOfCareId);
            return PartialView(data);
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(GuidelineInstanceDataIn guidelineInstance)
        {
            digitalGuidelineInstanceBLL.InsertOrUpdate(guidelineInstance);
            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string guidelineInstanceId)
        {
            digitalGuidelineInstanceBLL.Delete(guidelineInstanceId);

            return NoContent();
        }

        public ActionResult ListDigitalGuidelines(int? episodeOfCareId)
        {
            if (episodeOfCareId == null)
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, "Please choose episode of care!");
            }

            return PartialView(digitalGuidelineInstanceBLL.ListDigitalGuidelines(episodeOfCareId, userCookieData));
        }

        public ActionResult FilterDigitalGuidelines(string title)
        {
            return PartialView("SelectOptionRows", digitalGuidelineBLL.SearchByTitle(title, userCookieData));
        }

        public ActionResult ListGuidelineDocuments(int episodeOfCareId)
        {
            GuidelineInstanceViewDataOut data = digitalGuidelineInstanceBLL.ListDigitalGuidelineDocuments(episodeOfCareId, userCookieData);
            return PartialView(data);
        }

        public ActionResult FilterGuidelineDocuments(int episodeOfCareId, string title)
        {
            List<AutocompleteOptionDataOut> data = formInstanceBLL.SearchByTitle(episodeOfCareId, title, userCookieData);
            return PartialView("SelectOptionRows", data);
        }

        [HttpPost]
        public ActionResult PreviewInstanceNode(GuidelineElementDataDataIn dataIn, string guidelineInstanceId, string guidelineId)
        {
            GuidelineElementDataDataOut data = digitalGuidelineInstanceBLL.PreviewInstanceNode(dataIn);
            ViewBag.NodeGuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeGuidelineId = guidelineId;
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult PreviewInstanceDecisionNode(GuidelineElementDataDataIn dataIn, string guidelineInstanceId, string guidelineId)
        {
            GuidelineElementDataDataOut data = digitalGuidelineInstanceBLL.PreviewInstanceNode(dataIn);
            ViewBag.NodeGuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeGuidelineId = guidelineId;
            return PartialView(data);
        }

        [SReportsAuthorize]
        public string GetValueFromDocument(string formInstanceId, int thesaurusId)
        {
            return digitalGuidelineInstanceBLL.GetValueFromDocument(formInstanceId, thesaurusId);
        }

        [SReportsAuthorize]
        public ActionResult MarksAsCompleted(string value, string nodeId, string guidelineInstanceId, string guidelineId)
        {
            digitalGuidelineInstanceBLL.MarksAsCompleted(value, nodeId, guidelineInstanceId);
            ViewBag.GuidelineInstanceId = guidelineInstanceId;

            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, guidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }

        [SReportsAuthorize]
        public ActionResult GetConditions(string nodeId, string digitalGuidelineId, string guidelineInstanceId)
        {
            ViewBag.GuidelineId = digitalGuidelineId;
            ViewBag.GuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeId = nodeId;
            return PartialView(digitalGuidelineInstanceBLL.GetConditions(nodeId, digitalGuidelineId));
        }

        [SReportsAuthorize]
        public ActionResult SaveCondition(string condition, string nodeId, string guidelineInstanceId, string digitalGuidelineId)
        {
            digitalGuidelineInstanceBLL.SaveCondition(condition, nodeId, guidelineInstanceId, digitalGuidelineId);
            ViewBag.GuidelineInstanceId = guidelineInstanceId;

            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, digitalGuidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }
    }
}