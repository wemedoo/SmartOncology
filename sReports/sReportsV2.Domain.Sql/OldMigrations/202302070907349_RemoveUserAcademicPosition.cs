namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveUserAcademicPosition : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserAcademicPositions", "AcademicPositionTypeId", "dbo.AcademicPositionTypes");
            DropForeignKey("dbo.UserAcademicPositions", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.UserAcademicPositions", "UserId", "dbo.Personnel");
            DropIndex("dbo.UserAcademicPositions", new[] { "UserId" });
            DropIndex("dbo.UserAcademicPositions", new[] { "AcademicPositionTypeId" });
            DropIndex("dbo.UserAcademicPositions", new[] { "CreatedById" });
            DropTable("dbo.UserAcademicPositions");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.UserAcademicPositions",
                c => new
                {
                    UserAcademicPositionId = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    AcademicPositionTypeId = c.Int(nullable: false),
                    Active = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                })
                .PrimaryKey(t => t.UserAcademicPositionId);

            CreateIndex("dbo.UserAcademicPositions", "CreatedById");
            CreateIndex("dbo.UserAcademicPositions", "AcademicPositionTypeId");
            CreateIndex("dbo.UserAcademicPositions", "UserId");
            AddForeignKey("dbo.UserAcademicPositions", "UserId", "dbo.Personnel", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UserAcademicPositions", "CreatedById", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.UserAcademicPositions", "AcademicPositionTypeId", "dbo.AcademicPositionTypes", "AcademicPositionTypeId", cascadeDelete: true);
        }
    }
}
