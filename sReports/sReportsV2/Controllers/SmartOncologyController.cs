using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.HistorySheet.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Linq;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Common.DTO;
using AutoMapper;
using sReportsV2.Common.Entities.User;
using System.Threading.Tasks;

namespace sReportsV2.Controllers
{
    public class SmartOncologyController : BaseController
    {
        private readonly IPatientBLL patientBLL;
        private readonly IChemotherapySchemaBLL chemotherapySchemaBLL;
        private readonly IChemotherapySchemaInstanceBLL chemotherapySchemaInstanceBLL;
        private readonly ITrialManagementBLL trialManagementBLL;
        private readonly IMapper mapper;

        public SmartOncologyController(IChemotherapySchemaBLL chemotherapySchemaBLL, IChemotherapySchemaInstanceBLL chemotherapySchemaInstanceBLL, ITrialManagementBLL trialManagementBLL, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IPatientBLL patientBLL, IMapper mapper, IAsyncRunner asyncRunner) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.chemotherapySchemaBLL = chemotherapySchemaBLL;
            this.chemotherapySchemaInstanceBLL = chemotherapySchemaInstanceBLL;
            this.trialManagementBLL = trialManagementBLL;
            this.patientBLL = patientBLL;
            this.mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult HistorySheet(int? patientId)
        {
            var patient = patientId != null ? patientBLL.GetById(patientId.Value, loadClinicalTrials: true) : null;
            return View(new HistorySheetDataOut() { Patient = patient });
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult PatientOncologyDiseaseHistory(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            var data = patientBLL.GetPreviewById(dataIn.PatientId);
            ViewBag.FilterData = dataIn;
            SetSmartOncologyCodeSetsToViewBag();
            return View("Patient/OncologyDiseasesHistory", data);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public async Task<ActionResult> BasicPatientDataOncology(PatientFilterDataIn filterDataIn)
        {
            return await GetPatients(filterDataIn, false).ConfigureAwait(false);
        }

        public async Task<ActionResult> ReloadPatientTable(PatientFilterDataIn filterDataIn)
        {
            return await GetPatients(filterDataIn, true).ConfigureAwait(false);
        }

        public ActionResult CreatePatientData()
        {
            SetFormEnumViewbags();
            SetGenderTypesToViewBag();
            SetSmartOncologyCodeSetsToViewBag();
            return PartialView("Patient/EditPatientData");
        }

        public ActionResult ViewPatientData(int id)
        {
            var data = patientBLL.GetById(id, loadClinicalTrials: true);
            SetGenderTypesToViewBag();
            SetSmartOncologyCodeSetsToViewBag();
            return PartialView("Patient/ViewPatientData", data);
        }

        public ActionResult EditPatientData(int id)
        {
            var data = patientBLL.GetById(id, loadClinicalTrials: true);
            SetFormEnumViewbags();
            SetGenderTypesToViewBag();
            SetSmartOncologyCodeSetsToViewBag();
            return PartialView("Patient/EditPatientData", data);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ClinicalOncology)]
        [HttpPost]
        public ActionResult EditPatientData(PatientDataIn patientDataIn)
        {
            patientDataIn = Ensure.IsNotNull(patientDataIn, nameof(patientDataIn));
            patientDataIn.PatientChemotherapyData.PatientInformedBy = string.Concat(userCookieData.FirstName, " ", userCookieData.LastName);
            ResourceCreatedDTO resourceCreatedDTO = patientBLL.InsertOrUpdate(patientDataIn, mapper.Map<UserData>(userCookieData));

            return Json(resourceCreatedDTO);
        }

        public ActionResult ReloadClinicalTrial(string name)
        {
            var data = trialManagementBLL.GetlClinicalTrialsByName(name);
            return PartialView("Patient/ClinicalTrialValues", data);
        }

        public ActionResult GetAutocompleteEnumData(SmartOncologyEnumAutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = SingletonDataContainer.Instance.GetSmartOncologyEnums(dataIn.EnumType)
                .Where(e => e.Name.ToLower().Contains(dataIn.Term.ToLower()));
            var enumDataOuts = filtered
                .OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.Name,
                    text = e.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList()
                ;

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = enumDataOuts
            };

            return Json(result);
        }

