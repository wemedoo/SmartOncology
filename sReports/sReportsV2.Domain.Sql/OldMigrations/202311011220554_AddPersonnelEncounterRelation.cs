namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelEncounterRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelEncounterRelations",
                c => new
                    {
                        PersonnelEncounterRelationId = c.Int(nullable: false, identity: true),
                        EncounterId = c.Int(nullable: false),
                        PersonnelId = c.Int(nullable: false),
                        RelationTypeCD = c.Int(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.PersonnelEncounterRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Encounters", t => t.EncounterId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.RelationTypeCD)
                .Index(t => t.EncounterId)
                .Index(t => t.PersonnelId)
                .Index(t => t.RelationTypeCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonnelEncounterRelations", "RelationTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelEncounterRelations", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelEncounterRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelEncounterRelations", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.PersonnelEncounterRelations", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.PersonnelEncounterRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.PersonnelEncounterRelations", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelEncounterRelations", new[] { "RelationTypeCD" });
            DropIndex("dbo.PersonnelEncounterRelations", new[] { "PersonnelId" });
            DropIndex("dbo.PersonnelEncounterRelations", new[] { "EncounterId" });
            DropTable("dbo.PersonnelEncounterRelations");
        }
    }
}
