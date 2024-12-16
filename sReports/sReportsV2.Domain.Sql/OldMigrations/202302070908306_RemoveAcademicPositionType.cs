namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveAcademicPositionType : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.AcademicPositionTypes");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.AcademicPositionTypes",
                c => new
                {
                    AcademicPositionTypeId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.AcademicPositionTypeId);

        }
    }
}
