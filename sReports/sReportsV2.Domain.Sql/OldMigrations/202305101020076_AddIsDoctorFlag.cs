namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDoctorFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Personnel", "IsDoctor", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Personnel", "IsDoctor");
        }
    }
}
