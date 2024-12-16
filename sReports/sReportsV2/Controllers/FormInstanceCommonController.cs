using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common.DTO;
using Form = sReportsV2.Domain.Entities.Form.Form;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using sReportsV2.DTOs.FormInstance.DataIn;
using Serilog;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController
    {
        protected async Task<ActionResult> DeleteFormInstance(string formInstanceId, DateTime lastUpdate)
        {
            await formInstanceBLL.DeleteAsync(formInstanceId, lastUpdate, userCookieData.Id).ConfigureAwait(false);
            return NoContent();
        }

        protected int GetEncounterFromRequestOrCreateDefault(int episodeOfCareId, int encounterId)
        {
            if (encounterId == 0)
            {
                encounterId = InsertEncounter(episodeOfCareId);
            }

            return encounterId;
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
            List<FormInstance> referrals = this.formInstanceBLL.GetByIds(formInstance.Referrals ?? new List<string>());
            FormDataOut data = formBLL.GetFormDataOut(
                formInstance, 
                referrals, 
                userCookieData, 
                new DTOs.DTOs.FormInstance.DataIn.FormInstanceReloadDataIn
                {
                    ActiveChapterId = filter?.ActiveChapterId,
                    ActivePageId = filter?.ActivePageId,
                    ActivePageLeftScroll = filter?.ActivePageLeftScroll
                }
            );

            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.Referrals = referrals != null && referrals.Count > 0 ? referrals.Select(x => x.Id) : null;
            ViewBag.FormInstanceWorkflowHistory = formInstanceBLL.GetWorkflowHistory(formInstance.WorkflowHistory);
            SetEngineCommonViewBags(filter, showUserProjects);

            if (!string.IsNullOrEmpty(actionUrl))
            {
                ViewBag.Action = actionUrl;
            }

            if (!string.IsNullOrEmpty(partialViewName))
            {
                return PartialView(partialViewName, data);
            }
            else
            {
                ViewBag.Title = formInstance.Title;
                return View("~/Views/FormInstance/FormInstance.cshtml", data);
            }
        }

        protected async Task<ActionResult> CreateOrEdit(FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            string versionId = string.IsNullOrWhiteSpace(formInstanceDataIn.EditVersionId) ? formInstanceDataIn.VersionId : formInstanceDataIn.EditVersionId;
            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formInstanceDataIn.ThesaurusId, userCookieData.ActiveOrganization, formInstanceDataIn.Language, versionId);

            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn.ThesaurusId.ToString());
            }

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

        protected FormDataOut GetDataOutForCreatingNewFormInstance(Form form, List<Form> formReferrals)
        {
            form.SetFieldInstances(new List<FieldInstance>());
            FormDataOut formOut = formBLL.SetFormDependablesAndReferrals(form, formReferrals, userCookieData);
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

        #region PatientRelatedData
        protected int InsertPatient(Patient patient)
        {
            //TO DO FIX THIS FUNCTION
            Patient patientDb = patient == null || patient.PatientIdentifiers == null || patient.PatientIdentifiers.Count <= 0 ?
                patient
                :
                patientDAL.GetByIdentifier(patient.PatientIdentifiers[0]);

            if (patientDb?.PatientId == 0)
            {
                patientDb.CreatedById = userCookieData.Id;
                patientDAL.InsertOrUpdate(patientDb, null);
            }
        
            return patientDb != null ? patientDb.PatientId : 0;
        }

        protected int InsertEpisodeOfCare(int patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate, UserData user)
        {
            startDate = startDate.Date;
            EpisodeOfCare eoc;
            if (episodeOfCare != null)
            {
                eoc = Mapper.Map<EpisodeOfCare>(episodeOfCare);
                eoc.Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.DiagnosisRole = 12227;
                eoc.OrganizationId = 1;
            }
            else
            {
                eoc = Mapper.Map<EpisodeOfCare>(new EpisodeOfCareDataIn()
                    {
                        Description = $"Generated from {source}",
                        DiagnosisRole = 12227,
                        PatientId = patientId,
                        StatusCD = (int)EocStatus.Active,
                        Period = new PeriodDTO() { StartDate = startDate }
                    }
                );
                eoc.OrganizationId = 1;
            }

            return episodeOfCareDAL.InsertOrUpdate(eoc, user);

        }

        protected int InsertEncounter(int episodeOfCareId)
        {
            Encounter encounterEntity = Mapper.Map<Encounter>(new EncounterDataIn()
                {
                    ClassCD = 12246,
                    Period = new PeriodDTO
                    {
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now
                    },
                    StatusCD = 12218,
                    TypeCD = 12208,
                    ServiceTypeCD = 11087
                }
            );
            encounterEntity.EpisodeOfCareId = episodeOfCareId;
            return encounterDAL.InsertOrUpdate(encounterEntity);
        }

        #endregion
    }
}