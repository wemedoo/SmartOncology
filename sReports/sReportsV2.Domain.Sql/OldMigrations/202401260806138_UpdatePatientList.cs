namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePatientList : DbMigration
    {
        public override void Up()
        {
            string timezone = TimeZoneConstants.CEST;

            RenameColumn("dbo.Encounters", "AdmitDatetime", "AdmissionDate");
            AlterColumn("dbo.Encounters", "AdmissionDate", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Encounters SET AdmissionDate = (SELECT AdmissionDate AT TIME ZONE '{timezone}')");

            RenameColumn("dbo.Encounters", "DischargeDatetime", "DischargeDate");
            AlterColumn("dbo.Encounters", "DischargeDate", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Encounters SET DischargeDate = (SELECT DischargeDate AT TIME ZONE '{timezone}')");

            AddColumn("dbo.PatientLists", "AdmissionDate", c => c.DateTime());
            AddColumn("dbo.PatientLists", "DischargeDate", c => c.DateTime());

            DropColumn("dbo.PatientLists", "PatientActiveFrom");
            DropColumn("dbo.PatientLists", "PatientActiveTo");

            AddColumn("dbo.PatientLists", "EncounterStatusCD", c => c.Int());
            CreateIndex("dbo.PatientLists", "EncounterStatusCD");
            AddForeignKey("dbo.PatientLists", "EncounterStatusCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Encounters", "DischargeDate", "DischargeDatetime");
            RenameColumn("dbo.Encounters", "AdmissionDate", "AdmissionDatetime");
            AlterColumn("dbo.Encounters", "DischargeDatetime", c => c.DateTime());
            AlterColumn("dbo.Encounters", "AdmitDatetime", c => c.DateTime());
            AddColumn("dbo.PatientLists", "PatientActiveTo", c => c.DateTime());
            AddColumn("dbo.PatientLists", "PatientActiveFrom", c => c.DateTime());
            DropColumn("dbo.PatientLists", "DischargeDate");
            DropColumn("dbo.PatientLists", "AdmissionDate");

            DropForeignKey("dbo.PatientLists", "EncounterStatusCD", "dbo.Codes");
            DropIndex("dbo.PatientLists", new[] { "EncounterStatusCD" });
            DropColumn("dbo.PatientLists", "EncounterStatusCD");
        }
    }
}