        public ActionResult GetAutocompletePatientData(AutocompleteDataIn dataIn)
        {
            AutocompleteResultDataOut result = patientBLL.GetAutocompletePatientData(dataIn, userCookieData);

            return Json(result);
        }


        public ActionResult GetAutocompleteSchemaData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetDataForAutocomplete(dataIn);

            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ClinicalOncology)]
        public ActionResult CreateNewSchema()
        {
            return View("SchemaDefinition/EditSchema");
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ClinicalOncology)]
        public ActionResult EditSchema(int id)
        {
            var data = chemotherapySchemaBLL.GetById(id);
            return View("SchemaDefinition/EditSchema", data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult EditSchema(ChemotherapySchemaDataIn chemotherapySchemaDataIn)
        {
            var data = chemotherapySchemaBLL.InsertOrUpdate(chemotherapySchemaDataIn, userCookieData);
            return Json(data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaGeneralProperties(EditGeneralPropertiesDataIn chemotherapySchemaDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateGeneralProperties(chemotherapySchemaDataIn, userCookieData);
            return Json(data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaIndications(EditIndicationsDataIn indicationsDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateIndications(indicationsDataIn, userCookieData);
            return Json(data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaName(EditNameDataIn nameDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateName(nameDataIn, userCookieData);
            return Json(data);
        }

        public ActionResult GetSchemaReference(int id)
        {
            var data = chemotherapySchemaBLL.GetReference(id);
            return PartialView("SchemaDefinition/EditReferenceModal", data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaReference(EditLiteratureReferenceDataIn referenceDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateReference(referenceDataIn, userCookieData);
            return Json(data);
        }

        public ActionResult GetSchemaMedication(int id)
        {
            var data = chemotherapySchemaBLL.GetMedication(id);
            ViewBag.BodyCalculationFormulas = chemotherapySchemaBLL.GetFormulas();
            ViewBag.SchemaDefinitionMedication = true;
            return PartialView("SchemaDefinition/EditMedication", data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaMedication(MedicationDataIn medicationDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedication(medicationDataIn);
            return Json(data);
        }

        public ActionResult EditMedicationDoseContent(MedicationDoseDataOut dataIn)
        {
            ViewBag.MedicationDoseTypes = chemotherapySchemaBLL.GetMedicationDoseTypes();
            return PartialView("SchemaDefinition/EditMedicationDoseContent", dataIn);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDose(MedicationDoseDataIn medicationDoseDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedicationDose(medicationDoseDataIn);
            return Json(data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDoseInBatch(EditMedicationDoseInBatchDataIn editMedicationDoseInBatchDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedicationDoseInBatch(editMedicationDoseInBatchDataIn);
            return Json(data);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationDose(EditMedicationDoseDataIn dataIn)
        {
            chemotherapySchemaBLL.DeleteDose(dataIn);
            return NoContent();
        }

        public ActionResult EditMedicationDoseTimes(int interval)
        {
            var data = chemotherapySchemaBLL.GetMedicationDoseType(interval);
            return PartialView("SchemaDefinition/EditMedicationDoseTimes", data.IntervalsList);
        }

        public ActionResult GetAutocompleteRouteOfAdministrationData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetRouteOfAdministrationDataForAutocomplete(dataIn);
            return Json(result);
        }

        public ActionResult GetRouteOfAdministration(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                RouteOfAdministrationDTO routeOfAdministration = chemotherapySchemaBLL.GetRouteOfAdministration(id.Value);
                return Json(new { id = routeOfAdministration.Id, text = routeOfAdministration.Name });
            }
            else
            {
                return Json(new { });
            }

        }

        public ActionResult GetAutocompleteUnitData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetUnitDataForAutocomplete(dataIn);
            return Json(result);
        }

        public ActionResult GetUnit(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                UnitDTO unit = chemotherapySchemaBLL.GetUnit(id.Value);
                return Json(new { id = unit.Id, text = unit.Name });
            }
            else
            {
                return Json(new { });
            }

        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult PreviewSchema(int id)
        {
            var data = chemotherapySchemaBLL.GetById(id);
            return View("SchemaDefinition/PreviewSchema", data);
        }

        [Authorize]
        public ActionResult BrowseSchemas(ChemotherapySchemaFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View("SchemaDefinition/GetAll");
        }

        public ActionResult ReloadSchemas(ChemotherapySchemaFilterDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.ReloadTable(dataIn);
            return PartialView("SchemaDefinition/SchemaDefinitionEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.ClinicalOncology)]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteSchema(int id)
        {
            chemotherapySchemaBLL.Delete(id);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult ProgressNote(int? schemaInstanceId)
        {
            SetSmartOncologyCodeSetsToViewBag();
            var data = schemaInstanceId.HasValue ? chemotherapySchemaInstanceBLL.GetSchemaInstance(schemaInstanceId.Value) : new ChemotherapySchemaInstanceDataOut();
            return View("ProgressNote/ProgressNote", data);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult ViewPatientDataProgressNote(int? id, string viewDisplayType)
        {
            SetSmartOncologyCodeSetsToViewBag();
            var data = id != null ? patientBLL.GetById(id.Value, loadClinicalTrials: true) : null;
            return PartialView("ProgressNote/ViewPatientData" + viewDisplayType, new ChemotherapySchemaInstanceDataOut() { Patient = data });
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult ViewSchema(int id, DateTime? schemaStartDate, string viewDisplayType)
        {
            var schema = chemotherapySchemaBLL.GetSchemaDefinition(id, schemaStartDate);
            ViewBag.ViewDisplayType = viewDisplayType;

            return PartialView("ProgressNote/ViewSchemaData", schema);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult ViewSchemaInstance(int id)
        {
            var schemaInstance = chemotherapySchemaInstanceBLL.GetById(id);

            return Json(schemaInstance);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ClinicalOncology)]
        public ActionResult ViewSchemaInstanceTableData(int id)
        {
            var data = chemotherapySchemaInstanceBLL.GetSchemaTableData(id);

            return PartialView("ProgressNote/ViewSchemaData", data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult EditSchemaInstance(ChemotherapySchemaInstanceDataIn chemotherapySchemaInstanceDataIn)
        {
            var data = chemotherapySchemaInstanceBLL.InsertOrUpdate(chemotherapySchemaInstanceDataIn, userCookieData);
            return Json(data);
        }

        public ActionResult GetSchemaMedicationInstance(int id)
        {
            var data = chemotherapySchemaBLL.GetMedication(id);
            ViewBag.BodyCalculationFormulas = chemotherapySchemaBLL.GetFormulas();
            ViewBag.SchemaDefinitionMedication = false;
            return PartialView("SchemaDefinition/EditMedication", data);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationInstance(MedicationInstanceDataIn medicationDataIn)
        {
            medicationDataIn = Ensure.IsNotNull(medicationDataIn, nameof(medicationDataIn));
            chemotherapySchemaInstanceBLL.UpdateMedicationInstance(medicationDataIn, userCookieData);
            return ViewSchemaInstanceTableData(medicationDataIn.ChemotherapySchemaInstanceId);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationInstance(DeleteMedicationInstanceDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            chemotherapySchemaInstanceBLL.DeleteMedicationInstance(dataIn, userCookieData);
            return ViewSchemaInstanceTableData(dataIn.ChemotherapySchemaInstanceId);
        }

        public ActionResult GetAutoCompleteMedicationData(MedicationInstanceAutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var data = chemotherapySchemaInstanceBLL.GetMedicationInstanceDataForAutocomplete(dataIn);

            return Json(data);
        }

        public ActionResult ViewMedicationReplacements(int medicationInstanceId)
        {
            var data = chemotherapySchemaInstanceBLL.GetReplacementHistoryForMedication(medicationInstanceId);
            return PartialView("SchemaInstance/ViewMedicationReplacementContent", data);
        }

        public ActionResult EditMedicationDoseInstanceContent(MedicationDoseInstanceDataOut dataIn)
        {
            ViewBag.MedicationDoseTypes = chemotherapySchemaBLL.GetMedicationDoseTypes();
            return PartialView("SchemaDefinition/EditMedicationDoseContent", dataIn);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDoseInstance(MedicationDoseInstanceDataIn medicationDoseInstanceDataIn)
        {
            var data = chemotherapySchemaInstanceBLL.UpdateMedicationDoseInstance(medicationDoseInstanceDataIn, userCookieData);
            return Json(data);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationDoseInstance(EditMedicationDoseDataIn dataIn)
        {
            chemotherapySchemaInstanceBLL.DeleteDose(dataIn, userCookieData);
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteSchemaInstance(int id)
        {
            chemotherapySchemaInstanceBLL.Delete(id);
            return NoContent();
        }

        public ActionResult ReloadSchemaInstances(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            SetSmartOncologyCodeSetsToViewBag();
            var result = chemotherapySchemaInstanceBLL.ReloadTable(dataIn);
            return PartialView("SchemaInstance/SchemaInstanceEntryTable", result);
        }

        public ActionResult DelayDose(DelayDoseDataIn dataIn)
        {
            var data = chemotherapySchemaInstanceBLL.DelayDose(dataIn, userCookieData);

            return PartialView("ProgressNote/ViewSchemaData", data);
        }

        public ActionResult ViewHistoryOfDayDose(DelayDoseHistoryDataIn dataIn)
        {
            var data = chemotherapySchemaInstanceBLL.ViewHistoryOfDayDose(dataIn);

            return PartialView("SchemaInstance/ViewHistoryOfDayDoseContent", data);
        }

        public ActionResult ParseChemotherapySchemaV2()
        {
            chemotherapySchemaBLL.ParseExcelDataAndInsert(userCookieData.Id);


            return Json(null);
        }

        private void SetFormEnumViewbags()
        {
            ViewBag.PresentationState = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.PresentationStage);
            ViewBag.Anatomy = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.Anatomy);
            ViewBag.Morphology = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.Morphology);
            ViewBag.TherapeuticContext = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.TherapeuticContext);
            ViewBag.ChemotherapyType = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.ChemotherapyType);
        }

        private void SetGenderTypesToViewBag()
        {
            ViewBag.Genders = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
        }

        private void SetSmartOncologyCodeSetsToViewBag()
        {
            ViewBag.Contraceptions = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Contraception);
            ViewBag.DiseaseContexts = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.DiseaseContext);
            ViewBag.InstanceStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.InstanceState);
        }

        private async Task<ActionResult> GetPatients(PatientFilterDataIn filterDataIn, bool isPartialView)
        {
            filterDataIn = Ensure.IsNotNull(filterDataIn, nameof(filterDataIn));
            filterDataIn.OrganizationId = userCookieData.ActiveOrganization;
            filterDataIn.SimpleNameSearch = true;
            PaginationDataOut<PatientTableDataOut, PatientFilterDataIn> result = await patientBLL.GetAllFilteredAsync<PatientTableDataOut>(filterDataIn).ConfigureAwait(false);
            SetGenderTypesToViewBag();
            if (isPartialView)
            {
                return PartialView("Patient/PatientEntriesTable", result);
            }
            else
            {
                return View("Patient/BasicPatientDataOncology", result);
            }
        }
    }
}