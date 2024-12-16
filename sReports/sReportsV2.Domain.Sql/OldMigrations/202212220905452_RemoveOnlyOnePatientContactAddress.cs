namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOnlyOnePatientContactAddress : DbMigration
    {
        public override void Up()
        {
            string removeOnlyOnePatientContactAddress = @"
                alter table dbo.PatientContacts drop constraint if exists [FK_dbo.Contacts_dbo.Addresses_Address_Id];
                drop index if exists dbo.PatientContacts.IX_Address_Id;
                alter table dbo.PatientContacts drop column if exists AddressId;";
            Sql(removeOnlyOnePatientContactAddress);
        }

        public override void Down()
        {
            AddColumn("dbo.PatientContacts", "AddressId", c => c.Int());
            CreateIndex("dbo.PatientContacts", "Address_Id");
            AddForeignKey("dbo.PatientContacts", "Address_Id", "dbo.Addresses", "Id");
        }
    }
}
