namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateClinicalTrialDocumentRelationsTable : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.ClinicalTrialPersonnelRelationId)
                .ForeignKey("dbo.ClinicalTrials", t => t.ClinicalTrialId, cascadeDelete: true)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .Index(t => t.ClinicalTrialId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialDocumentRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialDocumentRelations", new[] { "ClinicalTrialId" });
            DropTable("dbo.ClinicalTrialDocumentRelations");
        }
    }
}
