namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdatePersonnelClinicalTrialsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifier", c => c.String());
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialDataProviderIdentifier", c => c.String());
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifier", c => c.String(maxLength: 60));
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorName", c => c.String(maxLength: 300));
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialDataManagementProvider", c => c.String(maxLength: 300));
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifierTypeCD", c => c.Int());
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifierTypeCD", c => c.Int());
            AddColumn("dbo.PersonnelClinicalTrials", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.PersonnelClinicalTrials", "EntryDatetime", c => c.DateTime(nullable: false));
            AddColumn("dbo.PersonnelClinicalTrials", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.PersonnelClinicalTrials", "CreatedById", c => c.Int());
            AddColumn("dbo.PersonnelClinicalTrials", "ActiveFrom", c => c.DateTime(nullable: false));
            AddColumn("dbo.PersonnelClinicalTrials", "ActiveTo", c => c.DateTime(nullable: false));
            AddColumn("dbo.PersonnelClinicalTrials", "EntityStateCD", c => c.Int());
            CreateIndex("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifierTypeCD");
            CreateIndex("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifierTypeCD");
            CreateIndex("dbo.PersonnelClinicalTrials", "CreatedById");
            CreateIndex("dbo.PersonnelClinicalTrials", "EntityStateCD");
            AddForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifierTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifierTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PersonnelClinicalTrials", "CreatedById", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.PersonnelClinicalTrials", "EntityStateCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.PersonnelClinicalTrials", "SponosorId");
            DropColumn("dbo.PersonnelClinicalTrials", "WemedooId");
        }

        public override void Down()
        {
            AddColumn("dbo.PersonnelClinicalTrials", "WemedooId", c => c.String());
            AddColumn("dbo.PersonnelClinicalTrials", "SponosorId", c => c.String());
            DropForeignKey("dbo.PersonnelClinicalTrials", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelClinicalTrials", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifierTypeCD", "dbo.Codes");
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "EntityStateCD" });
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "ClinicalTrialSponsorIdentifierTypeCD" });
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "ClinicalTrialIdentifierTypeCD" });
            DropColumn("dbo.PersonnelClinicalTrials", "EntityStateCD");
            DropColumn("dbo.PersonnelClinicalTrials", "ActiveTo");
            DropColumn("dbo.PersonnelClinicalTrials", "ActiveFrom");
            DropColumn("dbo.PersonnelClinicalTrials", "CreatedById");
            DropColumn("dbo.PersonnelClinicalTrials", "LastUpdate");
            DropColumn("dbo.PersonnelClinicalTrials", "EntryDatetime");
            DropColumn("dbo.PersonnelClinicalTrials", "RowVersion");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifierTypeCD");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifierTypeCD");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialDataManagementProvider");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorName");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialIdentifier");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialDataProviderIdentifier");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialSponsorIdentifier");
        }
    }
}
