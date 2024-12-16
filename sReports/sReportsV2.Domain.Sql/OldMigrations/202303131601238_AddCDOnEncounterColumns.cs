namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCDOnEncounterColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Encounters", name: "Status", newName: "StatusCD");
            RenameColumn(table: "dbo.Encounters", name: "Class", newName: "ClassCD");
            RenameColumn(table: "dbo.Encounters", name: "Type", newName: "TypeCD");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Encounters", name: "TypeCD", newName: "Type");
            RenameColumn(table: "dbo.Encounters", name: "ClassCD", newName: "Class");
            RenameColumn(table: "dbo.Encounters", name: "StatusCD", newName: "Status");
        }
    }
}
