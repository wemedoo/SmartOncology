namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveActiveColumnFromEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Addresses", "Active");
            DropColumn("dbo.Codes", "Active");
            DropColumn("dbo.Personnel", "Active");
            DropColumn("dbo.Organizations", "Active");
            DropColumn("dbo.OrganizationClinicalDomains", "Active");
            DropColumn("dbo.OrganizationIdentifiers", "Active");
            DropColumn("dbo.OrganizationRelations", "Active");
            DropColumn("dbo.PersonnelTeamOrganizationRelations", "Active");
            DropColumn("dbo.PersonnelTeams", "Active");
            DropColumn("dbo.PersonnelTeamRelations", "Active");
            DropColumn("dbo.Telecoms", "Active");
            DropColumn("dbo.PersonnelAcademicPositions", "Active");
            DropColumn("dbo.PersonnelAddresses", "Active");
            DropColumn("dbo.PersonnelIdentifiers", "Active");
            DropColumn("dbo.PersonnelPositions", "Active");
            DropColumn("dbo.ThesaurusEntries", "Active");
            DropColumn("dbo.ChemotherapySchemaInstances", "Active");
            DropColumn("dbo.ChemotherapySchemas", "Active");
            DropColumn("dbo.Medications", "Active");
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "Active");
            DropColumn("dbo.MedicationReplacements", "Active");
            DropColumn("dbo.MedicationInstances", "Active");
            DropColumn("dbo.SmartOncologyPatients", "Active");
            DropColumn("dbo.EpisodeOfCares", "Active");
            DropColumn("dbo.CodeAssociations", "Active");
            DropColumn("dbo.CodeSets", "Active");
            DropColumn("dbo.Comments", "Active");
            DropColumn("dbo.PatientContacts", "Active");
            DropColumn("dbo.Patients", "Active");
            DropColumn("dbo.PatientAddresses", "Active");
            DropColumn("dbo.PatientIdentifiers", "Active");
            DropColumn("dbo.PatientTelecoms", "Active");
            DropColumn("dbo.PatientContactAddresses", "Active");
            DropColumn("dbo.PatientContactTelecoms", "Active");
            DropColumn("dbo.Encounters", "Active");
            DropColumn("dbo.GlobalThesaurusRoles", "Active");
            DropColumn("dbo.GlobalThesaurusUserRoles", "Active");
            DropColumn("dbo.GlobalThesaurusUsers", "Active");
            DropColumn("dbo.InboundAliases", "Active");
            DropColumn("dbo.OutboundAliases", "Active");
            DropColumn("dbo.OutsideUsers", "Active");
            DropColumn("dbo.PositionPermissions", "Active");
            DropColumn("dbo.ThesaurusMerges", "Active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ThesaurusMerges", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PositionPermissions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutsideUsers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutboundAliases", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.InboundAliases", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUsers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUserRoles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusRoles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Encounters", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContactTelecoms", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContactAddresses", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientTelecoms", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientIdentifiers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientAddresses", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patients", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContacts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.CodeSets", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.CodeAssociations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.EpisodeOfCares", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.SmartOncologyPatients", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationInstances", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationReplacements", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Medications", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemas", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstances", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThesaurusEntries", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelPositions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelIdentifiers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelAddresses", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelAcademicPositions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Telecoms", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeamRelations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeams", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonnelTeamOrganizationRelations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationRelations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationIdentifiers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationClinicalDomains", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Personnel", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Codes", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "Active", c => c.Boolean(nullable: false));
        }
    }
}
