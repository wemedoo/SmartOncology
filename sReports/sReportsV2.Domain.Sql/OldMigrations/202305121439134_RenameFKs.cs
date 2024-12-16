namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class RenameFKs : DbMigration
    {
        public override void Up()
        {
            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.ExecuteRenameFKs(GetForeignKeyToBeRenamed(), isUpMigration: true);
        }

        public override void Down()
        {
            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.ExecuteRenameFKs(GetForeignKeyToBeRenamed(), isUpMigration: false);
        }

        private List<Tuple<string, string>> GetForeignKeyToBeRenamed()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string, string>       ("FK_dbo.ChemotherapySchemaInstances_dbo.Users_CreatedById","FK_dbo.ChemotherapySchemaInstances_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemaInstances_dbo.Users_Creator_Id",        "FK_dbo.ChemotherapySchemaInstances_dbo.Personnel_Creator_Id"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemaInstanceVersions_dbo.Users_CreatorId",              "FK_dbo.ChemotherapySchemaInstanceVersions_dbo.Personnel_CreatorId"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemaInstanceVersions_dbo.Users_CreatedById",                "FK_dbo.ChemotherapySchemaInstanceVersions_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemas_dbo.Users_CreatorId", "FK_dbo.ChemotherapySchemas_dbo.Personnel_CreatorId"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemas_dbo.Users_CreatedById", "FK_dbo.ChemotherapySchemas_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.CodeAssociations_dbo.Users_CreatedById", "FK_dbo.CodeAssociations_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.CustomEnums_dbo.Users_CreatedById", "FK_dbo.Codes_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.CustomEnums_dbo.ThesaurusEntries_ThesaurusEntryId", "FK_dbo.Codes_dbo.ThesaurusEntries_ThesaurusEntryId"),
                new Tuple<string, string>("FK_dbo.CodeSets_dbo.Users_CreatedById", "FK_dbo.CodeSets_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Comments_dbo.Users_CreatedById", "FK_dbo.Comments_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Encounters_dbo.Users_CreatedById", "FK_dbo.Encounters_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.EpisodeOfCares_dbo.Users_CreatedById", "FK_dbo.EpisodeOfCares_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.ErrorMessageLogs_dbo.HL7MessageLog_HL7MessageLogId",      "FK_dbo.ErrorMessageLogs_dbo.HL7MessageLogs_HL7MessageLogId"),
                new Tuple<string, string>("FK_dbo.GlobalThesaurusRoles_dbo.Users_CreatedById", "FK_dbo.GlobalThesaurusRoles_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.GlobalThesaurusUserRoles_dbo.Users_CreatedById",  "FK_dbo.GlobalThesaurusUserRoles_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.GlobalThesaurusUserRoles_dbo.GlobalThesaurusRoles_RoleId",                "FK_dbo.GlobalThesaurusUserRoles_dbo.GlobalThesaurusRoles_GlobalThesaurusRoleId"),
                new Tuple<string, string>("FK_dbo.GlobalThesaurusUserRoles_dbo.GlobalThesaurusUsers_UserId",                "FK_dbo.GlobalThesaurusUserRoles_dbo.GlobalThesaurusUsers_GlobalThesaurusUserId"),
                new Tuple<string, string>("FK_dbo.GlobalThesaurusUsers_dbo.Users_CreatedById", "FK_dbo.GlobalThesaurusUsers_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.HL7MessageLog_dbo.Codes_EntityStateCD", "FK_dbo.HL7MessageLogs_dbo.Codes_EntityStateCD"),
                new Tuple<string, string>("FK_dbo.HL7MessageLog_dbo.Personnel_CreatedById", "FK_dbo.HL7MessageLogs_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.InboundAliases_dbo.Users_CreatedById", "FK_dbo.InboundAliases_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.MedicationInstances_dbo.Users_CreatedById", "FK_dbo.MedicationInstances_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.MedicationReplacements_dbo.Users_CreatorId", "FK_dbo.MedicationReplacements_dbo.Personnel_CreatorId"),
                new Tuple<string, string>("FK_dbo.MedicationReplacements_dbo.Users_CreatedById", "FK_dbo.MedicationReplacements_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Medications_dbo.Users_CreatedById", "FK_dbo.Medications_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.OrganizationClinicalDomains_dbo.Users_CreatedById",       "FK_dbo.OrganizationClinicalDomains_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.OrganizationRelations_dbo.Users_CreatedById", "FK_dbo.OrganizationRelations_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Organizations_dbo.Users_CreatedById", "FK_dbo.Organizations_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.OutboundAliases_dbo.Users_CreatedById", "FK_dbo.OutboundAliases_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.OutsideUsers_dbo.Users_CreatedById", "FK_dbo.OutsideUsers_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.PatientAddresses_dbo.Users_CreatedById", "FK_dbo.PatientAddresses_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.PatientAddresses_dbo.CustomEnums_AddressTypeId", "FK_dbo.PatientAddresses_dbo.Codes_AddressTypeCD"),
                new Tuple<string, string>("FK_dbo.PatientAddresses_dbo.CustomEnums_CountryId", "FK_dbo.PatientAddresses_dbo.Codes_CountryCD"),
                new Tuple<string, string>("FK_dbo.PatientContactAddresses_dbo.CustomEnums_AddressTypeId",       "FK_dbo.PatientContactAddresses_dbo.Codes_AddressTypeCD"),
                new Tuple<string, string>("FK_dbo.PatientContactAddresses_dbo.CustomEnums_CountryId", "FK_dbo.PatientContactAddresses_dbo.Codes_CountryCD"),
                new Tuple<string, string>("FK_dbo.PatientContactAddresses_dbo.Users_CreatedById", "FK_dbo.PatientContactAddresses_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.PatientContacts_dbo.Users_CreatedById", "FK_dbo.PatientContacts_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.PatientContactTelecoms_dbo.Users_CreatedById", "FK_dbo.PatientContactTelecoms_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Patients_dbo.Users_CreatedById", "FK_dbo.Patients_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Patients_dbo.CustomEnums_CitizenshipId", "FK_dbo.Patients_dbo.Codes_CitizenshipCD"),
                new Tuple<string, string>("FK_dbo.Patients_dbo.CustomEnums_ReligionId", "FK_dbo.Patients_dbo.Codes_ReligionCD"),
                new Tuple<string, string>("FK_dbo.PatientTelecoms_dbo.Users_CreatedById", "FK_dbo.PatientTelecoms_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Users_dbo.UserConfigs_UserConfigId", "FK_dbo.Personnel_dbo.PersonnelConfigs_PersonnelConfigId"),
                new Tuple<string, string>("FK_dbo.Users_dbo.Users_CreatedById", "FK_dbo.Personnel_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.SmartOncologyPatients_dbo.Users_CreatedById", "FK_dbo.SmartOncologyPatients_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Telecoms_dbo.Users_CreatedById", "FK_dbo.OrganizationTelecoms_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.Telecoms_dbo.Codes_EntityStateCD", "FK_dbo.OrganizationTelecoms_dbo.Codes_EntityStateCD"),
                new Tuple<string, string>("FK_dbo.Telecoms_dbo.Codes_SystemCD", "FK_dbo.OrganizationTelecoms_dbo.Codes_SystemCD"),
                new Tuple<string, string>("FK_dbo.Telecoms_dbo.Codes_UseCD", "FK_dbo.OrganizationTelecoms_dbo.Codes_UseCD"),
                new Tuple<string, string>("FK_dbo.Telecoms_dbo.Organizations_Organization_Id",  "FK_dbo.OrganizationTelecoms_dbo.Organizations_Organization_Id"),
                new Tuple<string, string>("FK_dbo.ThesaurusEntries_dbo.Users_CreatedById", "FK_dbo.ThesaurusEntries_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.ThesaurusMerges_dbo.Users_CreatedById", "FK_dbo.ThesaurusMerges_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Codes_EntityStateCD", "FK_dbo.Transactions_dbo.Codes_EntityStateCD"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Codes_SourceSystemCD", "FK_dbo.Transactions_dbo.Codes_SourceSystemCD"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Codes_TransactionDirectionCD",   "FK_dbo.Transactions_dbo.Codes_TransactionDirectionCD"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Encounters_EncounterId", "FK_dbo.Transactions_dbo.Encounters_EncounterId"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.HL7MessageLog_HL7MessageLogId",      "FK_dbo.Transactions_dbo.HL7MessageLog_HL7MessageLogId"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Patients_PatientId", "FK_dbo.Transactions_dbo.Patients_PatientId"),
                new Tuple<string, string>("FK_dbo.ProcedeedMessageLogs_dbo.Personnel_CreatedById", "FK_dbo.Transactions_dbo.Personnel_CreatedById"),
                new Tuple<string, string>("FK_dbo.UserClinicalTrials_dbo.Users_UserId", "FK_dbo.PersonnelClinicalTrials_dbo.Personnel_PersonnelId"),
                new Tuple<string, string>("FK_dbo.UserConfigs_dbo.Organizations_ActiveOrganizationId",          "FK_dbo.PersonnelConfigs_dbo.Organizations_ActiveOrganizationId"),
                new Tuple<string, string>("FK_dbo.UserOrganizations_dbo.Users_UserId", "FK_dbo.PersonnelOrganizations_dbo.Personnel_PersonnelId"),
                new Tuple<string, string>("FK_dbo.UserOrganizations_dbo.Organizations_OrganizationId", "FK_dbo.PersonnelOrganizations_dbo.Organizations_OrganizationId")
            };
        }
    }
}
