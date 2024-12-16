namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class SaveDataToNewIdentifierTables : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string saveDataIntoPatientIdentifier = @"
                insert into dbo.PatientIdentifiers (PatientId, IdentifierValue, IdentifierTypeCD, IdentifierPoolCD, IdentifierUseCD, Active, IsDeleted, EntryDatetime, LastUpdate, CreatedById)
                SELECT PatientId
                      ,IdentifierValue
                      ,IdentifierTypeCD
                      ,IdentifierPoolCD
                      ,IdentifierUseCD
                      ,Active
                      ,IsDeleted
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                  FROM dbo.Identifiers where PatientId is not null;
            ";

            string saveDataIntoOrganizationIdentifier = @"
                insert into dbo.OrganizationIdentifiers (OrganizationId, IdentifierValue, IdentifierTypeCD, IdentifierPoolCD, IdentifierUseCD, Active, IsDeleted, EntryDatetime, LastUpdate, CreatedById)
                  SELECT 
                      OrganizationId
                      ,IdentifierValue
                      ,IdentifierTypeCD
                      ,IdentifierPoolCD
                      ,IdentifierUseCD
                      ,Active
                      ,IsDeleted
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                  FROM dbo.Identifiers where OrganizationId is not null;
            ";

            string deleteDataFromIdentifier = @"
                delete from dbo.Identifiers;
            ";

            context.Database.ExecuteSqlCommand(saveDataIntoPatientIdentifier);
            context.Database.ExecuteSqlCommand(saveDataIntoOrganizationIdentifier);
            context.Database.ExecuteSqlCommand(deleteDataFromIdentifier);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();

            string revertDataFromPatientIdentifier = @"
                insert into dbo.Identifiers (PatientId, IdentifierValue, IdentifierTypeCD, IdentifierPoolCD, IdentifierUseCD, Active, IsDeleted, EntryDatetime, LastUpdate, CreatedById)
                SELECT PatientId
                      ,IdentifierValue
                      ,IdentifierTypeCD
                      ,IdentifierPoolCD
                      ,IdentifierUseCD
                      ,Active
                      ,IsDeleted
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                  FROM dbo.PatientIdentifiers;
            ";

            string revertDataFromOrganizationIdentifier = @"
                insert into dbo.Identifiers (OrganizationId, IdentifierValue, IdentifierTypeCD, IdentifierPoolCD, IdentifierUseCD, Active, IsDeleted, EntryDatetime, LastUpdate, CreatedById)
                  SELECT 
                      OrganizationId
                      ,IdentifierValue
                      ,IdentifierTypeCD
                      ,IdentifierPoolCD
                      ,IdentifierUseCD
                      ,Active
                      ,IsDeleted
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                  FROM dbo.OrganizationIdentifiers;
            ";

            string deleteFromPatientIdentifiers = @"
                delete from dbo.PatientIdentifiers;
            ";

            string deleteFromOrganizationIdentifiers = @"
                delete from dbo.OrganizationIdentifiers;
            ";

            context.Database.ExecuteSqlCommand(revertDataFromPatientIdentifier);
            context.Database.ExecuteSqlCommand(revertDataFromOrganizationIdentifier);
            context.Database.ExecuteSqlCommand(deleteFromPatientIdentifiers);
            context.Database.ExecuteSqlCommand(deleteFromOrganizationIdentifiers);
        }
    }
}
