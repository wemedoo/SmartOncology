using AutoMapper;
using iText.StyledXmlParser.Jsoup.Select;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sReportsV2.Controllers
{
    public partial class FormController
    {

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpPost]
        public async Task<ActionResult> PasteChapters([ModelBinder(typeof(JsonNetModelBinder))] List<FormChapterDataIn> elements, string destinationFormId, string destinationElementId, bool afterDestination)
        {
            return await PasteElements(elements, destinationFormId, destinationElementId, afterDestination).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpPost]
        public async Task<ActionResult> PastePages([ModelBinder(typeof(JsonNetModelBinder))] List<FormPageDataIn> elements, string destinationFormId, string destinationElementId, bool afterDestination)
        {
            return await PasteElements(elements, destinationFormId, destinationElementId, afterDestination).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpPost]

        public async Task<ActionResult> PasteFieldSets([ModelBinder(typeof(JsonNetModelBinder))] List<List<FormFieldSetDataIn>> elements, string destinationFormId, string destinationElementId, bool afterDestination)
        {
            return await PasteElements(elements, destinationFormId, destinationElementId, afterDestination).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        [HttpPost]
        public async Task<ActionResult> PasteFields([ModelBinder(typeof(JsonNetModelBinder))] List<FieldDataIn> elements, string destinationFormId, string destinationElementId, bool afterDestination)
        {
            return await PasteElements(elements, destinationFormId, destinationElementId, afterDestination).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public async Task<ActionResult> ReloadFormTreeNestable(string formId)
        {
            return PartialView("~/Views/Form/DragAndDrop/FormTreeNestable.cshtml", GetCurrentFormDataOut(formId));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public async Task<ActionResult> ReloadFormPreviewContainer(string formId)
        {
            SetFormDragAndDropPartialViewBags(false);
            return PartialView("~/Views/Form/DragAndDrop/DragAndDropFormPartial.cshtml", GetCurrentFormDataOut(formId));
        }

        private async Task<ActionResult> PasteElements<T> (List<T> elements, string destinationFormId, string destinationElementId, bool afterDestination)
        {
            FormDataOut formDataOut = await formBLL.PasteElements(elements, destinationFormId, destinationElementId, afterDestination, userCookieData).ConfigureAwait(false);
            return Json(new { lastUpdate = System.Net.WebUtility.UrlEncode(formDataOut?.LastUpdate.Value.ToString("o")) });
        }

        private FormDataOut GetCurrentFormDataOut(string formId)
        {
            return formBLL.GetFormDataOutById(formId, userCookieData);
        }
    }
}