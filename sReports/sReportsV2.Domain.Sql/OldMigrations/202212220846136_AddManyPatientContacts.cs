namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyPatientContacts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientContacts", "PatientId", c => c.Int());
            CreateIndex("dbo.PatientContacts", "PatientId");
            AddForeignKey("dbo.PatientContacts", "PatientId", "dbo.Patients", "PatientId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.PatientContacts", "PatientId", "dbo.Patients");
            DropIndex("dbo.PatientContacts", new[] { "PatientId" });
            DropColumn("dbo.PatientContacts", "PatientId");
        }
    }
}
