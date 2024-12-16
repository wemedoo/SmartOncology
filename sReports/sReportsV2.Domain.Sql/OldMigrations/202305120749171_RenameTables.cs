namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTables : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserClinicalTrials", newName: "PersonnelClinicalTrials");
            RenameTable(name: "dbo.UserOrganizations", newName: "PersonnelOrganizations");
            RenameTable(name: "dbo.UserConfigs", newName: "PersonnelConfigs");
            RenameTable(name: "dbo.HL7MessageLog", newName: "HL7MessageLogs");
            RenameTable(name: "dbo.ProcedeedMessageLogs", newName: "Transactions");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Transactions", newName: "ProcedeedMessageLogs");
            RenameTable(name: "dbo.HL7MessageLogs", newName: "HL7MessageLog");
            RenameTable(name: "dbo.PersonnelConfigs", newName: "UserConfigs");
            RenameTable(name: "dbo.PersonnelOrganizations", newName: "UserOrganizations");
            RenameTable(name: "dbo.PersonnelClinicalTrials", newName: "UserClinicalTrials");
        }
    }
}
