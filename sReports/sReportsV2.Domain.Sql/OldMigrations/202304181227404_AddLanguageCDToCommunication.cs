namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLanguageCDToCommunication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Communications", "LanguageCD", c => c.Int());
            CreateIndex("dbo.Communications", "LanguageCD");
            AddForeignKey("dbo.Communications", "LanguageCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Communications", "LanguageCD", "dbo.Codes");
            DropIndex("dbo.Communications", new[] { "LanguageCD" });
            DropColumn("dbo.Communications", "LanguageCD");
        }
    }
}
