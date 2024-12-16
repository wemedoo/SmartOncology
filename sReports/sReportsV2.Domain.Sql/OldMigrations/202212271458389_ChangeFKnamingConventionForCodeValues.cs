namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFKnamingConventionForCodeValues : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Addresses", name: "CountryId", newName: "CountryCD");
            RenameColumn(table: "dbo.Addresses", name: "AddressTypeId", newName: "AddressTypeCD");
            RenameColumn(table: "dbo.Codes", name: "CustomEnumId", newName: "CodeId");
            RenameColumn(table: "dbo.Codes", name: "Type", newName: "TypeCD");
            RenameColumn(table: "dbo.Users", name: "Prefix", newName: "PrefixCD");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "Status", newName: "StatusCD");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "Role", newName: "RoleCD");
            RenameColumn(table: "dbo.UserOrganizations", name: "State", newName: "StateCD");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "State", newName: "StateCD");
            RenameColumn(table: "dbo.Versions", name: "Type", newName: "TypeCD");
            RenameColumn(table: "dbo.Versions", name: "State", newName: "StateCD");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "Gender", newName: "GenderCD");
            RenameColumn(table: "dbo.EpisodeOfCares", name: "Status", newName: "StatusCD");
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "Status", newName: "StatusCD");
            RenameColumn(table: "dbo.Comments", name: "CommentState", newName: "CommentStateCD");
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "Source", newName: "SourceCD");
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "Status", newName: "StatusCD");
            RenameColumn(table: "dbo.Patients", name: "CitizenshipId", newName: "CitizenshipCD");
            RenameColumn(table: "dbo.Patients", name: "ReligionId", newName: "ReligionCD");
            RenameColumn(table: "dbo.Patients", name: "Gender", newName: "GenderCD");
            RenameColumn(table: "dbo.PatientAddresses", name: "CountryId", newName: "CountryCD");
            RenameColumn(table: "dbo.PatientAddresses", name: "AddressTypeId", newName: "AddressTypeCD");
            RenameColumn(table: "dbo.SimilarTerms", name: "Source", newName: "SourceCD");
            RenameColumn(table: "dbo.ThesaurusMerges", name: "State", newName: "StateCD");
            RenameIndex(table: "dbo.Addresses", name: "IX_CountryId", newName: "IX_CountryCD");
            RenameIndex(table: "dbo.Addresses", name: "IX_AddressTypeId", newName: "IX_AddressTypeCD");
            RenameIndex(table: "dbo.Patients", name: "IX_CitizenshipId", newName: "IX_CitizenshipCD");
            RenameIndex(table: "dbo.Patients", name: "IX_ReligionId", newName: "IX_ReligionCD");
            RenameIndex(table: "dbo.PatientAddresses", name: "IX_CountryId", newName: "IX_CountryCD");
            RenameIndex(table: "dbo.PatientAddresses", name: "IX_AddressTypeId", newName: "IX_AddressTypeCD");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PatientAddresses", name: "IX_AddressTypeCD", newName: "IX_AddressTypeId");
            RenameIndex(table: "dbo.PatientAddresses", name: "IX_CountryCD", newName: "IX_CountryId");
            RenameIndex(table: "dbo.Patients", name: "IX_ReligionCD", newName: "IX_ReligionId");
            RenameIndex(table: "dbo.Patients", name: "IX_CitizenshipCD", newName: "IX_CitizenshipId");
            RenameIndex(table: "dbo.Addresses", name: "IX_AddressTypeCD", newName: "IX_AddressTypeId");
            RenameIndex(table: "dbo.Addresses", name: "IX_CountryCD", newName: "IX_CountryId");
            RenameColumn(table: "dbo.ThesaurusMerges", name: "StateCD", newName: "State");
            RenameColumn(table: "dbo.SimilarTerms", name: "SourceCD", newName: "Source");
            RenameColumn(table: "dbo.PatientAddresses", name: "AddressTypeCD", newName: "AddressTypeId");
            RenameColumn(table: "dbo.PatientAddresses", name: "CountryCD", newName: "CountryId");
            RenameColumn(table: "dbo.Patients", name: "GenderCD", newName: "Gender");
            RenameColumn(table: "dbo.Patients", name: "ReligionCD", newName: "ReligionId");
            RenameColumn(table: "dbo.Patients", name: "CitizenshipCD", newName: "CitizenshipId");
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "StatusCD", newName: "Status");
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "SourceCD", newName: "Source");
            RenameColumn(table: "dbo.Comments", name: "CommentStateCD", newName: "CommentState");
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "StatusCD", newName: "Status");
            RenameColumn(table: "dbo.EpisodeOfCares", name: "StatusCD", newName: "Status");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "GenderCD", newName: "Gender");
            RenameColumn(table: "dbo.Versions", name: "StateCD", newName: "State");
            RenameColumn(table: "dbo.Versions", name: "TypeCD", newName: "Type");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "StateCD", newName: "State");
            RenameColumn(table: "dbo.UserOrganizations", name: "StateCD", newName: "State");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "RoleCD", newName: "Role");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "StatusCD", newName: "Status");
            RenameColumn(table: "dbo.Users", name: "PrefixCD", newName: "Prefix");
            RenameColumn(table: "dbo.Codes", name: "TypeCD", newName: "Type");
            RenameColumn(table: "dbo.Codes", name: "CodeId", newName: "CustomEnumId");
            RenameColumn(table: "dbo.Addresses", name: "AddressTypeCD", newName: "AddressTypeId");
            RenameColumn(table: "dbo.Addresses", name: "CountryCD", newName: "CountryId");
        }
    }
}
