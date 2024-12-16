namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientContactGenderCD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientContacts", "GenderCD", c => c.Int());
            CreateIndex("dbo.PatientContacts", "GenderCD");
            AddForeignKey("dbo.PatientContacts", "GenderCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.PatientContacts", "Gender");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientContacts", "Gender", c => c.String());
            DropForeignKey("dbo.PatientContacts", "GenderCD", "dbo.Codes");
            DropIndex("dbo.PatientContacts", new[] { "GenderCD" });
            DropColumn("dbo.PatientContacts", "GenderCD");
        }
    }
}
