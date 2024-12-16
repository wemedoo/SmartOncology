namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnsetAddressToBeSystemVersioned : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.Addresses");
                context.UnsetSystemVersionedTables("dbo.Addresses");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.Addresses");
                context.CreateIndexesOnCommonProperties("dbo.Addresses");
            }
        }
    }
}
