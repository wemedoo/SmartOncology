namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeSets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeSets",
                c => new
                    {
                        CodeSetId = c.Int(nullable: false),
                        ThesaurusEntryId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.CodeSetId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.ThesaurusEntries", t => t.ThesaurusEntryId, cascadeDelete: true)
                .Index(t => t.ThesaurusEntryId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CodeSets", "ThesaurusEntryId", "dbo.ThesaurusEntries");
            DropForeignKey("dbo.CodeSets", "CreatedById", "dbo.Users");
            DropIndex("dbo.CodeSets", new[] { "CreatedById" });
            DropIndex("dbo.CodeSets", new[] { "ThesaurusEntryId" });
            DropTable("dbo.CodeSets");
        }
    }
}
