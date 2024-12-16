namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePatientContactIds : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PatientContactTelecoms", name: "Id", newName: "PatientContactTelecomId");
            RenameColumn(table: "dbo.PatientContactAddresses", name: "Id", newName: "PatientContactAddressId");
            RenameColumn(table: "dbo.PatientContacts", name: "ContactId", newName: "PatientContactId");
        }

        public override void Down()
        {
            RenameColumn(table: "dbo.PatientContacts", name: "PatientContactId", newName: "ContactId");
            RenameColumn(table: "dbo.PatientContactAddresses", name: "PatientContactAddressId", newName: "Id");
            RenameColumn(table: "dbo.PatientContactTelecoms", name: "PatientContactTelecomId", newName: "Id");
        }
    }
}
