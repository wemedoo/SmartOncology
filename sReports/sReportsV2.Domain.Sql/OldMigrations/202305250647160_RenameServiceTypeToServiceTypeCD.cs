namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameServiceTypeToServiceTypeCD : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM dbo.CodeSets WHERE CodeSetId=52");
            AddColumn("dbo.Encounters", "ServiceTypeCD", c => c.Int());
            Sql("UPDATE dbo.Encounters SET ServiceTypeCD = ServiceType");
            CreateIndex("dbo.Encounters", "ServiceTypeCD");
            AddForeignKey("dbo.Encounters", "ServiceTypeCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.Encounters", "ServiceType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Encounters", "ServiceType", c => c.Int(nullable: false));
            DropForeignKey("dbo.Encounters", "ServiceTypeCD", "dbo.Codes");
            DropIndex("dbo.Encounters", new[] { "ServiceTypeCD" });
            DropColumn("dbo.Encounters", "ServiceTypeCD");
        }
    }
}
