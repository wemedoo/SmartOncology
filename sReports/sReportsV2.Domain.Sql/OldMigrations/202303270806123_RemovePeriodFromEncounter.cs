namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePeriodFromEncounter : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Encounters", "Period_Start");
            DropColumn("dbo.Encounters", "Period_End");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Encounters", "Period_End", c => c.DateTime());
            AddColumn("dbo.Encounters", "Period_Start", c => c.DateTime(nullable: false));
        }
    }
}
