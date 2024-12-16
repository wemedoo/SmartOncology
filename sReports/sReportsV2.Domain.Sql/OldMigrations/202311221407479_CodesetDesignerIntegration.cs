namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodesetDesignerIntegration : DbMigration
    {
        public override void Up()
        {

            AddColumn("dbo.CodeSets", "ApplicableInDesigner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CodeSets", "ApplicableInDesigner");
        }
    }
}
