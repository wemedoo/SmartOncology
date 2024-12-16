namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCodeSets : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CodeSets", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.CodeSets", "ThesaurusEntryId", "dbo.ThesaurusEntries");
            DropIndex("dbo.CodeSets", new[] { "ThesaurusEntryId" });
            DropIndex("dbo.CodeSets", new[] { "CreatedById" });
            DropTable("dbo.CodeSets");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CodeSets",
                c => new
                    {
                        CodeSetId = c.Int(nullable: false, identity: true),
                        ThesaurusEntryId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.CodeSetId);
            
            CreateIndex("dbo.CodeSets", "CreatedById");
            CreateIndex("dbo.CodeSets", "ThesaurusEntryId");
            AddForeignKey("dbo.CodeSets", "ThesaurusEntryId", "dbo.ThesaurusEntries", "ThesaurusEntryId", cascadeDelete: true);
            AddForeignKey("dbo.CodeSets", "CreatedById", "dbo.Users", "UserId");
        }
    }
}
