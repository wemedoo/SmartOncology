namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using sReportsV2.Domain.Sql.Entities.ProjectEntry;

    public partial class AddInitialProjects : DbMigration
    {
        public override void Up()
        {
            using (var dbContext = new SReportsContext())
            {
                if (dbContext.Projects.Count() == 0)
                {
                    int projectTypeCD = GetCodeIdByPreferredTerm(dbContext, "Clinical Trial");

                    string sql = $@"
                    INSERT INTO Projects (ProjectName, ProjectTypeCD, ActiveFrom, ActiveTo, EntryDatetime, CreatedById, EntityStateCD, LastUpdate)
                    SELECT ClinicalTrialTitle, {projectTypeCD}, ActiveFrom, ActiveTo, EntryDatetime, CreatedById, EntityStateCD, LastUpdate
                    FROM ClinicalTrials";

                    dbContext.Database.ExecuteSqlCommand(sql);
                }
            }
        }

        public override void Down()
        {
        }

        private int GetCodeIdByPreferredTerm(SReportsContext dbContext, string preferredTerm)
        {
            return dbContext.Database.SqlQuery<int>(
                $@"SELECT TOP(1) code.CodeId
                from [dbo].[Codes] code
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                WHERE PreferredTerm = '{preferredTerm}'").FirstOrDefault();
        }
    }
}
