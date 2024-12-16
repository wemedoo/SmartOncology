namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelAcademicPosition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelAcademicPositions",
                c => new
                {
                    PersonnelAcademicPositionId = c.Int(nullable: false, identity: true),
                    PersonnelId = c.Int(nullable: false),
                    AcademicPositionCD = c.Int(),
                    AcademicPositionTypeCD = c.Int(),
                    Active = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                })
                .PrimaryKey(t => t.PersonnelAcademicPositionId)
                .ForeignKey("dbo.Codes", t => t.AcademicPositionCD)
                .ForeignKey("dbo.Codes", t => t.AcademicPositionTypeCD)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .Index(t => t.PersonnelId)
                .Index(t => t.AcademicPositionCD)
                .Index(t => t.AcademicPositionTypeCD)
                .Index(t => t.CreatedById);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PersonnelAcademicPositions", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelAcademicPositions", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelAcademicPositions", "AcademicPositionTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelAcademicPositions", "AcademicPositionCD", "dbo.Codes");
            DropIndex("dbo.PersonnelAcademicPositions", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelAcademicPositions", new[] { "AcademicPositionTypeCD" });
            DropIndex("dbo.PersonnelAcademicPositions", new[] { "AcademicPositionCD" });
            DropIndex("dbo.PersonnelAcademicPositions", new[] { "PersonnelId" });
            DropTable("dbo.PersonnelAcademicPositions");
        }
    }
}
