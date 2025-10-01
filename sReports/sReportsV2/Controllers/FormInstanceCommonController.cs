using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using System;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common.DTO;
using Form = sReportsV2.Domain.Entities.Form.Form;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using sReportsV2.DTOs.FormInstance.DataIn;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController
    {
        protected async Task<ActionResult> DeleteFormInstance(string formInstanceId, DateTime lastUpdate)
        {
            await formInstanceBLL.DeleteAsync(formInstanceId, lastUpdate, userCookieData.Id).ConfigureAwait(false);
            return NoContent();
        }

        protected ActionResult GetEditFormInstance(FormInstanceFilterDataIn filter, string partialViewName = "", string actionUrl = "", bool showUserProjects = false)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            FormInstance formInstance = formInstanceBLL.GetById(filter.FormInstanceId);
            if (formInstance == null)
            {
                return NotFound(TextLanguage.FormInstanceNotExists, filter.FormInstanceId);
            }
            if (IsAccessDeniedInDataCaptureViewMode(formInstance.UserId))
            {
                ViewBag.CustomExplanation = "You are not allowed to open this form instance!";
                return View("~/Views/Error/AccessDenied.cshtml");
            }
            FormDataOut data = formBLL.GetFormDataOut(
                formInstance, 
                userCookieData, 
                new DTOs.DTOs.FormInstance.DataIn.FormInstanceReloadDataIn
                {
                    ActiveChapterId = filter?.ActiveChapterId,
                    ActivePageId = filter?.ActivePageId,
                    ActivePageLeftScroll = filter?.ActivePageLeftScroll
                }
            );

            HttpContext.Session.SaveToCache(formInstance);

            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.Referrals = formInstance.Referrals;
            ViewBag.FormInstanceWorkflowHistory = formInstanceBLL.GetWorkflowHistory(formInstance.WorkflowHistory);
            SetEngineCommonViewBags(filter, showUserProjects);

            if (!string.IsNullOrEmpty(actionUrl))
            {
                ViewBag.Action = actionUrl;
            }

            if (!string.IsNullOrEmpty(partialViewName))
            {
                HttpContext.Session?.UpdateUserCookieDataInSession(true);
                return PartialView(partialViewName, data);
            }
            else
            {
                HttpContext.Session?.UpdateUserCookieDataInSession(false);
                ViewBag.Title = formInstance.Title;
                return View("~/Views/FormInstance/FormInstance.cshtml", data);
            }
        }

        protected async Task<ActionResult> CreateOrEdit(FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formInstanceDataIn.ThesaurusId, userCookieData.ActiveOrganization, formInstanceDataIn.Language, formInstanceDataIn.GetVersionId());

            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn.ThesaurusId.ToString());
            }

            formInstanceDataIn = HttpContext.Session.MergeFromCache(formInstanceDataIn);

            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);

            await formInstanceBLL.InsertOrUpdateAsync(
                formInstance,
                formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id),
                userCookieData
                )
                .ConfigureAwait(false);

            return GetCreateFormInstanceResponseResult(formInstance.Id, form.Version.Id, form.Title);
        }

        protected ActionResult GetCreateFormInstanceResponseResult(string formInstanceId, string formVersionId, string formTitle)
        {
            return Json(new CreateFormResponseResult
            {
                FormInstanceId = formInstanceId,
                FormVersionId = formVersionId,
                FormTitle = formTitle,
                Message = TextLanguage.Form_Instance_Saved
            });
        }

        protected FormDataOut GetDataOutForCreatingNewFormInstance(Form form)
        {
            form.SetFieldInstances(new List<FieldInstance>());
            FormDataOut formOut = formBLL.SetFormDependablesAndReferrals(form);
            formOut.SetActiveChapterAndPageId(null);

            return formOut;
        }

        protected void SetIsHiddenFieldsShown(bool hiddenFieldsShown)
        {
            ViewBag.HiddenFieldsShown = hiddenFieldsShown;
        }

        private bool IsAccessDeniedInDataCaptureViewMode(int formInstanceCreatedById)
        {
            return ViewBag.IsDateCaptureMode && userCookieData.Id != formInstanceCreatedById;
        }

        protected void SetProjectViewBags(FormInstanceFilterDataIn dataIn, bool showUserProjects)
        {
            ViewBag.ProjectId = dataIn.ProjectId;
            if (dataIn.ProjectId != null)
            {
                ViewBag.ProjectTitle = projectManagementBLL.GetNameById(dataIn.ProjectId.Value);
            }
            ViewBag.ShowUserProjects = showUserProjects;
        }

        protected void SetEngineCommonViewBags(FormInstanceFilterDataIn filter, bool showUserProjects)
        {
            ViewBag.IsEngineModule = true;
            ViewBag.NullFlavors = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.NullFlavor);
            ViewBag.NotApplicableId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.NullFlavor, ResourceTypes.NotApplicable);
            SetProjectViewBags(filter, showUserProjects);
        }
    }
}