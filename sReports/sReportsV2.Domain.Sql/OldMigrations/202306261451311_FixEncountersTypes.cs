namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class FixEncountersTypes : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext dbContext = new SReportsContext())
            {
                if(dbContext.CodeSets.Any())
                {
                    int codesetId = GetOrCreateCodeSet(dbContext, CodeSetAttributeNames.EncounterType);

                    string updateCmd = $@"
                    UPDATE [dbo].Codes
                    SET CodeSetId = {codesetId}
                    FROM [dbo].Encounters enc
                    inner join [dbo].[Codes] code on enc.TypeCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    WHERE (tranThCode.PreferredTerm = 'Not Applicable' 
                    OR  tranThCode.PreferredTerm = 'Encounter for check up (procedure)'
                    OR  tranThCode.PreferredTerm = 'Encounter for symptom'
                    OR  tranThCode.PreferredTerm = 'Encounter for problem'
                    OR  tranThCode.PreferredTerm = 'Follow-up encounter'
                    OR  tranThCode.PreferredTerm = 'Follow-up encounter (procedure)') 
                    AND tranThCode.Language = 'en'
                    AND CodeSetId IS NULL;";

                    dbContext.Database.ExecuteSqlCommand(updateCmd);
                }
                
            }

            
        }

        public override void Down()
        {
        }

        private int GetOrCreateCodeSet(SReportsContext dbContext, string codeSetName)
        {
            int codeSetId = GetCodeSetIdByPreferredTerm(dbContext, codeSetName);
            if (codeSetId <= 0)
            {
                codeSetId = GetLastAvailableCodeset(dbContext);
                int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

                dbContext.Database.ExecuteSqlCommand($@"
                insert into CodeSets (CodeSetId, ThesaurusEntryId, EntryDatetime, EntityStateCD) values ({codeSetId}, {thesaurusId}, GETDATE(), 2001);
            ");

            }
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

        private int GetCodeSetIdByPreferredTerm(SReportsContext dbContext, string preferredTerm)
        {
            return dbContext.Database.SqlQuery<int>(
                $@"SELECT TOP(1) codeset.CodeSetId
                from [dbo].[CodeSets] codeset
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = codeset.ThesaurusEntryId
                WHERE PreferredTerm = '{preferredTerm}' AND codeset.EntityStateCD != 2003").FirstOrDefault();
        }

        private int GetLastAvailableCodeset(SReportsContext dbContext)
        {
            return dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeSetId FROM CodeSets ORDER BY CodeSetId DESC").FirstOrDefault() + 1;
        }
    }
}
