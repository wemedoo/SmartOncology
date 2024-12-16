namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixColumnNameForPatienContactAddress : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PatientContactAddresses", name: "CountryId", newName: "CountryCD");
            RenameColumn(table: "dbo.PatientContactAddresses", name: "AddressTypeId", newName: "AddressTypeCD");
        }

        public override void Down()
        {
            RenameColumn(table: "dbo.PatientContactAddresses", name: "CountryCD", newName: "CountryId");
            RenameColumn(table: "dbo.PatientContactAddresses", name: "AddressTypeCD", newName: "AddressTypeId");
        }
    }
}
