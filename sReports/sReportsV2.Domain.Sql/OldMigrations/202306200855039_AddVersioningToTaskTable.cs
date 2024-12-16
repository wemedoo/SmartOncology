namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using sReportsV2.DAL.Sql.Sql;

    public partial class AddVersioningToTaskTable : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.Tasks");
                context.CreateIndexesOnCommonProperties("dbo.Tasks");
            }
        }
        
        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.Tasks");
                context.UnsetSystemVersionedTables("dbo.Tasks");
            }
        }
    }
}
