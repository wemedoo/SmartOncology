namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaritalStatusToPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "MaritalStatusCD", c => c.Int());
            CreateIndex("dbo.Patients", "MaritalStatusCD");
            AddForeignKey("dbo.Patients", "MaritalStatusCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "MaritalStatusCD", "dbo.Codes");
            DropIndex("dbo.Patients", new[] { "MaritalStatusCD" });
            DropColumn("dbo.Patients", "MaritalStatusCD");
        }
    }
}
