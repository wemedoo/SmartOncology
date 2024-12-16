namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodesFKToCodeSetsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Codes", "CodeSetId", c => c.Int());
            CreateIndex("dbo.Codes", "CodeSetId");
            AddForeignKey("dbo.Codes", "CodeSetId", "dbo.CodeSets", "CodeSetId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Codes", "CodeSetId", "dbo.CodeSets");
            DropIndex("dbo.Codes", new[] { "CodeSetId" });
            DropColumn("dbo.Codes", "CodeSetId");
        }
    }
}
