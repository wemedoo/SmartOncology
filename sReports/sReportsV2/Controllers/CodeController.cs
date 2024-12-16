using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.CodeAliases.DataIn;
using sReportsV2.DTOs.DTOs.CodeAliases.DataOut;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataIn;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataOut;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn;
using sReportsV2.DTOs.CodeEntry.DataIn;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Net;
using sReportsV2.Common.Enums;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class CodeController : BaseController
    {
        private readonly ICodeBLL codeBLL;
        private readonly ICodeSetBLL codeSetBLL;
        private readonly ICodeAliasBLL aliasBLL;
        private readonly ICodeAssociationBLL codeAssociationBLL;

        public CodeController(
               ICodeBLL codeBLL,
               ICodeSetBLL codeSetBLL,
               ICodeAliasBLL aliasBLL,
               ICodeAssociationBLL codeAssociationBLL,
               IHttpContextAccessor httpContextAccessor,
               IServiceProvider serviceProvider, 
               IConfiguration configuration, 
               IAsyncRunner asyncRunner
           ) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.codeBLL = codeBLL;
            this.codeSetBLL = codeSetBLL;
            this.aliasBLL = aliasBLL;
            this.codeAssociationBLL = codeAssociationBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.CodeSet)]
        public ActionResult GetAll(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            ViewBag.FilterData = dataIn;
            ViewBag.ReadOnly = false;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.CodeSet)]
        public ActionResult ViewCodes(CodeFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            ViewBag.ReadOnly = true;
            return View("GetAll");
        }

        public ActionResult ReloadTable(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.FilterData = dataIn;
            ViewBag.ReadOnly = dataIn.IsReadOnly;
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            PaginationDataOut<CodeDataOut, DataIn> result = codeBLL.GetAllFiltered(dataIn);

            return PartialView("CodeEntryTable", result);
        }

        public ActionResult ReloadCodeSetGroup(int codeSetId, bool canChangeCodeSet)
        {
            var result = codeSetBLL.GetById(codeSetId);
            if (canChangeCodeSet)
                ViewBag.CodeSetDisabled = "";
            else
                ViewBag.CodeSetDisabled = "disabled";

            return PartialView("CodeSetsGroup", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> CreateCodeSet(CodeSetDataIn dataIn)
        {
            await codeSetBLL.InsertAsync(dataIn).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateCodeEntry, Module = ModuleNames.CodeSet)]
        public ActionResult Create(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.CodeDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeDisplay);
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            ViewBag.FilterData = dataIn;
            ViewBag.IsEdit = false;
            SetCommunicationSystemsToViewBag();
            return View(EndpointConstants.Edit);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateCode, Module = ModuleNames.CodeSet)]
        public ActionResult Edit(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.CodeDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeDisplay);
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            ViewBag.FilterData = dataIn;
            ViewBag.IsEdit = true;
            var result = codeBLL.GetById(dataIn.CodeId);
            SetCommunicationSystemsToViewBag();
            return View(EndpointConstants.Edit, result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateCodeEntry, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(CodeDataIn dataIn)
        {
            return CreateOrEdit(dataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateCode, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Edit(CodeDataIn dataIn)
        {
            return CreateOrEdit(dataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.DeleteCode, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            codeBLL.Delete(id);
            RefreshCache(id, ModifiedResourceType.Code);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewAlias, Module = ModuleNames.CodeSet)]
        public ActionResult ReloadAliasTable(CodeAliasFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.CodeDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeDisplay);
            ViewBag.FilterAliasData = dataIn;
            PaginationDataOut<CodeAliasViewDataOut, DataIn> result = aliasBLL.GetAllFiltered(dataIn);

            return PartialView("AliasEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateAlias, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult CreateAlias(CodeAliasDataIn dataIn)
        {
            return CreateOrEditAlias(dataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateAlias, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult EditAlias(CodeAliasDataIn dataIn)
        {
            return CreateOrEditAlias(dataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.DeleteAlias, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult DeleteAlias(int inboundAliasId, int outboundAliasId)
        {
            aliasBLL.DeleteAlias(inboundAliasId, outboundAliasId);
            RefreshCache(inboundAliasId, ModifiedResourceType.Alias);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewAssociation, Module = ModuleNames.CodeSet)]
        public ActionResult ReloadNominatorTable(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            PaginationDataOut<CodeDataOut, DataIn> result = codeBLL.GetAllAssociationsFiltered(dataIn);

            return PartialView("NominatorCodeAssociationTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewAssociation, Module = ModuleNames.CodeSet)]
        public ActionResult ReloadNomineeTable(CodeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            PaginationDataOut<CodeDataOut, DataIn> result = codeBLL.GetAllAssociationsFiltered(dataIn);

            return PartialView("NomineeCodeAssociationTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateAssociation, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult CreateCodeAssociation(List<CodeAssociationDataIn> associations)
        {
            codeAssociationBLL.Insert(associations);
            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewAssociation, Module = ModuleNames.CodeSet)]
        public ActionResult ReloadAssociationTable(CodeAssociationFilterDataIn dataIn, string tableId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.TableId = tableId;
            ViewBag.IsChildToParent = dataIn.IsChildToParent;
            ViewBag.ReadOnly = dataIn.IsReadOnly;
            PaginationDataOut<CodeAssociationDataOut, DataIn> result = codeAssociationBLL.GetAllFiltered(dataIn);

            return PartialView("CodeAssociationTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.DeleteAssociation, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult DeleteCodeAssociation(int associationId)
        {
            codeAssociationBLL.Delete(associationId);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateAssociation, Module = ModuleNames.CodeSet)]
        public ActionResult IsValidAssociation(int parentId, int childId)
        {
            return Json(codeAssociationBLL.ExistAssociation(parentId, childId));
        }

        private void SetCommunicationSystemsToViewBag()
        {
            ViewBag.CommunicationSystems = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.CommunicationSystem);
        }

        private JsonResult CreateOrEdit(CodeDataIn dataIn)
        {
            int result = codeBLL.Insert(dataIn);
            RefreshCache(result, ModifiedResourceType.Code);
            return Json(new { codeId = result });
        }

        private ActionResult CreateOrEditAlias(CodeAliasDataIn dataIn) 
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            int aliasId = aliasBLL.InsertAliases(dataIn);
            //Added for purpose of parsing incoming HL7 messages 
            RefreshCache(aliasId, ModifiedResourceType.Alias);
            return StatusCode(StatusCodes.Status201Created);
        }

        public ActionResult GetChildCodes(int parentId, int codeSetId)
        {
            List<int> associationChildIds = codeAssociationBLL.GetByParentId(parentId);
            List<CodeDataOut> codeDataOuts = new List<CodeDataOut>();

            if (associationChildIds.Count > 0)
                codeDataOuts = codeBLL.GetAssociatedCodes(associationChildIds);
            else 
                if (codeSetId != 0)
                    codeDataOuts = SingletonDataContainer.Instance.GetCodesByCodeSetId(codeSetId);

            return Json(codeDataOuts);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewAlias, Module = ModuleNames.CodeSet)]
        public ActionResult ShowAliasModal(int aliasId)
        {
            CodeAliasViewDataOut aliasData = aliasBLL.GetById(aliasId) ?? new CodeAliasViewDataOut();
            SetCommunicationSystemsToViewBag();
            return PartialView("AliasModal", aliasData);
        }
    }
}