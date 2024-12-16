namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.Common;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    
    public partial class PersonnelClinicalTrialTypesAndNamingConventions : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.PersonnelClinicalTrials", "Name", "ClinicalTrialTitle");
            RenameColumn("dbo.PersonnelClinicalTrials", "Acronym", "ClinicalTrialAcronym");
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRecruitmentStatusCD", c => c.Int());
            AddColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD", c => c.Int());
            CreateIndex("dbo.PersonnelClinicalTrials", "ClinicalTrialRecruitmentStatusCD");
            CreateIndex("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD");
            AddForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialRecruitmentStatusCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD", "dbo.Codes", "CodeId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelClinicalTrials", "ClinicalTrialRecruitmentStatusCD", "dbo.Codes");
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "ClinicalTrialRoleCD" });
            DropIndex("dbo.PersonnelClinicalTrials", new[] { "ClinicalTrialRecruitmentStatusCD" });
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRoleCD");
            DropColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialRecruitmentStatusCD");
            RenameColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialTitle", "Name");
            RenameColumn("dbo.PersonnelClinicalTrials", "ClinicalTrialAcronym", "Acronym");
        }
    }
}
