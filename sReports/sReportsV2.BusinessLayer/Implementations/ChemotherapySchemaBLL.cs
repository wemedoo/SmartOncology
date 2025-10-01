using AutoMapper;
using ExcelImporter.Importers;
using Microsoft.EntityFrameworkCore;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ChemotherapySchemaBLL : IChemotherapySchemaBLL
    {
        private readonly IChemotherapySchemaDAL chemotherapySchemaDAL;
        private readonly ILiteratureReferenceDAL literatureReferenceDAL;
        private readonly IMedicationDAL medicationDAL;
        private readonly IMedicationDoseDAL medicationDoseDAL;
        private readonly IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL;
        private readonly IRouteOfAdministrationDAL routeOfAdministrationDAL;
        private readonly IMedicationDoseTypeDAL medicationDoseTypeDAL;
        private readonly IPersonnelDAL userDAL;
        private readonly IUnitDAL unitDAL;
        private readonly IMapper mapper;

        public ChemotherapySchemaBLL(IChemotherapySchemaDAL chemotherapySchemaDAL, ILiteratureReferenceDAL literatureReferenceDAL, IMedicationDAL medicationDAL, IMedicationDoseDAL medicationDoseDAL, IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL, IRouteOfAdministrationDAL routeOfAdministrationDAL, IMedicationDoseTypeDAL medicationDoseTypeDAL, IPersonnelDAL userDAL, IUnitDAL unitDAL, IMapper mapper)
        {
            this.chemotherapySchemaDAL = chemotherapySchemaDAL;
            this.literatureReferenceDAL = literatureReferenceDAL;
            this.medicationDAL = medicationDAL;
            this.medicationDoseDAL = medicationDoseDAL;
            this.bodySurfaceCalculationFormulaDAL = bodySurfaceCalculationFormulaDAL;
            this.routeOfAdministrationDAL = routeOfAdministrationDAL;
            this.medicationDoseTypeDAL = medicationDoseTypeDAL;
            this.userDAL = userDAL;
            this.unitDAL = unitDAL;
            this.mapper = mapper;
        }

        public ChemotherapySchemaDataOut GetById(int id)
        {
            ChemotherapySchema chemotherapySchema = chemotherapySchemaDAL.GetById(id);
            if (chemotherapySchema == null) throw new ArgumentNullException(nameof(id));

            ChemotherapySchemaDataOut chemotherapySchemaDataOut = mapper.Map<ChemotherapySchemaDataOut>(chemotherapySchema);
            SetRouteOfAdministrationForMedications(chemotherapySchemaDataOut);
            return chemotherapySchemaDataOut;
        }

        public ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchema chemotherapySchema = mapper.Map<ChemotherapySchema>(dataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(dataIn.Id);

            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.Copy(chemotherapySchema);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ResourceCreatedDTO UpdateGeneralProperties(EditGeneralPropertiesDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ChemotherapySchema chemotherapySchema = mapper.Map<ChemotherapySchema>(dataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(dataIn.Id);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }

            CopySchemaProperties(chemotherapySchemaDb, chemotherapySchema);
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ResourceCreatedDTO UpdateName(EditNameDataIn nameDataIn, UserCookieData userCookieData)
        {
            nameDataIn = Ensure.IsNotNull(nameDataIn, nameof(nameDataIn));
            ChemotherapySchema chemotherapySchema = mapper.Map<ChemotherapySchema>(nameDataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(nameDataIn.Id);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.CopyRowVersion(chemotherapySchema);
                chemotherapySchemaDb.CopyName(chemotherapySchema.Name);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return new ResourceCreatedDTO() { 
                Id = chemotherapySchemaDb.ChemotherapySchemaId.ToString(),
                RowVersion = Convert.ToBase64String(chemotherapySchemaDb.RowVersion)
            };
        }

        public ChemotherapySchemaDataOut UpdateIndications(EditIndicationsDataIn indicationsDataIn, UserCookieData userCookieData)
        {
            ChemotherapySchema chemotherapySchema = mapper.Map<ChemotherapySchema>(indicationsDataIn);
            ChemotherapySchema chemotherapySchemaDb = chemotherapySchemaDAL.GetById(indicationsDataIn.ChemotherapySchemaId);
            if (chemotherapySchemaDb == null)
            {
                chemotherapySchemaDb = chemotherapySchema;
                SetChemotherapySchemaCreator(chemotherapySchemaDb, userCookieData);
            }
            else
            {
                chemotherapySchemaDb.CopyRowVersion(chemotherapySchema);
                chemotherapySchemaDb.CopyEntries(chemotherapySchema.Indications);
            }
            InsertOrUpdate(chemotherapySchemaDb);

            return mapper.Map<ChemotherapySchemaDataOut>(chemotherapySchemaDb);
        }

        public ChemotherapySchemaResourceCreatedDTO UpdateReference(EditLiteratureReferenceDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            IsUniquePMID(dataIn.LiteratureReference.PubMedID, dataIn.LiteratureReference.Id);

            LiteratureReference literatureReference = mapper.Map<LiteratureReference>(dataIn.LiteratureReference);
            LiteratureReference literatureReferenceDb = literatureReferenceDAL.GetById(literatureReference.LiteratureReferenceId);

            if (literatureReferenceDb == null)
            {
                literatureReferenceDb = literatureReference;
            }
            else
            {
                literatureReferenceDb.Copy(literatureReference);
            }

            int chemotherapySchemaId;
            int referenceId;
            string rowVersion;
            if (IsSchemaAlreadyCreated(literatureReferenceDb.ChemotherapySchemaId))
            {
                literatureReferenceDAL.InsertOrUpdate(literatureReferenceDb);
                referenceId = literatureReferenceDb.LiteratureReferenceId;
                chemotherapySchemaId = literatureReferenceDb.ChemotherapySchemaId;
                rowVersion = dataIn.RowVersion;
                IsConcurrencyViolated(chemotherapySchemaId, rowVersion);
            }
            else
            {
                ChemotherapySchema newChemotherapySchema = new ChemotherapySchema(userCookieData.Id) { 
                    LiteratureReferences = new List<LiteratureReference>() { literatureReferenceDb }
                };
                SetChemotherapySchemaCreator(newChemotherapySchema, userCookieData);
                InsertOrUpdate(newChemotherapySchema);

                chemotherapySchemaId = newChemotherapySchema.ChemotherapySchemaId;
                referenceId = newChemotherapySchema.LiteratureReferences.First().LiteratureReferenceId;
                rowVersion = Convert.ToBase64String(newChemotherapySchema.RowVersion);
            }

            return new ChemotherapySchemaResourceCreatedDTO() { ParentId = chemotherapySchemaId, Id = referenceId.ToString(), RowVersion = rowVersion};
        }

        public MedicationDataOut GetMedication(int id)
        {
            Medication medication = medicationDAL.GetById(id);

            MedicationDataOut medicationDataOut = mapper.Map<MedicationDataOut>(medication);
            return medicationDataOut;
        }

        public ResourceCreatedDTO UpdateMedication(MedicationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            Medication medication = mapper.Map<Medication>(dataIn);
            Medication medicationDb = medicationDAL.GetById(dataIn.Id);

            if (medicationDb == null)
            {
                medicationDb = medication;
            }
            else
            {
                medicationDb.Copy(medication);
            }

            InsertOrUpdateMedication(medicationDb);

            return new ResourceCreatedDTO() { Id = medicationDb.MedicationId.ToString(), RowVersion = Convert.ToBase64String(medicationDb.RowVersion) };
        }

        public ResourceCreatedDTO UpdateMedicationDose(MedicationDoseDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            MedicationDose medicationDose = mapper.Map<MedicationDose>(dataIn);
            MedicationDose medicationDoseDb = medicationDoseDAL.GetById(dataIn.Id);

            if (medicationDoseDb == null)
            {
                medicationDoseDb = medicationDose;
            }
            else
            {
                medicationDoseDb.Copy(medicationDose);
            }

            IsConcurrencyMedicationViolated(dataIn.MedicationId, dataIn.RowVersion);
            medicationDoseDAL.InsertOrUpdate(medicationDoseDb);

            return new ResourceCreatedDTO() { Id = medicationDoseDb.MedicationDoseId.ToString(), RowVersion = dataIn.RowVersion };
        }

        public EditMedicationDoseInBatchDataOut UpdateMedicationDoseInBatch(EditMedicationDoseInBatchDataIn dataIn)
        {
            Medication medication = mapper.Map<Medication>(dataIn);
            Medication medicationDb = medicationDAL.GetById(medication.MedicationId);

            List<MedicationDose> upcomingMedicationDoses = medicationDb.GetPremedicationsDays();
            List<MedicationDose> medicationDayDoses = medication.MedicationDoses;
            upcomingMedicationDoses.AddRange(medicationDayDoses);

            medicationDb.CopyEntries(upcomingMedicationDoses);
            medicationDb.CopyRowVersion(medication);

            InsertOrUpdateMedication(medicationDb);

            Dictionary<string, int> idsPerDays = medicationDb.GetMedicationDays().ToDictionary(x => x.DayNumber.ToString(), x => x.MedicationDoseId);
            return new EditMedicationDoseInBatchDataOut { IdsPerDays = idsPerDays, RowVersion = Convert.ToBase64String(medicationDb.RowVersion) };
        }

        public void DeleteDose(EditMedicationDoseDataIn dataIn)
        {
            IsConcurrencyMedicationViolated(dataIn.MedicationId, dataIn.RowVersion);
            medicationDoseDAL.Delete(dataIn.Id);
        }

        public LiteratureReferenceDataOut GetReference(int id)
        {
            LiteratureReference literatureReference = literatureReferenceDAL.GetById(id);
            return mapper.Map<LiteratureReferenceDataOut>(literatureReference);
        }

        public List<BodySurfaceCalculationFormulaDTO> GetFormulas()
        {
            return mapper.Map<List<BodySurfaceCalculationFormulaDTO>>(bodySurfaceCalculationFormulaDAL.GetAll());
        }

        public AutocompleteResultDataOut GetRouteOfAdministrationDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            IQueryable<RouteOfAdministration> filtered = routeOfAdministrationDAL.FilterByName(dataIn.Term);
            List<AutocompleteDataOut> routeOfAdministrationDataOuts = filtered.OrderBy(x => x.Name)
                .Skip(dataIn.GetHowManyElementsToSkip())
                .Take(FilterConstants.DefaultPageSize)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.RouteOfAdministrationId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filtered.Count())
                },
                results = routeOfAdministrationDataOuts
            };

            return result;
        }

        public RouteOfAdministrationDTO GetRouteOfAdministration(int id)
        {
            return mapper.Map<RouteOfAdministrationDTO>(routeOfAdministrationDAL.GetById(id));
        }

        public AutocompleteResultDataOut GetUnitDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            IQueryable<Unit> filtered = unitDAL.FilterByName(dataIn.Term);
            List<AutocompleteDataOut> routeOfAdministrationDataOuts = filtered.OrderBy(x => x.Name)
                .Skip(dataIn.GetHowManyElementsToSkip())
                .Take(FilterConstants.DefaultPageSize)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.UnitId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filtered.Count())
                },
                results = routeOfAdministrationDataOuts
            };

            return result;
        }

        public UnitDTO GetUnit(int id)
        {
            return mapper.Map<UnitDTO>(unitDAL.GetById(id));
        }

        public List<MedicationPreviewDoseTypeDTO> GetMedicationDoseTypes()
        {
            return mapper.Map<List<MedicationPreviewDoseTypeDTO>>(medicationDoseTypeDAL.GetAll());
        }

        public PaginationDataOut<ChemotherapySchemaDataOut, DataIn> ReloadTable(ChemotherapySchemaFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            ChemotherapySchemaFilter filterData = mapper.Map<ChemotherapySchemaFilter>(dataIn);
            PaginationDataOut<ChemotherapySchemaDataOut, DataIn> result = new PaginationDataOut<ChemotherapySchemaDataOut, DataIn>()
            {
                Count = (int)chemotherapySchemaDAL.GetAllFilteredCount(filterData),
                Data = mapper.Map<List<ChemotherapySchemaDataOut>>(chemotherapySchemaDAL.GetAll(filterData)),
                DataIn = dataIn
            };

            return result;
        }

        public MedicationDoseTypeDTO GetMedicationDoseType(int id)
        {
            return mapper.Map<MedicationDoseTypeDTO>(medicationDoseTypeDAL.GetById(id));
        }

        public AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = chemotherapySchemaDAL.FilterByName(dataIn.Term);
            var enumDataOuts = filtered
                .OrderBy(x => x.Name)
                .Skip(dataIn.GetHowManyElementsToSkip())
                .Take(FilterConstants.DefaultPageSize)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.ChemotherapySchemaId.ToString(),
                    text = e.Name
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
                results = enumDataOuts
            };

            return result;
        }

        public void Delete(int id)
        {
            chemotherapySchemaDAL.Delete(id);
        }

        public SchemaTableDataOut GetSchemaDefinition(int id, DateTime? firstDay, UserCookieData userCookieData)
        {
            ChemotherapySchema chemotherapySchema = chemotherapySchemaDAL.GetSchemaDefinition(id);

            if(chemotherapySchema != null)
            {
                return GetSchemaTableData(chemotherapySchema, firstDay, userCookieData);
            }
            else
            {
                return null;
            }
        }
        public void ParseExcelDataAndInsert(int creatorId)
        {
            SchemaImporterV2 schemaImporterV2 = new SchemaImporterV2("Chemotherapy Compendium Import - 26.11.2021", "Basic Data", chemotherapySchemaDAL, routeOfAdministrationDAL, unitDAL, creatorId);
            schemaImporterV2.ImportDataFromExcelToDatabase();
        }

        private bool IsSchemaAlreadyCreated(int schemaId)
        {
            return schemaId > 0;
        }

        private void SetRouteOfAdministrationForMedications(ChemotherapySchemaDataOut chemotherapySchema)
        {
            foreach(MedicationPreviewDataOut medication in chemotherapySchema.Medications)
            {
                if(int.TryParse(medication.RouteOfAdministration, out int routeOfAdministrationId))
                {
                    RouteOfAdministration routeOfAdministration = routeOfAdministrationDAL.GetById(routeOfAdministrationId);
                    medication.RouteOfAdministrationDTO = mapper.Map<RouteOfAdministrationDTO>(routeOfAdministration);
                }
            }
        }

        private void SetChemotherapySchemaCreator(ChemotherapySchema chemotherapySchema, UserCookieData userCookieData)
        {
            chemotherapySchema.Creator = userDAL.GetById(userCookieData.Id);
        }

        private SchemaTableDataOut GetSchemaTableData(ChemotherapySchema chemotherapySchema, DateTime? firstDay, UserCookieData userCookieData)
        {
            SchemaTableDataOut schemaTableData = new SchemaTableDataOut
            {
                FirstDay = firstDay ?? DateTimeExtension.GetCurrentDateTime(userCookieData.OrganizationTimeZoneIana),
                Medications = chemotherapySchema.Medications.Select(i => new SchemaTableMedicationInstanceDataOut() {
                    Medication = mapper.Map<SchemaTableMedicationDataOut>(i)
                }).ToList()
            };
            schemaTableData.SetSchemaDays(true);

            return schemaTableData;
        }

        private void IsConcurrencyViolated(int schemaId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(chemotherapySchemaDAL.GetRowVersion(schemaId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void IsConcurrencyMedicationViolated(int medicationId, string rowVersion)
        {
            string rowVersionDb = Convert.ToBase64String(medicationDAL.GetRowVersion(medicationId));
            if (!rowVersion.Equals(rowVersionDb)) throw new DbUpdateConcurrencyException("Concurrency ex");
        }

        private void InsertOrUpdate(ChemotherapySchema chemotherapySchema)
        {
            try
            {
                chemotherapySchemaDAL.InsertOrUpdate(chemotherapySchema);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException("A concurrency issue occurred while updating the chemotherapy schema.", e);
            }
        }

        private void InsertOrUpdateMedication(Medication medication)
        {
            try
            {
                medicationDAL.InsertOrUpdate(medication);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException("A concurrency issue occurred while updating the medication.", e);
            }
        }

        private void IsUniquePMID(int pmid, int referenceId)
        {
            int? referenceIdWithGivenPMID = literatureReferenceDAL.FindByPMID(pmid);
            if(referenceIdWithGivenPMID.HasValue && referenceIdWithGivenPMID.Value != referenceId)
            {
                throw new DuplicateException($"Literature reference with given PMID ({pmid}) is already defined!");
            }
        }

        private static void CopySchemaProperties(ChemotherapySchema chemotherapySchemaDb, ChemotherapySchema chemotherapySchema)
        {
            chemotherapySchemaDb.CopyRowVersion(chemotherapySchema);
            chemotherapySchemaDb.CopyGeneralProperties(chemotherapySchema);
        }
    }
}
