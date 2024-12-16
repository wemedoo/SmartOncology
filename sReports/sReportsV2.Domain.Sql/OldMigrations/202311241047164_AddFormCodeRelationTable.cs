namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFormCodeRelationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FormCodeRelations",
                c => new
                    {
                        FormCodeRelationId = c.Int(nullable: false, identity: true),
                        CodeCD = c.Int(),
                        FormId = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.FormCodeRelationId)
                .ForeignKey("dbo.Codes", t => t.CodeCD)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .Index(t => t.CodeCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormCodeRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.FormCodeRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.FormCodeRelations", "CodeCD", "dbo.Codes");
            DropIndex("dbo.FormCodeRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.FormCodeRelations", new[] { "CreatedById" });
            DropIndex("dbo.FormCodeRelations", new[] { "CodeCD" });
            DropTable("dbo.FormCodeRelations");
        }
    }
}
