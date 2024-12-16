namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateClinicalTrialPersonnelRelationsTable : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.ClinicalTrialPersonnelRelationId)
                .ForeignKey("dbo.ClinicalTrials", t => t.ClinicalTrialId, cascadeDelete: true)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .Index(t => t.PersonnelId)
                .Index(t => t.ClinicalTrialId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "ClinicalTrialId", "dbo.ClinicalTrials");
            DropForeignKey("dbo.ClinicalTrialPersonnelRelations", "PersonnelId", "dbo.Personnel");
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "CreatedById" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "ClinicalTrialId" });
            DropIndex("dbo.ClinicalTrialPersonnelRelations", new[] { "PersonnelId" });
            DropTable("dbo.ClinicalTrialPersonnelRelations");
        }
    }
}
