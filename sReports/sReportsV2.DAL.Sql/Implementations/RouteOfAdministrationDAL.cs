using Microsoft.Extensions.Configuration;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class RouteOfAdministrationDAL : IRouteOfAdministrationDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public RouteOfAdministrationDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public int GetAllCount()
        {
            return context.RouteOfAdministrations.Count();
        }

        public void InsertMany(List<RouteOfAdministration> bodySurfaceCalculationFormulas)
        {
            DataTable bodySurfaceCalculationFormulaRowTable = new DataTable();
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Name", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("Definition", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("ShortName", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("FDACode", typeof(string)));
            bodySurfaceCalculationFormulaRowTable.Columns.Add(new DataColumn("NCICondeptId", typeof(string)));

            foreach (var bodySurfaceCalculationFormula in bodySurfaceCalculationFormulas)
            {
                DataRow bodySurfaceCalculationFormulaRow = bodySurfaceCalculationFormulaRowTable.NewRow();
                bodySurfaceCalculationFormulaRow["Name"] = bodySurfaceCalculationFormula.Name;
                bodySurfaceCalculationFormulaRow["Definition"] = bodySurfaceCalculationFormula.Definition;
                bodySurfaceCalculationFormulaRow["ShortName"] = bodySurfaceCalculationFormula.ShortName;
                bodySurfaceCalculationFormulaRow["FDACode"] = bodySurfaceCalculationFormula.FDACode;
                bodySurfaceCalculationFormulaRow["NCICondeptId"] = bodySurfaceCalculationFormula.NCICondeptId;

                bodySurfaceCalculationFormulaRowTable.Rows.Add(bodySurfaceCalculationFormulaRow);
            }


            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "RouteOfAdministrations"
            };
            objbulk.ColumnMappings.Add("Name", "Name");
            objbulk.ColumnMappings.Add("Definition", "Definition");
            objbulk.ColumnMappings.Add("ShortName", "ShortName");
            objbulk.ColumnMappings.Add("FDACode", "FDACode");
            objbulk.ColumnMappings.Add("NCICondeptId", "NCICondeptId");

            con.Open();
            objbulk.WriteToServer(bodySurfaceCalculationFormulaRowTable);
            con.Close();
        }
        public IQueryable<RouteOfAdministration> FilterByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return context.RouteOfAdministrations;
            }
            return context.RouteOfAdministrations.Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        public RouteOfAdministration GetById(int id)
        {
            return context.RouteOfAdministrations.FirstOrDefault(r => r.RouteOfAdministrationId == id);
        }
    }
}
