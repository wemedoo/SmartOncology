namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveVersioningForRolesTables : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.UserRoles");
                context.UnsetSystemVersionedTables("dbo.UserRoles");
                context.DropIndexesOnCommonProperties("dbo.Roles");
                context.UnsetSystemVersionedTables("dbo.Roles");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.UserRoles");
                context.CreateIndexesOnCommonProperties("dbo.UserRoles");
                context.SetSystemVersionedTables("dbo.Roles");
                context.CreateIndexesOnCommonProperties("dbo.Roles");
            }
        }
    }
}
