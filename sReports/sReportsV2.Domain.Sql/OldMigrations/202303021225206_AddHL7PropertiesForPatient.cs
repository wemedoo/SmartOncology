namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHL7PropertiesForPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientContacts", "ContactRoleStartDate", c => c.DateTime());
            AddColumn("dbo.PatientContacts", "ContactRoleEndDate", c => c.DateTime());
            AddColumn("dbo.Patients", "MiddleName", c => c.String(maxLength: 129));
            AddColumn("dbo.Encounters", "AdmitSourceCD", c => c.Int());
            //AddColumn("dbo.Encounters", "ClassCD", c => c.Int());
            //AddColumn("dbo.Encounters", "TypeCD", c => c.Int());
            AddColumn("dbo.Encounters", "AdmitDatetime", c => c.DateTime());
            AddColumn("dbo.Encounters", "DischargeDatetime", c => c.DateTime());
            AddColumn("dbo.Encounters", "AttendingDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "ReferringDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "ConsultingDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "AdmittingDoctorId", c => c.Int());
            CreateIndex("dbo.Encounters", "AdmitSourceCD");
            //CreateIndex("dbo.Encounters", "ClassCD");
            //CreateIndex("dbo.Encounters", "TypeCD");
            CreateIndex("dbo.Encounters", "AttendingDoctorId");
            CreateIndex("dbo.Encounters", "ReferringDoctorId");
            CreateIndex("dbo.Encounters", "ConsultingDoctorId");
            CreateIndex("dbo.Encounters", "AdmittingDoctorId");
            AddForeignKey("dbo.Encounters", "AdmitSourceCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Encounters", "AdmittingDoctorId", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.Encounters", "AttendingDoctorId", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.Encounters", "ConsultingDoctorId", "dbo.Personnel", "UserId");
            //AddForeignKey("dbo.Encounters", "ClassCD", "dbo.Codes", "CodeId");
            //AddForeignKey("dbo.Encounters", "TypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Encounters", "ReferringDoctorId", "dbo.Personnel", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Encounters", "ReferringDoctorId", "dbo.Personnel");
            //DropForeignKey("dbo.Encounters", "TypeCD", "dbo.Codes");
            //DropForeignKey("dbo.Encounters", "ClassCD", "dbo.Codes");
            DropForeignKey("dbo.Encounters", "ConsultingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "AttendingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "AdmittingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "AdmitSourceCD", "dbo.Codes");
            DropIndex("dbo.Encounters", new[] { "AdmittingDoctorId" });
            DropIndex("dbo.Encounters", new[] { "ConsultingDoctorId" });
            DropIndex("dbo.Encounters", new[] { "ReferringDoctorId" });
            DropIndex("dbo.Encounters", new[] { "AttendingDoctorId" });
            //DropIndex("dbo.Encounters", new[] { "TypeCD" });
            //DropIndex("dbo.Encounters", new[] { "ClassCD" });
            DropIndex("dbo.Encounters", new[] { "AdmitSourceCD" });
            DropColumn("dbo.Encounters", "AdmittingDoctorId");
            DropColumn("dbo.Encounters", "ConsultingDoctorId");
            DropColumn("dbo.Encounters", "ReferringDoctorId");
            DropColumn("dbo.Encounters", "AttendingDoctorId");
            DropColumn("dbo.Encounters", "DischargeDatetime");
            DropColumn("dbo.Encounters", "AdmitDatetime");
            //DropColumn("dbo.Encounters", "TypeCD");
            //DropColumn("dbo.Encounters", "ClassCD");
            DropColumn("dbo.Encounters", "AdmitSourceCD");
            DropColumn("dbo.Patients", "MiddleName");
            DropColumn("dbo.PatientContacts", "ContactRoleEndDate");
            DropColumn("dbo.PatientContacts", "ContactRoleStartDate");
        }
    }
}
