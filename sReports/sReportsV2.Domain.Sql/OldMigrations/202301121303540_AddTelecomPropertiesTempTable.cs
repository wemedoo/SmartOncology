namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTelecomPropertiesTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createTempTable = $@"
                create table dbo.TelecomPropertiesTempTable (
	                TelecomId int, 
	                PatientTelecomId int, 
	                PatientContactTelecomId int, 
	                [System] nvarchar(max), 
                    [Use] nvarchar(max)
                );
            ";

            context.Database.ExecuteSqlCommand(createTempTable);
            context.Database.ExecuteSqlCommand(CreateInsertCommand("Telecoms", "t", "TelecomId"));
            context.Database.ExecuteSqlCommand(CreateInsertCommand("PatientTelecoms", "pt", "PatientTelecomId", customTelecomUse: true));
            context.Database.ExecuteSqlCommand(CreateInsertCommand("PatientContactTelecoms", "pct", "PatientContactTelecomId", customTelecomUse: true));
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropTempTable = $@"
                drop table if exists dbo.TelecomPropertiesTempTable;
            ";
            context.Database.ExecuteSqlCommand(dropTempTable);
        }

        private string CreateInsertCommand(string tableName, string tableAlias, string tableIdName, bool customTelecomUse = false)
        {
            string telecomUse = customTelecomUse ? 
                $@"CASE
				    WHEN {tableAlias}.[Use] = 'Home' THEN 'Primary Residence Number'
				    WHEN {tableAlias}.[Use] = 'Work' THEN 'Work Number'
				    WHEN {tableAlias}.[Use] is NULL THEN ''
				    ELSE 'Other Residence Number'
					    END" 
                : 
                $@"{tableAlias}.[Use]";
            return $@"
                insert into dbo.TelecomPropertiesTempTable ({tableIdName}, [System], [Use]) 
	                select {tableAlias}.{tableIdName}
			                ,{tableAlias}.[System]
			                ,{telecomUse}
                from dbo.{tableName} {tableAlias};
            ";
        }
    }
}
