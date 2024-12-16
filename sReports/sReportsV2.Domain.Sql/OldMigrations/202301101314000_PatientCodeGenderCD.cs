namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientCodeGenderCD : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SmartOncologyPatients", "GenderCD");
            DropColumn("dbo.Patients", "GenderCD");
            AddColumn("dbo.SmartOncologyPatients", "GenderCD", c => c.Int());
            AddColumn("dbo.Patients", "GenderCD", c => c.Int());
            CreateIndex("dbo.SmartOncologyPatients", "GenderCD");
            CreateIndex("dbo.Patients", "GenderCD");
            AddForeignKey("dbo.SmartOncologyPatients", "GenderCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Patients", "GenderCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "GenderCD", "dbo.Codes");
            DropForeignKey("dbo.SmartOncologyPatients", "GenderCD", "dbo.Codes");
            DropIndex("dbo.Patients", new[] { "GenderCD" });
            DropIndex("dbo.SmartOncologyPatients", new[] { "GenderCD" });
            DropColumn("dbo.Patients", "GenderCD");
            DropColumn("dbo.SmartOncologyPatients", "GenderCD");
            AddColumn("dbo.Patients", "GenderCD", c => c.Int(nullable: false));
            AddColumn("dbo.SmartOncologyPatients", "GenderCD", c => c.Int(nullable: false));
        }
    }
}
