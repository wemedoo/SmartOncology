using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ChemotherapySchemaInstanceDAL : IChemotherapySchemaInstanceDAL
    {
        private readonly SReportsContext context;

        public ChemotherapySchemaInstanceDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            ChemotherapySchemaInstance fromDb = context.ChemotherapySchemaInstances.FirstOrDefault(x => x.ChemotherapySchemaInstanceId == id);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        public ChemotherapySchemaInstance GetById(int id)
        {
            return context.ChemotherapySchemaInstances
                .Include(x => x.ChemotherapySchema)
                .Include(x => x.Patient)
                .Include(x => x.Patient.PatientChemotherapyData)
                .Include(x => x.Medications)
                .Include("Medications.MedicationDoses")
                .Include("Medications.MedicationDoses.MedicationDoseTimes")
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceId == id);
        }

        public ChemotherapySchemaInstance GetSchemaInstance(int id)
        {
            return context.ChemotherapySchemaInstances
                .Include(x => x.ChemotherapySchema)
                .Include(x => x.Patient)
                .Include(x => x.Patient.PatientChemotherapyData)
                .Include(x => x.ChemotherapySchemaInstanceHistory)
                .Include(x => x.MedicationReplacements)
                .Include(x => x.Medications)
                .Include("Medications.Medication")
                .Include("Medications.Medication.Unit")
                .Include("Medications.Medication.MedicationDoses")
                .Include("Medications.Medication.MedicationDoses.Unit")
                .Include("Medications.Medication.MedicationDoses.MedicationDoseTimes")
                .Include("Medications.MedicationDoses")
                .Include("Medications.MedicationDoses.MedicationDoseTimes")
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceId == id);
        }

        public void InsertOrUpdate(ChemotherapySchemaInstance chemotherapySchemaInstance)
        {
            if (chemotherapySchemaInstance.ChemotherapySchemaInstanceId == 0)
            {
                context.ChemotherapySchemaInstances.Add(chemotherapySchemaInstance);
            }
            else
            {
                context.UpdateEntryMetadata(chemotherapySchemaInstance, setEntityState: false);
            }
            context.SaveChanges();
        }

        public List<ChemotherapySchemaInstance> GetAll(ChemotherapySchemaInstanceFilter chemotherapySchemaInstanceFilter)
        {
            IQueryable<ChemotherapySchemaInstance> result = GetChemotherapySchemaInstancesFiltered(chemotherapySchemaInstanceFilter);
            result = result.OrderByDescending(x => x.EntryDatetime)
                .Skip((chemotherapySchemaInstanceFilter.Page - 1) * chemotherapySchemaInstanceFilter.PageSize)
                .Take(chemotherapySchemaInstanceFilter.PageSize);

            return result.ToList();
        }

        public long GetAllFilteredCount(ChemotherapySchemaInstanceFilter chemotherapySchemaInstanceFilter)
        {
            return GetChemotherapySchemaInstancesFiltered(chemotherapySchemaInstanceFilter).Count();
        }

        public byte[] GetRowVersion(int id)
        {
            return context.ChemotherapySchemaInstances
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceId == id).RowVersion;
        }

        private IQueryable<ChemotherapySchemaInstance> GetChemotherapySchemaInstancesFiltered(ChemotherapySchemaInstanceFilter chemotherapySchemaInstanceFilter)
        {
            IQueryable<ChemotherapySchemaInstance> query = this.context.ChemotherapySchemaInstances
                .Include(x => x.ChemotherapySchema)
                .Include(x => x.Creator)
                .WhereEntriesAreActive()
                .Where(sh => sh.PatientId == chemotherapySchemaInstanceFilter.PatientId
                && (chemotherapySchemaInstanceFilter.Name == null || sh.ChemotherapySchema.Name.ToLower().Contains(chemotherapySchemaInstanceFilter.Name.ToLower()))
                && (chemotherapySchemaInstanceFilter.CreatedBy == null || sh.Creator.FirstName.ToLower().Contains(chemotherapySchemaInstanceFilter.CreatedBy))
                && (!chemotherapySchemaInstanceFilter.StateCD.HasValue || sh.StateCD == chemotherapySchemaInstanceFilter.StateCD)
                );

            return query;
        }

        public string GetName(int id)
        {
            return context.ChemotherapySchemaInstances
                .Include(x => x.ChemotherapySchema)
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceId == id).ChemotherapySchema.Name;
        }
    }
}
