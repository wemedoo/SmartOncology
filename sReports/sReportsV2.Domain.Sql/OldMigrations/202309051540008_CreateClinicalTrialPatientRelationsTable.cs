namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateClinicalTrialPatientRelationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalTrialPatientRelations",
                c => new
                {
                    ClinicalTrialPatientRelationId = c.Int(nullable: false, identity: true),
                    ClinicalTrialId = c.Int(nullable: false),
                    PatientId = c.Int(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                    ActiveFrom = c.DateTime(nullable: false),
                    ActiveTo = c.DateTime(nullable: false),
                    EntityStateCD = c.Int(),
                })
                .PrimaryKey(t => t.ClinicalTrialPatientRelationId)
                .ForeignKey("dbo.ClinicalTrials", t => t.ClinicalTrialId, cascadeDelete: true)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.ClinicalTrialId)
                .Index(t => t.PatientId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "PatientId" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "ClinicalTrialId" });
            DropTable("dbo.ClinicalTrialPatientRelations");
        }
    }
}
