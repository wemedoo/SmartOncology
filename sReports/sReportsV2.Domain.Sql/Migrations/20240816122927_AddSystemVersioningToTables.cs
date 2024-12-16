using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemVersioningToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.EncounterIdentifiers");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.EncounterIdentifiers");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientLists");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientLists");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientListPersonnelRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientListPersonnelRelations");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientListPatientRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientListPatientRelations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelEncounterRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelEncounterRelations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ProjectPersonnelRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectPersonnelRelations");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ProjectDocumentRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectDocumentRelations");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ProjectPatientRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectPatientRelations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Projects");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Projects");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelOccupations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelOccupations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Tasks");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Tasks");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OrganizationAddresses");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationAddresses");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelTeams");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeams");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelTeamRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeamRelations");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelTeamOrganizationRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeamOrganizationRelations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelAcademicPositions");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelAcademicPositions");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientIdentifiers");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientIdentifiers");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelAddresses");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelAddresses");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelIdentifiers");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelIdentifiers");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PersonnelPositions");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelPositions");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PositionPermissions");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PositionPermissions");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Codes");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Codes");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.CodeSets");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.CodeSets");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.InboundAliases");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.InboundAliases");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OutboundAliases");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OutboundAliases");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientContactAddresses");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContactAddresses");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientContactTelecoms");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContactTelecoms");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientContacts");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContacts");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientTelecoms");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientTelecoms");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ChemotherapySchemaInstances");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemaInstances");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ChemotherapySchemaInstanceVersions");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemaInstanceVersions");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ChemotherapySchemas");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemas");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ClinicalTrials");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ClinicalTrials");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Comments");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Comments");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Encounters");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Encounters");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.EpisodeOfCares");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.EpisodeOfCares");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.GlobalThesaurusRoles");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusRoles");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.GlobalThesaurusUserRoles");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusUserRoles");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.GlobalThesaurusUsers");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusUsers");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.MedicationInstances");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.MedicationInstances");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.MedicationReplacements");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.MedicationReplacements");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Medications");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Medications");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OrganizationClinicalDomains");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationClinicalDomains");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OrganizationRelations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationRelations");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Organizations");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Organizations");

            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OrganizationTelecoms");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationTelecoms");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OutsideUserAddresses");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OutsideUserAddresses");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.OutsideUsers");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.OutsideUsers");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientAddresses");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientAddresses");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Patients");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Patients");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Personnel");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Personnel");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.SmartOncologyPatients");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.SmartOncologyPatients");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ThesaurusEntries");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ThesaurusEntries");
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.ThesaurusMerges");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.ThesaurusMerges");

            var createFunction = @"
                CREATE OR ALTER FUNCTION NumOfExtendedProperties ( @tableName sysname, @columnName sysname = NULL) 
                RETURNS int
                AS
                BEGIN
                    DECLARE @numOfExtendedProperties int;
                    DECLARE @level2type varchar(128);
                    IF @columnName IS NULL
                        SET @level2type = @columnName;
                    ELSE
                        SET @level2type = 'Column';
                    SELECT @numOfExtendedProperties = COUNT(name)
                        FROM ::fn_listextendedproperty ('Description','Schema', 'dbo', 'Table', @tableName, @level2type, @columnName);
                    RETURN @numOfExtendedProperties;
                END;
            ";

            var createStoredProcedure = @"
                CREATE OR ALTER PROCEDURE AddExtendedProperty 
                    @description sql_variant, 
                    @tableName sysname, 
                    @columnName sysname = NULL
                AS
                BEGIN
                    DECLARE @numOfExtendedProperties int = [dbo].[NumOfExtendedProperties] (@tableName, @columnName);
                    DECLARE @extendedPropertyProcedureName varchar(128) = 'sys.sp_addextendedproperty';
                    IF @numOfExtendedProperties > 0
                        SET @extendedPropertyProcedureName = 'sys.sp_updateextendedproperty';
                        DECLARE @level2type sysname = 'Column';
                    IF @columnName IS NULL
                        SET @level2type = @columnName;
                    EXEC @extendedPropertyProcedureName
                        @name = 'Description',   
                        @value = @description,   
                        @level0type = 'SCHEMA', 
                        @level0name = 'dbo',  
                        @level1type = 'TABLE',  
                        @level1name = @tableName,
                        @level2type = @level2type,  
                        @level2name = @columnName;
                END;
            ";

            var createInsertSpacesBetweenCapitalLetters = @"
                CREATE FUNCTION dbo.InsertSpacesBetweenCapitalLetters(@inputString NVARCHAR(MAX))
                RETURNS NVARCHAR(MAX)
                AS
                BEGIN
                    DECLARE @outputString NVARCHAR(MAX) = '';
                    DECLARE @len INT = LEN(@inputString);
                    DECLARE @i INT = 1;

                    WHILE @i <= @len
                    BEGIN
                        IF UNICODE(SUBSTRING(@inputString, @i, 1)) BETWEEN UNICODE('A') AND UNICODE('Z')
                            AND @i > 1
                            AND UNICODE(SUBSTRING(@inputString, @i - 1, 1)) BETWEEN UNICODE('a') AND UNICODE('z')
                        BEGIN
                            SET @outputString += ' ';
                        END

                        SET @outputString += SUBSTRING(@inputString, @i, 1);
                        SET @i += 1;
                    END

                    RETURN @outputString;
                END;
            GO
            ";

            migrationBuilder.Sql(createFunction);
            migrationBuilder.Sql(createStoredProcedure);
            migrationBuilder.Sql(createInsertSpacesBetweenCapitalLetters);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ThesaurusMerges");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ThesaurusMerges");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ThesaurusEntries");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ThesaurusEntries");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.SmartOncologyPatients");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.SmartOncologyPatients");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Personnel");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Personnel");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Patients");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Patients");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientAddresses");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientAddresses");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OutsideUsers");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OutsideUsers");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OutsideUserAddresses");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OutsideUserAddresses");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationTelecoms");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OrganizationTelecoms");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Organizations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Organizations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OrganizationRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationClinicalDomains");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OrganizationClinicalDomains");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Medications");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Medications");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.MedicationReplacements");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.MedicationReplacements");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.MedicationInstances");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.MedicationInstances");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusUsers");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.GlobalThesaurusUsers");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusUserRoles");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.GlobalThesaurusUserRoles");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.GlobalThesaurusRoles");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.GlobalThesaurusRoles");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.EpisodeOfCares");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.EpisodeOfCares");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Encounters");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Encounters");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Comments");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Comments");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ClinicalTrials");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ClinicalTrials");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemas");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ChemotherapySchemas");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemaInstanceVersions");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ChemotherapySchemaInstanceVersions");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ChemotherapySchemaInstances");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ChemotherapySchemaInstances");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContactAddresses");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientContactAddresses");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContactTelecoms");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientContactTelecoms");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientContacts");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientContacts");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientTelecoms");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientTelecoms");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Codes");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Codes");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.CodeSets");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.CodeSets");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.InboundAliases");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.InboundAliases");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OutboundAliases");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OutboundAliases");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelPositions");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelPositions");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PositionPermissions");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PositionPermissions");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelAcademicPositions");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelAcademicPositions");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientIdentifiers");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientIdentifiers");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelAddresses");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelAddresses");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelIdentifiers");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelIdentifiers");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeams");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelTeams");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeamRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelTeamRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelTeamOrganizationRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelTeamOrganizationRelations");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.OrganizationAddresses");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.OrganizationAddresses");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Tasks");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Tasks");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelOccupations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelOccupations");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Projects");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Projects");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.EncounterIdentifiers");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.EncounterIdentifiers");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientLists");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientLists");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientListPersonnelRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientListPersonnelRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientListPatientRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientListPatientRelations");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ClinicalTrialPersonnelRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ClinicalTrialPersonnelRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ClinicalTrialPatientRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ClinicalTrialPatientRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ClinicalTrialDocumentRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ClinicalTrialDocumentRelations");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PersonnelEncounterRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PersonnelEncounterRelations");

            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectPersonnelRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ProjectPersonnelRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectDocumentRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ProjectDocumentRelations");
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.ProjectPatientRelations");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.ProjectPatientRelations");
        }
    }
}
