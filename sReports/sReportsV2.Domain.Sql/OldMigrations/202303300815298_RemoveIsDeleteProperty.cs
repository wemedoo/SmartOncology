namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsDeleteProperty : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Addresses", "IsDeleted");
            DropColumn("dbo.Codes", "IsDeleted");
            DropColumn("dbo.Personnel", "IsDeleted");
            DropColumn("dbo.Organizations", "IsDeleted");
            DropColumn("dbo.OrganizationClinicalDomains", "IsDeleted");
            DropColumn("dbo.OrganizationIdentifiers", "IsDeleted");
            DropColumn("dbo.OrganizationRelations", "IsDeleted");
            DropColumn("dbo.PersonnelTeamOrganizationRelations", "IsDeleted");
            DropColumn("dbo.PersonnelTeams", "IsDeleted");
            DropColumn("dbo.PersonnelTeamRelations", "IsDeleted");
            DropColumn("dbo.Telecoms", "IsDeleted");
            DropColumn("dbo.PersonnelAcademicPositions", "IsDeleted");
            DropColumn("dbo.PersonnelAddresses", "IsDeleted");
            DropColumn("dbo.PersonnelIdentifiers", "IsDeleted");
            DropColumn("dbo.PersonnelPositions", "IsDeleted");
            DropColumn("dbo.ThesaurusEntries", "IsDeleted");
            DropColumn("dbo.ChemotherapySchemaInstances", "IsDeleted");
            DropColumn("dbo.ChemotherapySchemas", "IsDeleted");
            DropColumn("dbo.Medications", "IsDeleted");
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "IsDeleted");
            DropColumn("dbo.MedicationReplacements", "IsDeleted");
            DropColumn("dbo.MedicationInstances", "IsDeleted");
            DropColumn("dbo.SmartOncologyPatients", "IsDeleted");
            DropColumn("dbo.EpisodeOfCares", "IsDeleted");
            DropColumn("dbo.CodeAssociations", "IsDeleted");
            DropColumn("dbo.CodeSets", "IsDeleted");
            DropColumn("dbo.Comments", "IsDeleted");
            DropColumn("dbo.PatientContacts", "IsDeleted");
            DropColumn("dbo.Patients", "IsDeleted");
            DropColumn("dbo.PatientAddresses", "IsDeleted");
            DropColumn("dbo.PatientIdentifiers", "IsDeleted");
            DropColumn("dbo.PatientTelecoms", "IsDeleted");
            DropColumn("dbo.PatientContactAddresses", "IsDeleted");
            DropColumn("dbo.PatientContactTelecoms", "IsDeleted");
            DropColumn("dbo.Encounters", "IsDeleted");
            DropColumn("dbo.GlobalThesaurusRoles", "IsDeleted");
            DropColumn("dbo.GlobalThesaurusUserRoles", "IsDeleted");
            DropColumn("dbo.GlobalThesaurusUsers", "IsDeleted");
            DropColumn("dbo.InboundAliases", "IsDeleted");
            DropColumn("dbo.OutboundAliases", "IsDeleted");
            DropColumn("dbo.OutsideUsers", "IsDeleted");
            DropColumn("dbo.PositionPermissions", "IsDeleted");
            DropColumn("dbo.ThesaurusMerges", "IsDeleted");
            DropColumn("dbo.EncounterIdentifiers", "IsDeleted");
            DropColumn("dbo.ErrorMessageLogs", "IsDeleted");
            DropColumn("dbo.HL7MessageLog", "IsDeleted");
            DropColumn("dbo.ProcedeedMessageLogs", "IsDeleted");
        }

        public override void Down()
        {
            AddColumn("dbo.ProcedeedMessageLogs", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.HL7MessageLog", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ErrorMessageLogs", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.EncounterIdentifiers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThesaurusMerges", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PositionPermissions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutsideUsers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutboundAliases", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.InboundAliases", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUsers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUserRoles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusRoles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Encounters", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContactTelecoms", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContactAddresses", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientTelecoms", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientIdentifiers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientAddresses", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patients", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContacts", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.CodeSets", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.CodeAssociations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.EpisodeOfCares", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SmartOncologyPatients", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationInstances", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationReplacements", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Medications", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstances", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThesaurusEntries", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelPositions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelIdentifiers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelAddresses", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelAcademicPositions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Telecoms", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeamRelations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeams", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeamOrganizationRelations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationRelations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationIdentifiers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationClinicalDomains", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Personnel", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Codes", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "IsDeleted", c => c.Boolean(nullable: false));
        }
    }
}
