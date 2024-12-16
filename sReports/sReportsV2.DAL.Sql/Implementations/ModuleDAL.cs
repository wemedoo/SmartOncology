using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ModuleDAL : IModuleDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public ModuleDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public int Count()
        {
            return context.Modules.Count();
        }

        public List<Module> GetAll()
        {
            return context
            .Modules
            .Include(m => m.Permissions)
                .ThenInclude(pm => pm.Permission) 
            .ToList();
        }

        public Module GetByName(string name)
        {
            return context.Modules.FirstOrDefault(m => m.Name.Equals(name));
        }

        public void InsertMany(List<Module> modules)
        {
            DataTable moduleTable = new DataTable();
            moduleTable.Columns.Add(new DataColumn("Name", typeof(string)));
            moduleTable.Columns.Add(new DataColumn("Description", typeof(string)));

            foreach (var module in modules)
            {
                DataRow codeRow = moduleTable.NewRow();
                codeRow["Name"] = module.Name;
                codeRow["Description"] = module.Description;

                moduleTable.Rows.Add(codeRow);
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "Modules";
            objbulk.ColumnMappings.Add("Name", "Name");
            objbulk.ColumnMappings.Add("Description", "Description");

            con.Open();
            objbulk.WriteToServer(moduleTable);
            con.Close();
        }
    }
}
