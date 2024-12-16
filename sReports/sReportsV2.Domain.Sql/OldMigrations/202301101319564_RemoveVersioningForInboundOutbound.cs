namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveVersioningForInboundOutbound : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.OutboundAliases");
                context.UnsetSystemVersionedTables("dbo.OutboundAliases");
                context.DropIndexesOnCommonProperties("dbo.InboundAliases");
                context.UnsetSystemVersionedTables("dbo.InboundAliases");
            }
        }
        
        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.InboundAliases");
                context.CreateIndexesOnCommonProperties("dbo.InboundAliases");
                context.SetSystemVersionedTables("dbo.OutboundAliases");
                context.CreateIndexesOnCommonProperties("dbo.OutboundAliases");
            }
        }
    }
}
