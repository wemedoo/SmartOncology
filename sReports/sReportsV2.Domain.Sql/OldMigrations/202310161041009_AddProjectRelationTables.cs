namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectRelationTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectDocumentRelations",
                c => new
                    {
                        ProjectPersonnelRelationId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(),
                        FormId = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPersonnelRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
            CreateTable(
                "dbo.ProjectPatientRelations",
                c => new
                    {
                        ProjectPatientRelationId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(),
                        PatientId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPatientRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.PatientId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
            CreateTable(
                "dbo.ProjectPersonnelRelations",
                c => new
                    {
                        ProjectPersonnelRelationId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(),
                        PersonnelId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPersonnelRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.PersonnelId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectPersonnelRelations", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectPersonnelRelations", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.ProjectPersonnelRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ProjectPersonnelRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ProjectPatientRelations", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectPatientRelations", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.ProjectPatientRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ProjectPatientRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ProjectDocumentRelations", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectDocumentRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.ProjectDocumentRelations", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.ProjectPersonnelRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ProjectPersonnelRelations", new[] { "CreatedById" });
            DropIndex("dbo.ProjectPersonnelRelations", new[] { "PersonnelId" });
            DropIndex("dbo.ProjectPersonnelRelations", new[] { "ProjectId" });
            DropIndex("dbo.ProjectPatientRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ProjectPatientRelations", new[] { "CreatedById" });
            DropIndex("dbo.ProjectPatientRelations", new[] { "PatientId" });
            DropIndex("dbo.ProjectPatientRelations", new[] { "ProjectId" });
            DropIndex("dbo.ProjectDocumentRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.ProjectDocumentRelations", new[] { "CreatedById" });
            DropIndex("dbo.ProjectDocumentRelations", new[] { "ProjectId" });
            DropTable("dbo.ProjectPersonnelRelations");
            DropTable("dbo.ProjectPatientRelations");
            DropTable("dbo.ProjectDocumentRelations");
        }
    }
}
