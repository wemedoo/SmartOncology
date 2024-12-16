namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFkCodeStatusToEncounter : DbMigration
    {
        public override void Up()
        {
            string updateEncounterFks = @"ALTER TABLE [Encounters] ALTER COLUMN [StatusCD] INT NULL;
                                        ALTER TABLE [Encounters] ALTER COLUMN [ClassCD] INT NULL;
	                                    ALTER TABLE [Encounters] ALTER COLUMN [TypeCD] INT NULL";

            string removeEncounterIdentifiers = @"DELETE ei
                                                FROM dbo.EncounterIdentifiers ei
                                                LEFT JOIN dbo.Encounters e ON e.EncounterId = ei.EncounterId
                                                WHERE e.PatientId is null;";

            string procedeedMessageLogs = @"DELETE logs
                                FROM dbo.[ProcedeedMessageLogs] logs
                                LEFT JOIN dbo.Encounters e ON e.EncounterId = logs.EncounterId
                                WHERE e.PatientId is null;";

            string removeEncounters = @"delete from dbo.Encounters where PatientId is null";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(updateEncounterFks);
            sReportsContext.Database.ExecuteSqlCommand(removeEncounterIdentifiers);
            sReportsContext.Database.ExecuteSqlCommand(procedeedMessageLogs);
            sReportsContext.Database.ExecuteSqlCommand(removeEncounters);

            CreateIndex("dbo.Encounters", "StatusCD");
            AddForeignKey("dbo.Encounters", "StatusCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Encounters", "StatusCD", "dbo.Codes");
            DropIndex("dbo.Encounters", new[] { "StatusCD" });
        }
    }
}
