namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteLanguageColumnFromCommunication : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Communications", "Language");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Communications", "Language", c => c.String());
        }
    }
}
