using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace sReportsV2.SqlDomain.Implementations
{
    public class AdministrativeDataDAL : IAdministrativeDataDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public AdministrativeDataDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IEnumerable<AdministrativeData> GetAll()
        {
            return context.AdministrativeDatas;
        }

        public void InsertMany(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses)
        {
            DataTable administrativeDataTable = new DataTable();
            administrativeDataTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));

            for (int i = 0; i < thesauruses.Count; i++)
            {
                DataRow translationRow = administrativeDataTable.NewRow();
                translationRow["ThesaurusEntryId"] = bulkedThesauruses[i];

                administrativeDataTable.Rows.Add(translationRow);
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "AdministrativeDatas";
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");

            con.Open();
            objbulk.WriteToServer(administrativeDataTable);
            con.Close();
        }

        public void InsertManyVersions(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses)
        {
            DataTable versionTable = new DataTable();
            versionTable.Columns.Add(new DataColumn("TypeCD", typeof(int)) { AllowDBNull = true });
            versionTable.Columns.Add(new DataColumn("CreatedOn", typeof(DateTimeOffset)));
            versionTable.Columns.Add(new DataColumn("StateCD", typeof(int)) { AllowDBNull = true });
            versionTable.Columns.Add(new DataColumn("AdministrativeDataId", typeof(int)));
            versionTable.Columns.Add(new DataColumn("PersonnelId", typeof(int)));
            versionTable.Columns.Add(new DataColumn("OrganizationId", typeof(int)));

            int i = 0;
            foreach (var thesaurus in thesauruses)
            {
                if(thesaurus.AdministrativeData != null)
                {
                    foreach (var version in thesaurus.AdministrativeData.VersionHistory)
                    {
                        DataRow translationRow = versionTable.NewRow();
                        translationRow["TypeCD"] = version.TypeCD;
                        translationRow["CreatedOn"] = version.CreatedOn.ConvertToOrganizationTimeZone();
                        translationRow["StateCD"] = version.StateCD;
                        translationRow["AdministrativeDataId"] = bulkedThesauruses[i];
                        translationRow["PersonnelId"] = version.PersonnelId;
                        translationRow["OrganizationId"] = version.OrganizationId;

                        versionTable.Rows.Add(translationRow);
                    }
                }
                i++;
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "Versions";
            objbulk.ColumnMappings.Add("TypeCD", "TypeCD");
            objbulk.ColumnMappings.Add("CreatedOn", "CreatedOn");
            objbulk.ColumnMappings.Add("StateCD", "StateCD");
            objbulk.ColumnMappings.Add("AdministrativeDataId", "AdministrativeDataId");
            objbulk.ColumnMappings.Add("PersonnelId", "PersonnelId");
            objbulk.ColumnMappings.Add("OrganizationId", "OrganizationId");

            con.Open();
            objbulk.WriteToServer(versionTable);
            con.Close();
        }

        public void ExecuteCustomSqlCommand(string script)
        {
            if (string.IsNullOrEmpty(script)) return;
            string connectionString = configuration["Sql"];
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(script, sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("Error while executing custom sql command, error: " + ex.Message);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
    }
}
