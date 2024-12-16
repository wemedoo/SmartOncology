namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTrialRelationTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialPatientRelations", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "PersonnelId", "dbo.Personnel");
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "ClinicalTrialId" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "PersonnelId" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "ClinicalTrialId" });
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "ClinicalTrialId" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "PatientId" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialPatientRelations", new[] { "EntityStateCD" });
            DropTable("dbo.ClinicalTrialPersonnelRelations");
            DropTable("dbo.ClinicalTrialDocumentRelations");
            DropTable("dbo.ClinicalTrialPatientRelations");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.ClinicalTrialPatientRelationId);

            CreateTable(
                "dbo.ClinicalTrialDocumentRelations",
                c => new
                {
                    ClinicalTrialPersonnelRelationId = c.Int(nullable: false, identity: true),
                    ClinicalTrialId = c.Int(nullable: false),
                    FormId = c.String(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                    ActiveFrom = c.DateTime(nullable: false),
                    ActiveTo = c.DateTime(nullable: false),
                    EntityStateCD = c.Int(),
                })
                .PrimaryKey(t => t.ClinicalTrialPersonnelRelationId);

            CreateTable(
                "dbo.ClinicalTrialPersonnelRelations",
                c => new
                {
                    ClinicalTrialPersonnelRelationId = c.Int(nullable: false, identity: true),
                    ClinicalTrialId = c.Int(nullable: false),
                    PersonnelId = c.Int(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                    ActiveFrom = c.DateTime(nullable: false),
                    ActiveTo = c.DateTime(nullable: false),
                    EntityStateCD = c.Int(),
                })
                .PrimaryKey(t => t.ClinicalTrialPersonnelRelationId);

            CreateIndex("dbo.ClinicalTrialPatientRelations", "EntityStateCD");
            CreateIndex("dbo.ClinicalTrialPatientRelations", "CreatedById");
            CreateIndex("dbo.ClinicalTrialPatientRelations", "PatientId");
            CreateIndex("dbo.ClinicalTrialPatientRelations", "ClinicalTrialId");
            CreateIndex("dbo.ClinicalTrialDocumentRelations", "EntityStateCD");
            CreateIndex("dbo.ClinicalTrialDocumentRelations", "CreatedById");
            CreateIndex("dbo.ClinicalTrialDocumentRelations", "ClinicalTrialId");
            CreateIndex("dbo.ClinicalTrialPersonnelRelations", "EntityStateCD");
            CreateIndex("dbo.ClinicalTrialPersonnelRelations", "CreatedById");
            CreateIndex("dbo.ClinicalTrialPersonnelRelations", "PersonnelId");
            CreateIndex("dbo.ClinicalTrialPersonnelRelations", "ClinicalTrialId");
            AddForeignKey("dbo.ClinicalTrialPersonnelRelations", "PersonnelId", "dbo.Personnel", "PersonnelId", cascadeDelete: true);
            AddForeignKey("dbo.ClinicalTrialPersonnelRelations", "EntityStateCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.ClinicalTrialPersonnelRelations", "CreatedById", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.ClinicalTrialPersonnelRelations", "ClinicalTrialId", "dbo.ClinicalTrials", "ClinicalTrialId", cascadeDelete: true);
            AddForeignKey("dbo.ClinicalTrialPatientRelations", "PatientId", "dbo.Patients", "PatientId", cascadeDelete: true);
            AddForeignKey("dbo.ClinicalTrialPatientRelations", "EntityStateCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.ClinicalTrialPatientRelations", "CreatedById", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.ClinicalTrialPatientRelations", "ClinicalTrialId", "dbo.ClinicalTrials", "ClinicalTrialId", cascadeDelete: true);
            AddForeignKey("dbo.ClinicalTrialDocumentRelations", "EntityStateCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.ClinicalTrialDocumentRelations", "CreatedById", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.ClinicalTrialDocumentRelations", "ClinicalTrialId", "dbo.ClinicalTrials", "ClinicalTrialId", cascadeDelete: true);
        }
    }
}
