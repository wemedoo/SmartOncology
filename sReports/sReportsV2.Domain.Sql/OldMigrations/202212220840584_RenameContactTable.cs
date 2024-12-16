namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameContactTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Contacts", newName: "PatientContacts");
            RenameColumn(table: "dbo.Telecoms", name: "ContactId", newName: "PatientContactId");
            RenameIndex(table: "dbo.Telecoms", name: "IX_Contact_Id", newName: "IX_PatientContact_Id");
        }

        public override void Down()
        {
            RenameIndex(table: "dbo.Telecoms", name: "IX_PatientContact_Id", newName: "IX_Contact_Id");
            RenameColumn(table: "dbo.Telecoms", name: "PatientContactId", newName: "ContactId");
            RenameTable(name: "dbo.PatientContacts", newName: "Contacts");
        }
    }
}
