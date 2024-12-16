namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveVersioningUserAcademicPosition : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.UserAcademicPositions");
                context.UnsetSystemVersionedTables("dbo.UserAcademicPositions");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.UserAcademicPositions");
                context.CreateIndexesOnCommonProperties("dbo.UserAcademicPositions");
            }
        }
    }
}
