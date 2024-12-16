namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using sReportsV2.DAL.Sql.Sql;

    public partial class AddVersioningForProjects : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.Projects");
                context.CreateIndexesOnCommonProperties("dbo.Projects");
            }
        }

        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.Projects");
                context.UnsetSystemVersionedTables("dbo.Projects");
            }
        }
    }
}
