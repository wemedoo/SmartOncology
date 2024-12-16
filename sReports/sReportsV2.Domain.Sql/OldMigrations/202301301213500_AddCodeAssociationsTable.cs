namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeAssociationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeAssociations",
                c => new
                    {
                        CodeAssociationId = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ChildId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.CodeAssociationId)
                .ForeignKey("dbo.Codes", t => t.ChildId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.ParentId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CodeAssociations", "ParentId", "dbo.Codes");
            DropForeignKey("dbo.CodeAssociations", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.CodeAssociations", "ChildId", "dbo.Codes");
            DropIndex("dbo.CodeAssociations", new[] { "CreatedById" });
            DropIndex("dbo.CodeAssociations", new[] { "ChildId" });
            DropIndex("dbo.CodeAssociations", new[] { "ParentId" });
            DropTable("dbo.CodeAssociations");
        }
    }
}
