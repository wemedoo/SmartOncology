using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ChemotherapySchemaInstanceHistoryDAL : IChemotherapySchemaInstanceHistoryDAL
    {
        private readonly SReportsContext context;

        public ChemotherapySchemaInstanceHistoryDAL(SReportsContext context)
        {
            this.context = context;
        }

        public ChemotherapySchemaInstanceVersion GetById(int id)
        {
            return context.ChemotherapySchemaInstanceVersions
                .FirstOrDefault(x => x.ChemotherapySchemaInstanceVersionId == id);
        }

        public List<ChemotherapySchemaInstanceVersion> ViewHistoryOfDayDose(int chemotherapySchemaId, int dayNumber, int actionTypeCD)
        {
            return context.ChemotherapySchemaInstanceVersions
                .Include(x => x.Creator)
                .WhereEntriesAreActive()
                .Where(x => x.ChemotherapySchemaInstanceId == chemotherapySchemaId && dayNumber >= x.FirstDelayDay && x.ActionTypeCD == actionTypeCD)
                .ToList();
        }

        public void InsertOrUpdate(ChemotherapySchemaInstanceVersion chemotherapySchemaInstanceHistory)
        {
            if (chemotherapySchemaInstanceHistory.ChemotherapySchemaInstanceVersionId == 0)
            {
                context.ChemotherapySchemaInstanceVersions.Add(chemotherapySchemaInstanceHistory);
            }
            else
            {
                chemotherapySchemaInstanceHistory.SetLastUpdate();
            }
            context.SaveChanges();
        }
    }
}
