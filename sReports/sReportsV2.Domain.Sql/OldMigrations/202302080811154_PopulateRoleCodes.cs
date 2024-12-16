namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.Common;
    using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class PopulateRoleCodes : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();
            //bool hasEntities = dbContext.Modules.Any() || dbContext.Permissions.Any();
            //if (hasEntities)
            //{
            //    CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(dbContext, GetRoleCodes(), insertWithCustomSql: false);
            //    codeMigrationHelper.InsertCodes(CodeSetAttributeNames.Role);
            //}

            bool hasEntities = dbContext.Modules.Any() || dbContext.Permissions.Any();
            if (hasEntities) 
            {
                int codeSetId = dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeSetId FROM CodeSets ORDER BY CodeSetId DESC").FirstOrDefault() + 1;
                CreateCodeSet(dbContext, CodeSetAttributeNames.Role, codeSetId);
                    
                // Insert Codes
                CreateCode(dbContext, codeSetId, PredifinedRole.SuperAdministrator.ToString(), "A super administrator is a person who has full access to user and organizational modules, and other non-patient-related resources of the system. Super administrator can create users, add organizations and change general system properties.");
                CreateCode(dbContext, codeSetId, PredifinedRole.Administrator.ToString(), "A administrator is a person who has can manage users within an organization he belongs to and perform other non-patient related system tasks defined by the Super Administrator");
                CreateCode(dbContext, codeSetId, PredifinedRole.Doctor.ToString(), "A doctor is a person who has can edit documents of patients clinical trials within organization he is employed and do other activities defined by administrators.");
            }


        }

        public override void Down()
        {
            CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(new SReportsContext());
            codeMigrationHelper.RemoveCodes(CodeSetAttributeNames.Role);
        }

        private void CreateCodeSet(SReportsContext dbContext, string codeSetName, int codeSetId)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

            dbContext.Database.ExecuteSqlCommand($@"
                    insert into CodeSets (CodeSetId, ThesaurusEntryId, Active, IsDeleted, EntryDatetime) values ({codeSetId}, {thesaurusId}, 1, 0, GETDATE());
                ");
        }

        private void CreateCode(SReportsContext dbContext, int codeSetId, string codePreferredTerm, string codeDefinition = null)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm, codeDefinition);

            int codeId = dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC").FirstOrDefault() + 1;

            dbContext.Database.ExecuteSqlCommand($@"
                SET IDENTITY_INSERT dbo.Codes ON;
                INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, TypeCD, IsDeleted, EntryDatetime) values ({codeId}, {thesaurusId}, {codeSetId}, 0, 0, GETDATE());
                SET IDENTITY_INSERT dbo.Codes OFF;
            ");
        }

        private int GetOrCreateThesaurusId(SReportsContext dbContext, string preferredTerm, string definition=null)
        {
            int thesaurusId = dbContext.Database.SqlQuery<int>($@"SELECT ThesaurusEntryId FROM ThesaurusEntryTranslations WHERE PreferredTerm = '{preferredTerm}'").FirstOrDefault();
            if (thesaurusId <= 0)
            {
                thesaurusId = (int)dbContext.Database.SqlQuery<Decimal>($@"
                    INSERT INTO ThesaurusEntries (IsDeleted, EntryDatetime, Active) Values (0, GETDATE(), 1)
                    SELECT SCOPE_IDENTITY()
                    
                ").FirstOrDefault();

                dbContext.Database.ExecuteSqlCommand($@"INSERT INTO ThesaurusEntryTranslations (ThesaurusEntryId, Language, PreferredTerm, Definition) VALUES ({thesaurusId}, '{LanguageConstants.EN}', '{preferredTerm}', '{definition ?? preferredTerm}')");
            }
            return thesaurusId;
        }

        //private List<ThesaurusEntry> GetRoleCodes()
        //{
        //    return new List<ThesaurusEntry>()
        //    {
        //        new ThesaurusEntry()
        //        {
        //            Translations = new List<ThesaurusEntryTranslation>()
        //            {
        //                new ThesaurusEntryTranslation()
        //                {
        //                    Language = LanguageConstants.EN,
        //                    PreferredTerm = PredifinedRole.SuperAdministrator.ToString(),
        //                    Definition = "A super administrator is a person who has full access to user and organizational modules, and other non-patient-related resources of the system. Super administrator can create users, add organizations and change general system properties."
        //                }
        //            }
        //        },
        //        new ThesaurusEntry()
        //        {
        //            Translations = new List<ThesaurusEntryTranslation>()
        //            {
        //                new ThesaurusEntryTranslation()
        //                {
        //                    Language = LanguageConstants.EN,
        //                    PreferredTerm = PredifinedRole.Administrator.ToString(),
        //                    Definition = "A administrator is a person who has can manage users within an organization he belongs to and perform other non-patient related system tasks defined by the Super Administrator"
        //                }
        //            }
        //        },
        //        new ThesaurusEntry()
        //        {
        //            Translations = new List<ThesaurusEntryTranslation>()
        //            {
        //                new ThesaurusEntryTranslation()
        //                {
        //                    Language = LanguageConstants.EN,
        //                    PreferredTerm = PredifinedRole.Doctor.ToString(),
        //                    Definition = "A doctor is a person who has can edit documents of patient's clinical trials within organization he is employed and do other activities defined by administrators."
        //                }
        //            }
        //        }
        //    };
        //}
    }
}
