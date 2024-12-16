namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveEncounterPeriodData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string saveEncounterPeriodData = "update dbo.Encounters set AdmitDatetime = Period_Start, DischargeDatetime = Period_End;";
            context.Database.ExecuteSqlCommand(saveEncounterPeriodData);
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string saveEncounterPeriodData = "update dbo.Encounters set Period_Start = AdmitDatetime, Period_End = DischargeDatetime;";
            context.Database.ExecuteSqlCommand(saveEncounterPeriodData);
        }
    }
}
