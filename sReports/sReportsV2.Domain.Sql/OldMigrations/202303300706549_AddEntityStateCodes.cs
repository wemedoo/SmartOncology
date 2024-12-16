namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class AddEntityStateCodes : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();
            bool hasEntities = dbContext.Modules.Any() || dbContext.Permissions.Any();
            if (hasEntities)
            {
                CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(dbContext, GetEntityStateCodes(), insertWithCustomSql: false);
                codeMigrationHelper.InsertCodes(CodeSetAttributeNames.EntityState, 2000);
            }
        }


        public override void Down()
        { }

        private List<ThesaurusEntry> GetEntityStateCodes()
        {
            return new List<ThesaurusEntry>()
            {
                new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = LanguageConstants.EN,
                            PreferredTerm = "Active",
                            Definition = "Active"
                        }
                    }
                },
                new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = LanguageConstants.EN,
                            PreferredTerm = "Merged",
                            Definition = "Merged"
                        }
                    }
                },
                new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = LanguageConstants.EN,
                            PreferredTerm = "Deleted",
                            Definition = "Deleted"
                        }
                    }
                }

            };
        }
    }
}
