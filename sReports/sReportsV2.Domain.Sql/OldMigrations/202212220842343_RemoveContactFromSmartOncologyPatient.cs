namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveContactFromSmartOncologyPatient : DbMigration
    {
        public override void Up()
        {
            string removeContactFromSmartOncologyPatient = @"
                alter table dbo.SmartOncologyPatients drop constraint if exists [FK_dbo.SmartOncologyPatients_dbo.Contacts_ContactPerson_Id];
                drop index if exists dbo.SmartOncologyPatients.IX_ContactPerson_Id;
                alter table dbo.SmartOncologyPatients drop column if exists ContactId;";
            Sql(removeContactFromSmartOncologyPatient);
        }

        public override void Down()
        {
            AddColumn("dbo.SmartOncologyPatients", "ContactId", c => c.Int());
            CreateIndex("dbo.SmartOncologyPatients", "ContactPerson_Id");
            AddForeignKey("dbo.SmartOncologyPatients", "ContactPerson_Id", "dbo.PatientContacts", "Id");
        }
    }
}
