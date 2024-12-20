﻿using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class MedicationReplacementDAL : IMedicationReplacementDAL
    {
        private readonly SReportsContext context;

        public MedicationReplacementDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void InsertInBatch(List<MedicationReplacement> medicationReplacements)
        {
            foreach(var medicationReplacement in medicationReplacements)
            {
                context.MedicationReplacements.Add(medicationReplacement);
            }
            context.SaveChanges();
        }

        public IQueryable<MedicationReplacement> GetByMedication(int medicationInstanceId)
        {
            return context.MedicationReplacements
                .WhereEntriesAreActive()
                .Where(x => x.ReplaceMedicationId == medicationInstanceId || x.ReplaceWithMedicationId == medicationInstanceId);
        }

        public bool DoesMedicationParticipateInAnyReplacment(int medicationInstanceId)
        {
            return context.MedicationReplacements
                .Any(x => x.ReplaceMedicationId == medicationInstanceId || x.ReplaceWithMedicationId == medicationInstanceId);
        }
    }
}
