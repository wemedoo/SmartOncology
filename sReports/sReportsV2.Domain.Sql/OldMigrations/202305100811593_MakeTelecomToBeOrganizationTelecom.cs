namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeTelecomToBeOrganizationTelecom : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Telecoms", newName: "OrganizationTelecoms");
            RenameColumn(table: "dbo.OrganizationTelecoms", name: "TelecomId", newName: "OrganizationTelecomId");
        }

        public override void Down()
        {
            RenameColumn(table: "dbo.OrganizationTelecoms", name: "OrganizationTelecomId", newName: "TelecomId");
            RenameTable(name: "dbo.OrganizationTelecoms", newName: "Telecoms");
        }
    }
}
