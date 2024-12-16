namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameClinicalTrialTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.PersonnelClinicalTrials", "PersonnelClinicalTrialId", "ClinicalTrialId");
            DropForeignKey("dbo.PersonnelClinicalTrials", "PersonnelId", "dbo.Personnel");
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "PersonnelId" });
            DropColumn("dbo.PersonnelClinicalTrials", "PersonnelId");

            DropForeignKey("dbo.PersonnelClinicalTrials", name: "FK_dbo.PersonnelClinicalTrials_dbo.Codes_ClinicalTrialRoleCD", "dbo.Codes");
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "ClinicalTrialRoleCD" });
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD");

            RenameTable("dbo.PersonnelClinicalTrials", "ClinicalTrials");
        }

        public override void Down()
        {
            RenameTable("dbo.ClinicalTrials", "PersonnelClinicalTrials");

            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD", c => c.Int());
            CreateIndex("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD");
            AddForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD", "dbo.Codes", "CodeId");

            AddColumn("dbo.PersonnelClinicalTrials", "PersonnelId", c => c.Int(nullable: true));
            CreateIndex("dbo.PersonnelClinicalTrials", new[] { "PersonnelId" });
            AddForeignKey("dbo.PersonnelClinicalTrials", "PersonnelId", "dbo.Personnel");
            RenameColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialId", "PersonnelClinicalTrialId");
        }
    }
}
