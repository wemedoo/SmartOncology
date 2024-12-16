namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOnlyOnePatientContact : DbMigration
    {
        public override void Up()
        {
            string removeContact = @"
                alter table dbo.Patients drop constraint if exists [FK_dbo.Patients_dbo.Contacts_ContactPerson_Id];
                drop index if exists dbo.Patients.IX_ContactPerson_Id;
                alter table dbo.Patients drop column if exists ContactId;";
            Sql(removeContact);
        }

        public override void Down()
        {
            AddColumn("dbo.Patients", "ContactId", c => c.Int());
            CreateIndex("dbo.Patients", "ContactPerson_Id");
            AddForeignKey("dbo.Patients", "ContactPerson_Id", "dbo.PatientContacts", "Id");
        }
    }
}
