namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropsToPatientContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientContacts", "BirthDate", c => c.DateTime());
            AddColumn("dbo.PatientContacts", "ContactRelationshipCD", c => c.Int());
            AddColumn("dbo.PatientContacts", "ContactRoleCD", c => c.Int());
            CreateIndex("dbo.PatientContacts", "ContactRelationshipCD");
            CreateIndex("dbo.PatientContacts", "ContactRoleCD");
            AddForeignKey("dbo.PatientContacts", "ContactRelationshipCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PatientContacts", "ContactRoleCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.PatientContacts", "Relationship");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientContacts", "Relationship", c => c.String());
            DropForeignKey("dbo.PatientContacts", "ContactRoleCD", "dbo.Codes");
            DropForeignKey("dbo.PatientContacts", "ContactRelationshipCD", "dbo.Codes");
            DropIndex("dbo.PatientContacts", new[] { "ContactRoleCD" });
            DropIndex("dbo.PatientContacts", new[] { "ContactRelationshipCD" });
            DropColumn("dbo.PatientContacts", "ContactRoleCD");
            DropColumn("dbo.PatientContacts", "ContactRelationshipCD");
            DropColumn("dbo.PatientContacts", "BirthDate");
        }
    }
}
