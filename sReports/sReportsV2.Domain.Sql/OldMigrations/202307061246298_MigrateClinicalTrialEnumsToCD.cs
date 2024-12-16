namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateClinicalTrialEnumsToCD : DbMigration
    {

        private List<Tuple<int, string>> statuses = new List<Tuple<int, string>>()
        {
            new Tuple<int, string>(0, "Not yet recruiting"),
            new Tuple<int, string>(1, "Recruiting"),
            new Tuple<int, string>(2, "Enrolling by invitation"),
            new Tuple<int, string>(3, "Not recruiting"),
            new Tuple<int, string>(4, "Suspended"),
            new Tuple<int, string>(5, "Terminated"),
            new Tuple<int, string>(6, "Completed"),
            new Tuple<int, string>(7, "Withdrawn"),
            new Tuple<int, string>(8, "Unknown"),
            new Tuple<int, string>(9, "Active")        
        };

        private List<Tuple<int, string>> roles = new List<Tuple<int, string>>()
        {
            new Tuple<int, string>(0, "Principal investigator"),
            new Tuple<int, string>(1, "Co-Investigator"),
            new Tuple<int, string>(2, "Study nurse"),
            new Tuple<int, string>(3, "Clinical research administrator"),
            new Tuple<int, string>(4, "Medical monitor"),
            new Tuple<int, string>(5, "Clinical trial pharmacist")        
        };
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();

            StatusEnumToCD(dbContext);
            RoleEnumToCD(dbContext);

            DropColumn("dbo.PersonnelClinicalTrials", "StatusCD");
            DropColumn("dbo.PersonnelClinicalTrials", "RoleCD");
        }

        public override void Down()
        {
            AddColumn("dbo.PersonnelClinicalTrials", "RoleCD", c => c.Int());
            AddColumn("dbo.PersonnelClinicalTrials", "StatusCD", c => c.Int());
        }

        private void StatusEnumToCD(SReportsContext dbContext)
        {
            int statusCodeSetId = (int)CodeSetList.ClinicalTrialRecruitmentsStatus;

            string updateCmd = "";

            foreach (var status in statuses)
            {
                updateCmd += $@"
                UPDATE dbo.PersonnelClinicalTrials
                SET ClinicalTrialRecruitmentStatusCD = (
                                SELECT TOP (1) CodeId
				                From dbo.Codes code 
				                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
				                WHERE CodeSetId = {statusCodeSetId} AND PreferredTerm = '{status.Item2}')
                WHERE StatusCD = {status.Item1};
                ";
            }

            dbContext.Database.ExecuteSqlCommand(updateCmd);
        }

        private void RoleEnumToCD(SReportsContext dbContext)
        {
            int roleCodesetId = (int)CodeSetList.ClinicalTrialRole;

            string updateCmd = "";

            foreach (var role in roles)
            {
                updateCmd += $@"
                UPDATE dbo.PersonnelClinicalTrials
                SET ClinicalTrialRoleCD = (
                                SELECT TOP (1) CodeId
				                From dbo.Codes code 
				                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
				                WHERE CodeSetId = {roleCodesetId} AND PreferredTerm = '{role.Item2}')
                WHERE RoleCD = {role.Item1};
                ";
            }

            dbContext.Database.ExecuteSqlCommand(updateCmd);
        }
    }
}
