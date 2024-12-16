namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersioningForOrgAndOUAddrs : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.OrganizationAddresses");
                context.CreateIndexesOnCommonProperties("dbo.OrganizationAddresses");
                context.SetSystemVersionedTables("dbo.OutsideUserAddresses");
                context.CreateIndexesOnCommonProperties("dbo.OutsideUserAddresses");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.OutsideUserAddresses");
                context.UnsetSystemVersionedTables("dbo.OutsideUserAddresses");
                context.DropIndexesOnCommonProperties("dbo.OrganizationAddresses");
                context.UnsetSystemVersionedTables("dbo.OrganizationAddresses");
            }
        }
    }
}
