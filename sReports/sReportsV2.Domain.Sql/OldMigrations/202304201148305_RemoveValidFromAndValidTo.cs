namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveValidFromAndValidTo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InboundAliases", "ValidFrom");
            DropColumn("dbo.InboundAliases", "ValidTo");
            DropColumn("dbo.OutboundAliases", "ValidFrom");
            DropColumn("dbo.OutboundAliases", "ValidTo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InboundAliases", "ValidTo", c => c.DateTime(nullable: false));
            AddColumn("dbo.InboundAliases", "ValidFrom", c => c.DateTime(nullable: false));
            AddColumn("dbo.CodeAliasViews", "ValidTo", c => c.DateTime(nullable: false));
            AddColumn("dbo.CodeAliasViews", "ValidFrom", c => c.DateTime(nullable: false));
        }
    }
}
