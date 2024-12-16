namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class AddProjectTypeCodeSet : DbMigration
    {
        public override void Up()
        {
            using (var dbContext = new SReportsContext())
            {
                if (dbContext.CodeSets.Any())
                {
                    int codeSetId = CreateCodeSet(dbContext, CodeSetAttributeNames.ProjectType, 87);

                    foreach (string term in ProjectTypes)
                        GetOrCreateCodeByPreferredTerm(dbContext, codeSetId, term);
                }
                
            }
        }

        public override void Down()
        {
        }

        private int CreateCodeSet(SReportsContext dbContext, string codeSetName, int codeSetId)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

            dbContext.Database.ExecuteSqlCommand($@"
            insert into CodeSets (CodeSetId, ThesaurusEntryId, EntryDatetime, EntityStateCD) values ({codeSetId}, {thesaurusId}, GETDATE(), 2001);
            ");

            return codeSetId;
        }

        private int GetOrCreateThesaurusId(SReportsContext dbContext, string preferredTerm, string definition = null)
        {
            int thesaurusId = dbContext.Database.SqlQuery<int>($@"SELECT ThesaurusEntryId FROM ThesaurusEntryTranslations WHERE PreferredTerm = '{preferredTerm}'").FirstOrDefault();
            if (thesaurusId <= 0)
            {
                thesaurusId = (int)dbContext.Database.SqlQuery<Decimal>($@"
                    INSERT INTO ThesaurusEntries (EntryDatetime) Values (GETDATE())
                    SELECT SCOPE_IDENTITY()
                    
                ").FirstOrDefault();

                dbContext.Database.ExecuteSqlCommand($@"INSERT INTO ThesaurusEntryTranslations (ThesaurusEntryId, Language, PreferredTerm, Definition) VALUES ({thesaurusId}, '{LanguageConstants.EN}', '{preferredTerm}', '{definition ?? preferredTerm}')");
            }
            return thesaurusId;
        }

        private int GetOrCreateCodeByPreferredTerm(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int existingCode = GetCodeIdByPreferredTerm(dbContext, codePreferredTerm);
            if (existingCode > 0)
            {
                UpdateCodeSetId(dbContext, existingCode, codeSetId);
                return existingCode;
            }
            else
            {
                int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm);

                int codeId = dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC").FirstOrDefault() + 1;

                dbContext.Database.ExecuteSqlCommand($@"
                SET IDENTITY_INSERT dbo.Codes ON;
                INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, EntryDatetime, EntityStateCD) values ({codeId}, {thesaurusId}, {codeSetId}, GETDATE(), 2001);
                SET IDENTITY_INSERT dbo.Codes OFF;");

                return codeId;
            }
        }

        private int GetCodeIdByPreferredTerm(SReportsContext dbContext, string preferredTerm)
        {
            return dbContext.Database.SqlQuery<int>(
                $@"SELECT TOP(1) code.CodeId
                from [dbo].[Codes] code
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                WHERE PreferredTerm = '{preferredTerm}' AND code.EntityStateCD != 2003").FirstOrDefault();
        }

        private void UpdateCodeSetId(SReportsContext dbContext, int codeId, int codeSetId)
        {
            dbContext.Database.ExecuteSqlCommand(
                   $@"UPDATE [dbo].Codes
                    SET CodeSetId = {codeSetId}
                    WHERE CodeId = '{codeId}'");
        }

        public static List<string> ProjectTypes { get; set; } = new List<string>()
        {
            "Clinical Trial"
        };
    }
}
