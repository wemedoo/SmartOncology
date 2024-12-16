namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetEntityStateValues : DbMigration
    {
        public override void Up()
        {
            string script =
                @"update [dbo].[Addresses]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Addresses]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Codes]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Codes]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Personnel]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Personnel]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Organizations]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Organizations]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[OrganizationClinicalDomains]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[OrganizationClinicalDomains]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[OrganizationIdentifiers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[OrganizationIdentifiers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[OrganizationRelations]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[OrganizationRelations]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelTeamOrganizationRelations]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelTeamOrganizationRelations]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelTeams]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelTeams]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelTeamRelations]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelTeamRelations]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Telecoms]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Telecoms]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelAcademicPositions]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelAcademicPositions]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelAddresses]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelAddresses]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelIdentifiers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelIdentifiers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PersonnelPositions]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PersonnelPositions]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ThesaurusEntries]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ThesaurusEntries]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ChemotherapySchemaInstances]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ChemotherapySchemaInstances]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ChemotherapySchemas]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ChemotherapySchemas]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Medications]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Medications]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ChemotherapySchemaInstanceVersions]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ChemotherapySchemaInstanceVersions]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[MedicationReplacements]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[MedicationReplacements]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[MedicationInstances]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[MedicationInstances]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[SmartOncologyPatients]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[SmartOncologyPatients]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[EpisodeOfCares]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[EpisodeOfCares]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[CodeAssociations]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[CodeAssociations]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[CodeSets]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[CodeSets]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Comments]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Comments]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientContacts]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientContacts]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Patients]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Patients]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientAddresses]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientAddresses]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientIdentifiers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientIdentifiers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientTelecoms]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientTelecoms]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientContactAddresses]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientContactAddresses]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PatientContactTelecoms]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PatientContactTelecoms]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[Encounters]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[Encounters]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[GlobalThesaurusRoles]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[GlobalThesaurusRoles]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[GlobalThesaurusUserRoles]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[GlobalThesaurusUserRoles]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[GlobalThesaurusUsers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[GlobalThesaurusUsers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[InboundAliases]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[InboundAliases]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[OutboundAliases]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[OutboundAliases]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[OutsideUsers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[OutsideUsers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[PositionPermissions]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[PositionPermissions]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ThesaurusMerges]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ThesaurusMerges]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[EncounterIdentifiers]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[EncounterIdentifiers]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ErrorMessageLogs]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ErrorMessageLogs]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[HL7MessageLog]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[HL7MessageLog]
                  set EntityStateCD=2001
                  where IsDeleted=0;

                  update [dbo].[ProcedeedMessageLogs]
                  set EntityStateCD=2003
                  where IsDeleted=1;

                  update [dbo].[ProcedeedMessageLogs]
                  set EntityStateCD=2001
                  where IsDeleted=0;
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(script);
        }

        public override void Down()
        {
        }
    }
}
