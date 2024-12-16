namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEncounterView : DbMigration
    {
        public override void Up()
        {
            string script =
                   @"CREATE OR ALTER VIEW dbo.EncounterViews
                    AS
					select
					encounters.EncounterId
					,patients.NameGiven
					,patients.NameFamily
					,patients.GenderCD
					,patients.BirthDate
					,patients.PatientId
				    ,encounters.AdmitDatetime
				    ,encounters.DischargeDatetime
                    ,encounters.EntityStateCD
                    ,encounters.[RowVersion]
					,encounters.[EntryDatetime]
					,encounters.[LastUpdate]
					,encounters.[ActiveFrom]
					,encounters.[ActiveTo]
					,encounters.[CreatedById]
					from dbo.Encounters encounters
					left join dbo.Patients patients
					on encounters.PatientId = patients.PatientId
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(script);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EncounterViews", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.EncounterViews", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.EncounterViews", new[] { "EntityStateCD" });
            DropIndex("dbo.EncounterViews", new[] { "CreatedById" });
            DropTable("dbo.EncounterViews");
        }
    }
}
