using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class CodeSetController : BaseController
    {
        private readonly ICodeSetBLL codeSetBLL;
        public CodeSetController(ICodeSetBLL codeSetBLL, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.codeSetBLL = codeSetBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.CodeSet)]
        public ActionResult GetAll(CodeSetFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.CodeSet)]
        public ActionResult ReloadTable(CodeSetFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            PaginationDataOut<CodeSetDataOut, DataIn> result = codeSetBLL.GetAllFiltered(dataIn);

            return PartialView("CodeSetEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(CodeSetDataIn dataIn)
        {
            codeSetBLL.Insert(dataIn);
            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        [SReportsAuditLog]
        public ActionResult ExistCodeSetId(int codeSetId)
        {
            return Json(codeSetBLL.ExistCodeSet(codeSetId));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        [SReportsAuditLog]
        public ActionResult IsCodeSetAvailableByPreferredTerm(string codesetName)
        {
            return Json(codeSetBLL.ExistCodeSetByPreferredTerm(codesetName) ? TextLanguage.CodeSet_Name_Already_In_Use : "true");
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.CreateCode, Module = ModuleNames.Thesaurus)]
        [HttpPost]
        public ActionResult ImportAsCSV(IFormFile file, string codesetName, bool applicableInDesigner)
        {
            if(codeSetBLL.ImportFileFromCsv(file, codesetName, applicableInDesigner))
                return StatusCode(StatusCodes.Status201Created);
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetAutoCompleteNames(AutocompleteDataIn dataIn, bool onlyApplicableInDesigner)
        {
            return Json(await codeSetBLL.GetAutoCompleteNames(dataIn, onlyApplicableInDesigner, userCookieData.ActiveLanguage).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetCodedCodesetDisplay(int codeSetId)  
        {
            return Json(new { CodeSetDisplay = (codeSetBLL.GetCodedCodeSetDisplay(codeSetId)) });
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetCodesForAutoCompleteByCodeset(AutocompleteDataIn dataIn, int codesetId)
        {
            return Json(
                codeSetBLL.GetAutoCompleteCodes(
                    dataIn,
                    codesetId,
                    userCookieData.ActiveLanguage));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.CodeSet)]
        [HttpGet]
        public ActionResult ReloadCodeSets(CodeSetFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.CodeSetDisplay = System.Net.WebUtility.UrlDecode(dataIn.CodeSetDisplay);
            List<CodeSetDataOut> result = codeSetBLL.GetAllByPreferredTerm(dataIn.CodeSetDisplay);
            return PartialView("~/Views/Code/NomineeAutocomplete.cshtml", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.CodeSet)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            codeSetBLL.Delete(id);
            return NoContent();
        }

    }
}