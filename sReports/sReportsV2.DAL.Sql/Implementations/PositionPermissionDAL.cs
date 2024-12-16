using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PositionPermissionDAL : IPositionPermissionDAL
    {
        private readonly SReportsContext context;
        private readonly IConfiguration configuration;

        public PositionPermissionDAL(SReportsContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public int Count()
        {
            return context.PositionPermissions
                .WhereEntriesAreActive()
                .Count();
        }

        public void InsertMany(List<PositionPermission> positionPermissions)
        {
            DataTable positionPermissionsTable = new DataTable();
            positionPermissionsTable.Columns.Add(new DataColumn("EntityStateCD", typeof(int)));
            positionPermissionsTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTimeOffset)));
            positionPermissionsTable.Columns.Add(new DataColumn("ActiveFrom", typeof(DateTimeOffset)));
            positionPermissionsTable.Columns.Add(new DataColumn("ActiveTo", typeof(DateTimeOffset)));
            positionPermissionsTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTimeOffset)));
            positionPermissionsTable.Columns.Add(new DataColumn("PositionCD", typeof(int)));
            positionPermissionsTable.Columns.Add(new DataColumn("PermissionModuleId", typeof(int)));
            positionPermissionsTable.Columns.Add(new DataColumn("CreatedById", typeof(int)));

            foreach (var positionPermission in positionPermissions)
            {
                DataRow positionPermissionRow = positionPermissionsTable.NewRow();
                positionPermissionRow["EntityStateCD"] = (int)EntityStateCode.Active;
                positionPermissionRow["EntryDatetime"] = positionPermission.EntryDatetime;
                positionPermissionRow["ActiveFrom"] = positionPermission.ActiveFrom;
                positionPermissionRow["ActiveTo"] = positionPermission.ActiveTo;
                positionPermissionRow["LastUpdate"] = DBNull.Value;
                positionPermissionRow["PositionCD"] = positionPermission.PositionCD;
                positionPermissionRow["PermissionModuleId"] = positionPermission.PermissionModuleId;
                positionPermissionRow["CreatedById"] = DBNull.Value;
                positionPermissionsTable.Rows.Add(positionPermissionRow);
            }


            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "PositionPermissions"
            };
            objbulk.ColumnMappings.Add("EntityStateCD", "EntityStateCD");
            objbulk.ColumnMappings.Add("EntryDateTime", "EntryDatetime");
            objbulk.ColumnMappings.Add("ActiveFrom", "ActiveFrom");
            objbulk.ColumnMappings.Add("ActiveTo", "ActiveTo");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");
            objbulk.ColumnMappings.Add("PositionCD", "PositionCD");
            objbulk.ColumnMappings.Add("PermissionModuleId", "PermissionModuleId");
            objbulk.ColumnMappings.Add("CreatedById", "CreatedById");

            con.Open();
            objbulk.WriteToServer(positionPermissionsTable);
            con.Close();
        }

        public List<PositionPermission> GetPermissionsForRole(int positionCD)
        {
            return context.PositionPermissions
                .WhereEntriesAreActive()
                .Where(p => p.PositionCD == positionCD)
                .ToList();
        }

        public void InsertOrUpdate(List<PositionPermission> checkedPermissionModules)
        {
            int? positionCD = checkedPermissionModules.FirstOrDefault()?.PositionCD;
            List<PositionPermission> positionPermissionsFromDb = GetAllPermissionsForRole(positionCD);
            UpdateExistingPermissions(
                positionPermissionsFromDb, 
                checkedPermissionModules
                    .Select(pM => pM.PermissionModuleId)
                    .ToList()
            );
            AddNewPermissions(checkedPermissionModules, positionPermissionsFromDb);
        }

        private void UpdateExistingPermissions(List<PositionPermission> positionPermissionsFromDb, List<int?> checkedPermissionModuleIds)
        {
            foreach (PositionPermission positionPermission in positionPermissionsFromDb)
            {
                if (checkedPermissionModuleIds.Contains(positionPermission.PermissionModuleId.GetValueOrDefault()))
                {
                    positionPermission.UpdatePermission(false);
                }
                else
                {
                    positionPermission.UpdatePermission(true);
                }
            }
            context.SaveChanges();
        }

        private void AddNewPermissions(List<PositionPermission> checkedPermissionModules, List<PositionPermission> positionPermissionsFromDb)
        {
            List<PositionPermission> positionPermissionsToAdd = new List<PositionPermission>();
            foreach (PositionPermission checkedPermissionModule in checkedPermissionModules)
            {
                if (!positionPermissionsFromDb.Exists(p => p.PermissionModuleId == checkedPermissionModule.PermissionModuleId))
                {
                    positionPermissionsToAdd.Add(checkedPermissionModule);
                }
            }
            InsertMany(positionPermissionsToAdd);
        }

        private List<PositionPermission> GetAllPermissionsForRole(int? positionCD)
        {
            return context.PositionPermissions.Where(p => p.PositionCD == positionCD).ToList();
        }
    }
}
