namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelOccupationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelOccupations",
                c => new
                    {
                        PersonnelOccupationId = c.Int(nullable: false, identity: true),
                        PersonnelId = c.Int(nullable: false),
                        OccupationCategoryCD = c.Int(nullable: false),
                        OccupationSubCategoryCD = c.Int(nullable: false),
                        OccupationCD = c.Int(nullable: false),
                        PersonnelSeniorityCD = c.Int(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.PersonnelOccupationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Codes", t => t.OccupationCD)
                .ForeignKey("dbo.Codes", t => t.OccupationCategoryCD, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.OccupationSubCategoryCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.PersonnelSeniorityCD)
                .Index(t => t.PersonnelId)
                .Index(t => t.OccupationCategoryCD)
                .Index(t => t.OccupationSubCategoryCD)
                .Index(t => t.OccupationCD)
                .Index(t => t.PersonnelSeniorityCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonnelOccupations", "PersonnelSeniorityCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelOccupations", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelOccupations", "OccupationSubCategoryCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelOccupations", "OccupationCategoryCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelOccupations", "OccupationCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelOccupations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelOccupations", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.PersonnelOccupations", new[] { "EntityStateCD" });
            DropIndex("dbo.PersonnelOccupations", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelOccupations", new[] { "PersonnelSeniorityCD" });
            DropIndex("dbo.PersonnelOccupations", new[] { "OccupationCD" });
            DropIndex("dbo.PersonnelOccupations", new[] { "OccupationSubCategoryCD" });
            DropIndex("dbo.PersonnelOccupations", new[] { "OccupationCategoryCD" });
            DropIndex("dbo.PersonnelOccupations", new[] { "PersonnelId" });
            DropTable("dbo.PersonnelOccupations");
        }
    }
}
