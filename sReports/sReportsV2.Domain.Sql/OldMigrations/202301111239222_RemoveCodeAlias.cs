namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCodeAlias : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CodeAliases", "CodeId", "dbo.Codes");
            DropForeignKey("dbo.CodeAliases", "CreatedById", "dbo.Users");
            DropIndex("dbo.CodeAliases", new[] { "CodeId" });
            DropIndex("dbo.CodeAliases", new[] { "CreatedById" });
            DropTable("dbo.CodeAliases");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CodeAliases",
                c => new
                    {
                        AliasId = c.Int(nullable: false, identity: true),
                        CodeId = c.Int(nullable: false),
                        System = c.String(),
                        Inbound = c.String(),
                        Outbound = c.String(),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidTo = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.AliasId);
            
            CreateIndex("dbo.CodeAliases", "CreatedById");
            CreateIndex("dbo.CodeAliases", "CodeId");
            AddForeignKey("dbo.CodeAliases", "CreatedById", "dbo.Users", "UserId");
            AddForeignKey("dbo.CodeAliases", "CodeId", "dbo.Codes", "CodeId", cascadeDelete: true);
        }
    }
}
