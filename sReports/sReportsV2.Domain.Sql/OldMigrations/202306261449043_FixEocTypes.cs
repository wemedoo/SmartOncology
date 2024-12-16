namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.Common;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class FixEocTypes : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext dbContext = new SReportsContext())
            {
                if(dbContext.CodeSets.Any())
                {
                    // ---

                    int codeSetId = GetOrCreateCodeSet(dbContext, CodeSetAttributeNames.EpisodeOfCareType);

                    // --- 

                    SubstituteEoCTypeCDByPreferredTerm(dbContext, "Bashkir", "Radiation oncology treatment", codeSetId);
                    SubstituteEoCTypeCDByPreferredTerm(dbContext,
                        "Treatment of Oncological Disease - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
                        "Treatment of Oncological Diseases - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
                        codeSetId);

                    // --- 

                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Home and Community Care");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Post Acute Care");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Post coordinated diabetes program");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Drug and alcohol rehabilitation");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Community-based aged care");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "General Prevention Program of Oncological Diseases");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Prevention of Oncological Diseases - Municipality Program for Smoking Cesation");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Screening for Oncological Diseases - Colorectal Cancer Screening");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Screening for Oncological Diseases - Cantonal Program for Breast Cancer Screening Bern");
                    UpdateEocTypeCDWIthNewCodeId(dbContext, codeSetId, "Treatment of Oncological Diseases - Head and Neck Cancer Clinical Pathway of Inselspital Bern");

                    dbContext.Database.ExecuteSqlCommand(
                        $@"UPDATE [dbo].Codes
                SET CodeSetId = {codeSetId}
                FROM [dbo].[EpisodeOfCares] eoc
                inner join [dbo].[Codes] code on eoc.TypeCD = code.ThesaurusEntryId
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                WHERE CodeSetId IS NULL;");
                }
            }
        }

        public override void Down()
        {
        }


        private void SubstituteEoCTypeCDByPreferredTerm(SReportsContext dbContext, string termToUpdate, string newTerm, int codeSetId)
        {
            int newCodeId = GetOrCreateCodeByPreferredTerm(dbContext, codeSetId, newTerm); // change newCodeId to CodeId


            SubstituteEoCTypeCDByCodeId(dbContext, termToUpdate, newCodeId);
        }

        private void SubstituteEoCTypeCDByCodeId(SReportsContext dbContext, string termToUpdate, int codeId)
        {
            if (codeId > 0)
            {
                dbContext.Database.ExecuteSqlCommand(
                    $@"UPDATE [dbo].EpisodeOfCares
                    SET TypeCD = {codeId}
                    FROM [dbo].[EpisodeOfCares] eoc
                    inner join [dbo].[Codes] code on eoc.TypeCD = code.CodeId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    WHERE PreferredTerm = '{termToUpdate}'");

                dbContext.Database.ExecuteSqlCommand(
                    $@"UPDATE [dbo].EpisodeOfCares
                    SET TypeCD = {codeId}
                    FROM [dbo].[EpisodeOfCares] eoc
                    inner join [dbo].[Codes] code on eoc.TypeCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    WHERE PreferredTerm = '{termToUpdate}'");
            }
        }

        private void UpdateEocTypeCDWIthNewCodeId(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int codeId = GetOrCreateCodeByPreferredTerm(dbContext, codeSetId, codePreferredTerm);
            SubstituteEoCTypeCDByCodeId(dbContext, codePreferredTerm, codeId);
        }


        private int GetOrCreateCodeByPreferredTerm(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int existingCode = GetCodeIdByPreferredTerm(dbContext, codePreferredTerm);
            if (existingCode > 0)
                return existingCode;
            else
            {
                int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm);

                int codeId = dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC").FirstOrDefault() + 1;

                dbContext.Database.ExecuteSqlCommand($@"
                SET IDENTITY_INSERT dbo.Codes ON;
                INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, EntryDatetime, EntityStateCD) values ({codeId}, {thesaurusId}, {codeSetId}, GETDATE(), 2001);
                SET IDENTITY_INSERT dbo.Codes OFF;
            ");

                return codeId;
            }

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

        private int GetCodeIdByPreferredTerm(SReportsContext dbContext, string preferredTerm)
        {
            return dbContext.Database.SqlQuery<int>(
                $@"SELECT TOP(1) code.CodeId
                from [dbo].[Codes] code
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                WHERE PreferredTerm = '{preferredTerm}' AND code.EntityStateCD != 2003").FirstOrDefault();
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
