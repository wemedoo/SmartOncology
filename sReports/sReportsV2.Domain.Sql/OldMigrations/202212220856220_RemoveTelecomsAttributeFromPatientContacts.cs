namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTelecomsAttributeFromPatientContacts : DbMigration
    {
        public override void Up()
        {
            string removeContactFromTelecomDB = @"
                delete from dbo.Telecoms where PatientContactId is not null; 
                alter table dbo.Telecoms drop constraint if exists [FK_dbo.Telecoms_dbo.Contacts_Contact_Id];
                drop index if exists dbo.Telecoms.IX_PatientContact_Id;
                alter table dbo.Telecoms drop column if exists PatientContactId;";
            Sql(removeContactFromTelecomDB);
        }

        public override void Down()
        {
            AddColumn("dbo.Telecoms", "PatientContactId", c => c.Int());
            CreateIndex("dbo.Telecoms", "PatientContact_Id");
            AddForeignKey("dbo.Telecoms", "PatientContact_Id", "dbo.PatientContacts", "Id");
        }
    }
}
