namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientFKInEncounter : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Encounters", "PatientId");
            AddForeignKey("dbo.Encounters", "PatientId", "dbo.Patients", "PatientId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Encounters", "PatientId", "dbo.Patients");
            DropIndex("dbo.Encounters", new[] { "PatientId" });
        }
    }
}
