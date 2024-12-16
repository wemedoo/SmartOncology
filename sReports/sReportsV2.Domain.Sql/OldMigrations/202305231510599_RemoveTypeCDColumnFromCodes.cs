namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTypeCDColumnFromCodes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Codes", "TypeCD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Codes", "TypeCD", c => c.Int(nullable: false));
        }
    }
}
