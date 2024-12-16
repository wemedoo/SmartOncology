namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class UpdateProjectIdInClinicalTrials : DbMigration
    {
        public override void Up()
        {
            string sqlCommand = @"
                WITH CTE_ClinicalTrials AS (
                    SELECT ClinicalTrialId, ProjectId, ROW_NUMBER() OVER(ORDER BY ClinicalTrialId) AS RowNum 
                    FROM ClinicalTrials
                ), CTE_Projects AS (
                    SELECT ProjectId, ROW_NUMBER() OVER(ORDER BY ProjectId) AS RowNum 
                    FROM Projects
                )
                UPDATE CTE_ClinicalTrials
                SET ProjectId = (SELECT ProjectId FROM CTE_Projects WHERE CTE_Projects.RowNum = CTE_ClinicalTrials.RowNum);
            ";

            Sql(sqlCommand);
        }

        public override void Down()
        {
        }
    }
}
