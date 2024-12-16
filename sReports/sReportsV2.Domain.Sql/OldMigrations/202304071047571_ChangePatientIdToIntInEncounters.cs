namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePatientIdToIntInEncounters : DbMigration
    {
        public override void Up()
        {
            string encounterVersioningOff = @"ALTER TABLE [dbo].[Encounters] SET ( SYSTEM_VERSIONING = OFF )";
            string patientIdToIntEncounter = @"ALTER TABLE [Encounters] ALTER COLUMN PatientId INT";
            string patientIdToIntEncounterHistory = @"ALTER TABLE [EncountersHistory] ALTER COLUMN PatientId INT";
            string encounterVersioningOn = @"ALTER TABLE [dbo].[Encounters] SET ( SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].EncountersHistory))";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(encounterVersioningOff);
            sReportsContext.Database.ExecuteSqlCommand(patientIdToIntEncounter);
            sReportsContext.Database.ExecuteSqlCommand(patientIdToIntEncounterHistory);
            sReportsContext.Database.ExecuteSqlCommand(encounterVersioningOn);
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Encounters", "PatientId", c => c.String());
        }
    }
}
