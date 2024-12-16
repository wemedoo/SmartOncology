namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteAliasTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InboundAliases", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.InboundAliases", "CodeId", "dbo.Codes");
            DropIndex("dbo.InboundAliases", new[] { "CreatedById" });
            DropIndex("dbo.InboundAliases", new[] { "CodeId" });
            DropTable("dbo.InboundAliases");
            DropForeignKey("dbo.OutboundAliases", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.OutboundAliases", "CodeId", "dbo.Codes");
            DropIndex("dbo.OutboundAliases", new[] { "CreatedById" });
            DropIndex("dbo.OutboundAliases", new[] { "CodeId" });
            DropTable("dbo.OutboundAliases");
        }
        
        public override void Down()
        {
        }
    }
}
