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
    public class UnitDAL : IUnitDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public UnitDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IQueryable<Unit> FilterByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return context.Units;
            }
            return context.Units.Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        public List<Unit> GetAll()
        {
            return context.Units.ToList();
        }

        public int GetAllCount()
        {
            return context.Units.Count();
        }

        public Unit GetById(int id)
        {
            return context.Units.FirstOrDefault(u => u.UnitId == id);
        }

        public int? GetByNameId(string name)
        {
            return context.Units.FirstOrDefault(u => u.Name.Equals(name))?.UnitId;
        }

        public void InsertMany(List<Unit> units)
        {
            DataTable unitTable = new DataTable();
            unitTable.Columns.Add(new DataColumn("Name", typeof(string)));
            unitTable.Columns.Add(new DataColumn("Description", typeof(string)));

            foreach (var unit in units)
            {
                DataRow unitRow = unitTable.NewRow();
                unitRow["Name"] = unit.Name;
                unitRow["Description"] = unit.Description;
                unitTable.Rows.Add(unitRow);
            }


            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "Units"
            };
            objbulk.ColumnMappings.Add("Name", "Name");
            objbulk.ColumnMappings.Add("Description", "Description");

            con.Open();
            objbulk.WriteToServer(unitTable);
            con.Close();
        }
    }
}
