namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePKs : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PersonnelClinicalTrials", name: "UserClinicalTrialId", newName: "PersonnelClinicalTrialId");
            RenameColumn(table: "dbo.PersonnelOrganizations", name: "UserOrganizationId", newName: "PersonnelOrganizationId");
            RenameColumn(table: "dbo.PersonnelConfigs", name: "UserConfigId", newName: "PersonnelConfigId");
            RenameColumn(table: "dbo.Transactions", name: "ProcedeedMessageLogId", newName: "TransactionId");
            RenameColumn(table: "dbo.Personnel", name: "UserId", newName: "PersonnelId");
        }

        public override void Down()
        {
            RenameColumn(table: "dbo.Personnel", name: "PersonnelId", newName: "UserId");
            RenameColumn(table: "dbo.Transactions", name: "TransactionId", newName: "ProcedeedMessageLogId");
            RenameColumn(table: "dbo.PersonnelConfigs", name: "PersonnelConfigId", newName: "UserConfigId");
            RenameColumn(table: "dbo.PersonnelOrganizations", name: "PersonnelOrganizationId", newName: "UserOrganizationId");
            RenameColumn(table: "dbo.PersonnelClinicalTrials", name: "PersonnelClinicalTrialId", newName: "UserClinicalTrialId");
        }
    }
}
