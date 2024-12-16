namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOutboundIdToInboundAliasTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InboundAliases", "OutboundAliasId", c => c.Int());
            CreateIndex("dbo.InboundAliases", "OutboundAliasId");
            AddForeignKey("dbo.InboundAliases", "OutboundAliasId", "dbo.OutboundAliases", "AliasId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InboundAliases", "OutboundAliasId", "dbo.OutboundAliases");
            DropIndex("dbo.InboundAliases", new[] { "OutboundAliasId" });
            DropColumn("dbo.InboundAliases", "OutboundAliasId");
        }
    }
}
