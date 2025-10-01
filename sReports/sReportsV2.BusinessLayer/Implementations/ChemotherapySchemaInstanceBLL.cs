using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ChemotherapySchemaInstanceBLL : IChemotherapySchemaInstanceBLL
    {
        private readonly IChemotherapySchemaInstanceDAL chemotherapySchemaInstanceDAL;
        private readonly IChemotherapySchemaInstanceHistoryDAL chemotherapySchemaInstanceHistoryDAL;
        private readonly IMedicationInstanceDAL medicationInstanceDAL;
        private readonly IMedicationReplacementDAL medicationReplacementDAL;
        private readonly IMedicationDoseInstanceDAL medicationDoseInstanceDAL;
        private readonly ITrialManagementDAL trialManagementDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IMapper mapper;

        public ChemotherapySchemaInstanceBLL(IChemotherapySchemaInstanceDAL chemotherapySchemaInstanceDAL, IChemotherapySchemaInstanceHistoryDAL chemotherapySchemaInstanceHistoryDAL, IMedicationInstanceDAL medicationInstanceDAL, IMedicationReplacementDAL medicationReplacementDAL, IMedicationDoseInstanceDAL medicationDoseInstanceDAL, ITrialManagementDAL trialManagementDAL, ICodeDAL codeDAL, IMapper mapper)
        {
            this.chemotherapySchemaInstanceDAL = chemotherapySchemaInstanceDAL;
            this.chemotherapySchemaInstanceHistoryDAL = chemotherapySchemaInstanceHistoryDAL;
            this.medicationInstanceDAL = medicationInstanceDAL;
            this.medicationReplacementDAL = medicationReplacementDAL;
            this.medicationDoseInstanceDAL = medicationDoseInstanceDAL;
            this.trialManagementDAL = trialManagementDAL;
            this.codeDAL = codeDAL;
            this.mapper = mapper;
        }

        public void Delete(int id)
        {
            chemotherapySchemaInstanceDAL.Delete(id);
        }

        public ChemotherapySchemaInstanceDataOut GetById(int id)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(id);
            if (chemotherapySchemaInstance == null) throw new ArgumentNullException(nameof(id));

            ChemotherapySchemaInstanceDataOut chemotherapySchemaInstanceDataOut = mapper.Map<ChemotherapySchemaInstanceDataOut>(chemotherapySchemaInstance);
            return chemotherapySchemaInstanceDataOut;
        }

        public ChemotherapySchemaInstanceDataOut GetSchemaInstance(int id, UserCookieData userCookieData)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(id);
            if (chemotherapySchemaInstance == null) throw new ArgumentNullException(nameof(id));

            ChemotherapySchemaInstanceDataOut chemotherapySchemaInstanceDataOut = mapper.Map<ChemotherapySchemaInstanceDataOut>(chemotherapySchemaInstance);
            chemotherapySchemaInstanceDataOut.SchemaTableData = GetSchemaTableData(chemotherapySchemaInstance, userCookieData);
            chemotherapySchemaInstanceDataOut.Patient.ClinicalTrials = mapper.Map<List<ClinicalTrialDataOut>>(trialManagementDAL.GetlClinicalTrialByIds(chemotherapySchemaInstance.Patient?.PatientChemotherapyData?.GetClinicalTrialIds()));
            return chemotherapySchemaInstanceDataOut;
        }

        public ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchemaInstance chemotherapySchemaInstance = mapper.Map<ChemotherapySchemaInstance>(dataIn);
            ChemotherapySchemaInstance chemotherapySchemaInstanceDb = chemotherapySchemaInstanceDAL.GetById(dataIn.Id);

            string actionName;
            if (chemotherapySchemaInstanceDb == null)
            {
                chemotherapySchemaInstanceDb = chemotherapySchemaInstance;
                chemotherapySchemaInstanceDb.StateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.InstanceState, CodeAttributeNames.Active);
                chemotherapySchemaInstance.CreatorId = userCookieData.Id;
                actionName = "created";
            }
            else
            {
                chemotherapySchemaInstanceDb.Copy(chemotherapySchemaInstance);
                actionName = "updated";
            }

            chemotherapySchemaInstanceDAL.InsertOrUpdate(chemotherapySchemaInstanceDb);

            int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.SaveInstance);
            SaveChemotherapySchemaInstanceAction(chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"ChemotherapySchemaInstance [{chemotherapySchemaInstanceDAL.GetName(chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId)}] is {actionName}.");

            return new ResourceCreatedDTO() { Id = chemotherapySchemaInstanceDb.ChemotherapySchemaInstanceId.ToString(), RowVersion = Convert.ToBase64String(chemotherapySchemaInstanceDb.RowVersion) };
        }

        public PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn> ReloadTable(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            ChemotherapySchemaInstanceFilter filterData = mapper.Map<ChemotherapySchemaInstanceFilter>(dataIn);

            List<ChemotherapySchemaInstance> chemotherapySchemasFiltered = chemotherapySchemaInstanceDAL.GetAll(filterData);
            PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn> result = new PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn>()
            {
                Count = (int)chemotherapySchemaInstanceDAL.GetAllFilteredCount(filterData),
                Data = mapper.Map<List<ChemotherapySchemaInstancePreviewDataOut>>(chemotherapySchemasFiltered),
                DataIn = dataIn
            };

            return result;
        }

        public MedicationDoseInstanceDataOut UpdateMedicationDoseInstance(MedicationDoseInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            MedicationDoseInstance medicationDoseInstance = mapper.Map<MedicationDoseInstance>(dataIn);
            MedicationDoseInstance medicationDoseInstanceDb = medicationDoseInstanceDAL.GetById(dataIn.Id);

            string actionName;
            if (medicationDoseInstanceDb == null)
            {
                medicationDoseInstanceDb = medicationDoseInstance;
                actionName = "created";
            }
            else
            {
                medicationDoseInstanceDb.Copy(medicationDoseInstance);
                actionName = "updated";
            }
            IsConcurrencyMedicationInstanceViolated(dataIn.MedicationInstanceId, dataIn.RowVersion);

            medicationDoseInstanceDAL.InsertOrUpdate(medicationDoseInstanceDb);

            int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.SaveDose);
            SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"Medication dose instance (day {medicationDoseInstanceDb.DayNumber}) for {dataIn.MedicationName} is {actionName}.");

            return GetMedicationDoseInstanceDataOut(medicationDoseInstanceDb, dataIn.RowVersion);
        }

        public void DeleteDose(EditMedicationDoseDataIn dataIn, UserCookieData userCookieData)
        {
            IsConcurrencyMedicationInstanceViolated(dataIn.MedicationId, dataIn.RowVersion);
            int doseDeletedId = medicationDoseInstanceDAL.Delete(dataIn.Id);
            if(doseDeletedId > 0)
            {
                int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.DeleteDose);
                SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"Medication dose instance (day {dataIn.DayNumber}) for {dataIn.MedicationName} is deleted.");
            }
        }

        public SchemaTableDataOut DelayDose(DelayDoseDataIn dataIn, UserCookieData userCookieData)
        {
            ChemotherapySchemaInstanceVersion newChemotherapySchemaInstanceVersion = new ChemotherapySchemaInstanceVersion(userCookieData.Id)
            {
                DelayFor = dataIn.DelayFor,
                FirstDelayDay = dataIn.DayNumber,
                ReasonForDelay = dataIn.ReasonForDelay,
            };

            int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.DelayDose);
            SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"Day number {dataIn.DayNumber} is delayed for {dataIn.DelayFor} days. Reason for delay: {dataIn.ReasonForDelay}.", newChemotherapySchemaInstanceVersion);

            return GetSchemaTableData(dataIn.ChemotherapySchemaInstanceId, userCookieData);
        }

        public ChemotherapySchemaInstanceHistoryDataOut ViewHistoryOfDayDose(DelayDoseHistoryDataIn dataIn)
        {
            int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.DelayDose);
            var history = mapper.Map<List<ChemotherapySchemaInstanceVersionDataOut>>(chemotherapySchemaInstanceHistoryDAL.ViewHistoryOfDayDose(dataIn.ChemotherapySchemaInstanceId, dataIn.DayNumber, actionTypeCD));

            var data = new ChemotherapySchemaInstanceHistoryDataOut()
            {
                FirstDelayDay = dataIn.DayNumber,
                StartDayDate = dataIn.StartDate.AddDays(dataIn.DayNumber),
                History = history
            };

            data.CalculateDelayedDateByVersions();

            return data;
        }

        public void DeleteMedicationInstance(DeleteMedicationInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            IsConcurrencyViolated(dataIn.ChemotherapySchemaInstanceId, dataIn.RowVersion);
            IsConcurrencyMedicationInstanceViolated(dataIn.Id, dataIn.RowVersionMedication);
            if (medicationReplacementDAL.DoesMedicationParticipateInAnyReplacment(dataIn.Id))
            {
                throw new DuplicateException($"Medication [{string.Join(",", medicationInstanceDAL.GetNameByIds(new List<int> { dataIn.Id }))}] cannot be deleted because it is used as medication replacment.");
            }
            else
            {
                int medicationDeletedId = medicationInstanceDAL.Delete(dataIn.Id);
                if (medicationDeletedId > 0)
                {
                    int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.DeleteMedication);
                    SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"Medication [{string.Join(",", medicationInstanceDAL.GetNameByIds(new List<int> { dataIn.Id }))}] is deleted from schema instance.");
                }
            }
        }

        public ChemotherapySchemaResourceCreatedDTO UpdateMedicationInstance(MedicationInstanceDataIn dataIn, UserCookieData userCookieData)
        {
            MedicationInstance medicationInstance = mapper.Map<MedicationInstance>(dataIn);
            MedicationInstance medicationInstanceDb = medicationInstanceDAL.GetById(dataIn.Id);

            if (medicationInstanceDb == null)
            {
                medicationInstanceDb = medicationInstance;

                medicationInstanceDAL.InsertOrUpdate(medicationInstance);
                int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.AddMedication);
                SaveChemotherapySchemaInstanceAction(dataIn.ChemotherapySchemaInstanceId, userCookieData.Id, actionTypeCD, $"New medication ({medicationInstanceDb.Medication.Name}) is created and added to the schema instance.");
                SaveMedicationReplacements(medicationInstance, dataIn.MedicationIdsToReplace, userCookieData.Id);
            }

            return new ChemotherapySchemaResourceCreatedDTO() { 
                Id = medicationInstanceDb.MedicationInstanceId.ToString(), 
                RowVersion = Convert.ToBase64String(medicationInstanceDb.RowVersion),
                ParentId = medicationInstanceDb.MedicationId
            };
        }

        public AutocompleteResultDataOut GetMedicationInstanceDataForAutocomplete(MedicationInstanceAutocompleteDataIn dataIn)
        {
            var filtered = medicationInstanceDAL.FilterByNameAndChemotherapySchemaInstanceAndType(dataIn.Term, dataIn.ChemotherapySchemaInstanceId, dataIn.IsSupportiveMedication);

            var medicationInstanceDataOuts = filtered
                .OrderBy(x => x.Medication.Name)
                .Skip(dataIn.GetHowManyElementsToSkip())
                .Take(FilterConstants.DefaultPageSize)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.MedicationInstanceId.ToString(),
                    text = e.Medication.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList()
                ;

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filtered.Count())
                },
                results = medicationInstanceDataOuts
            };

            return result;
        }

        public SchemaTableDataOut GetSchemaTableData(int chemotherapySchemaInstanceId, UserCookieData userCookieData)
        {
            ChemotherapySchemaInstance chemotherapySchemaInstance = chemotherapySchemaInstanceDAL.GetSchemaInstance(chemotherapySchemaInstanceId);
            return GetSchemaTableData(chemotherapySchemaInstance, userCookieData);
        }

        public MedicationReplacementHistoryDataOut GetReplacementHistoryForMedication(int medicationId)
        {
            return new MedicationReplacementHistoryDataOut()
            {
                MedicationInstance = mapper.Map<MedicationInstancePreviewDataOut>(medicationInstanceDAL.GetById(medicationId)),
                MedicationReplacements = mapper.Map<List<MedicationReplacementDataOut>>(medicationReplacementDAL.GetByMedication(medicationId).ToList())
            };
        }

        private void SaveMedicationReplacements(MedicationInstance medicationInstance, List<int> medicationsToReplace, int creatorId)
        {
            if (medicationsToReplace.Count > 0)
            {
                var medicationsReplacements = medicationsToReplace.Select(id => new MedicationReplacement(creatorId)
                {
                    ReplaceMedicationId = id,
                    ReplaceWithMedicationId = medicationInstance.MedicationInstanceId,
                    ChemotherapySchemaInstanceId = medicationInstance.ChemotherapySchemaInstanceId,
                    CreatorId = creatorId
                }).ToList();
                medicationReplacementDAL.InsertInBatch(medicationsReplacements);
                int actionTypeCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeAttributeNames.ReplaceMedication);
                SaveChemotherapySchemaInstanceAction(medicationInstance.ChemotherapySchemaInstanceId, creatorId, actionTypeCD, $"New medication ({medicationInstance.Medication.Name}) is set as an alternative for ({string.Join(",", medicationInstanceDAL.GetNameByIds(medicationsToReplace))}).");
            }
        }

        private SchemaTableDataOut GetSchemaTableData(ChemotherapySchemaInstance chemotherapySchemaInstance, UserCookieData userCookieData)
        {
            SchemaTableDataOut schemaTableData = new SchemaTableDataOut
            {
                Id = chemotherapySchemaInstance.ChemotherapySchemaInstanceId,
                RowVersion = Convert.ToBase64String(chemotherapySchemaInstance.RowVersion),
                FirstDay = chemotherapySchemaInstance.StartDate ?? DateTimeExtension.GetCurrentDateTime(userCookieData.OrganizationTimeZoneIana),
                Medications = mapper.Map<List<SchemaTableMedicationInstanceDataOut>>(chemotherapySchemaInstance.Medications.Where(x => x.IsActive())),
                History = mapper.Map<List<ChemotherapySchemaInstanceVersionDataOut>>(chemotherapySchemaInstance.ChemotherapySchemaInstanceHistory),
                MedicationReplacements = mapper.Map<List<MedicationReplacementDataOut>>(chemotherapySchemaInstance.MedicationReplacements)
            };
            schemaTableData.SetSchemaDays();

            return schemaTableData;
        }

        private void IsConcurrencyViolated(int schemaId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(chemotherapySchemaInstanceDAL.GetRowVersion(schemaId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void IsConcurrencyMedicationInstanceViolated(int medicationInstanceId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(medicationInstanceDAL.GetRowVersion(medicationInstanceId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private MedicationDoseInstanceDataOut GetMedicationDoseInstanceDataOut(MedicationDoseInstance medicationDose, string rowVersion)
        {
            MedicationDoseInstanceDataOut dataOut = mapper.Map<MedicationDoseInstanceDataOut>(medicationDose);
            dataOut.RowVersion = rowVersion;
            dataOut.StartsAt = medicationDose.GetStartTime();

            return dataOut;
        }

        private void SaveChemotherapySchemaInstanceAction(int chemotherapySchemaInstanceId, int creatorId, int actionTypeCD, string description, ChemotherapySchemaInstanceVersion chemotherapySchemaInstanceVersion = null)
        {
            chemotherapySchemaInstanceVersion = chemotherapySchemaInstanceVersion ?? new ChemotherapySchemaInstanceVersion(creatorId);
            chemotherapySchemaInstanceVersion.ChemotherapySchemaInstanceId = chemotherapySchemaInstanceId;
            chemotherapySchemaInstanceVersion.CreatorId = creatorId;
            chemotherapySchemaInstanceVersion.ActionTypeCD = actionTypeCD;
            chemotherapySchemaInstanceVersion.Description = description;

            chemotherapySchemaInstanceHistoryDAL.InsertOrUpdate(chemotherapySchemaInstanceVersion);
        }
    }
}
