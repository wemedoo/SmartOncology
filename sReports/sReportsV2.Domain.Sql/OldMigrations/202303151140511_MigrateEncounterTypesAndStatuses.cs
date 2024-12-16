namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateEncounterTypesAndStatuses : DbMigration
    {
        public override void Up()
        {
            string updateEncounterTypes =
                @"update dbo.Encounters
                    set TypeCD = code.[CodeId]
                    FROM [dbo].Encounters enc
                    inner join [dbo].[Codes] code on enc.TypeCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    inner join [dbo].[CodeSets] cS on code.CodeSetId = cs.CodeSetId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                    where tranThCodeSet.PreferredTerm = 'Encounter type' and tranThCodeSet.Language = 'en';
				";

            string updateEncounterStatuses =
                @"update dbo.Encounters
                    set StatusCD = code.[CodeId]
                    FROM [dbo].Encounters enc
                    inner join [dbo].[Codes] code on enc.StatusCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    inner join [dbo].[CodeSets] cS on code.CodeSetId = cs.CodeSetId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                    where tranThCodeSet.PreferredTerm = 'Encounter status' and tranThCodeSet.Language = 'en';
				";

            string updateEncounterClasses =
                @"update dbo.Encounters
                    set ClassCD = code.[CodeId]
                    FROM [dbo].Encounters enc
                    inner join [dbo].[Codes] code on enc.ClassCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    inner join [dbo].[CodeSets] cS on code.CodeSetId = cs.CodeSetId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                    where tranThCodeSet.PreferredTerm = 'Encounter classification' and tranThCodeSet.Language = 'en';
				";
            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(updateEncounterTypes);
            sReportsContext.Database.ExecuteSqlCommand(updateEncounterStatuses);
            sReportsContext.Database.ExecuteSqlCommand(updateEncounterClasses);
        }
        
        public override void Down()
        {
        }
    }
}
