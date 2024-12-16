namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateOutboundAliasesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OutboundAliases",
                c => new
                    {
                        AliasId = c.Int(nullable: false, identity: true),
                        CodeId = c.Int(nullable: false),
                        Alias = c.String(),
                        System = c.String(),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidTo = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.AliasId)
                .ForeignKey("dbo.Codes", t => t.CodeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .Index(t => t.CodeId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundAliases", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.OutboundAliases", "CodeId", "dbo.Codes");
            DropIndex("dbo.OutboundAliases", new[] { "CreatedById" });
            DropIndex("dbo.OutboundAliases", new[] { "CodeId" });
            DropTable("dbo.OutboundAliases");
        }
    }
}
