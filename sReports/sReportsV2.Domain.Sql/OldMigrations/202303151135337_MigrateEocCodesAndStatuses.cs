namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateEocCodesAndStatuses : DbMigration
    {
        public override void Up()
        {
            string updateEocTypes =
                @"update dbo.EpisodeOfCares
                    set TypeCD = code.[CodeId]
                    FROM [dbo].[EpisodeOfCares] eoc
                    inner join [dbo].[Codes] code on eoc.TypeCD = code.ThesaurusEntryId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    inner join [dbo].[CodeSets] cS on code.CodeSetId = cs.CodeSetId
                    inner join [dbo].[ThesaurusEntryTranslations] tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                    where tranThCodeSet.PreferredTerm = 'Episode of care type' and tranThCodeSet.Language = 'en'
				";

            string script1 =
                @"update dbo.EpisodeOfCares
                  set StatusCD=1752
                  where StatusCD=4
				";

            string script2 =
                @"update dbo.EpisodeOfCares
                    set StatusCD=1751
                    where StatusCD=3
				";

            string script3 =
                @"update dbo.EpisodeOfCares
                    set StatusCD=1748
                    where StatusCD=0
				";

            string script4 =
                @"update dbo.EpisodeOfCares
                    set StatusCD=1753
                    where StatusCD=5
				";

            string script5 =
                @"update dbo.EpisodeOfCares
                    set StatusCD=1750
                    where StatusCD=2
				";

            string script6 =
                @"update dbo.EpisodeOfCares
                    set StatusCD=1749
                    where StatusCD=1
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(updateEocTypes);
            sReportsContext.Database.ExecuteSqlCommand(script1);
            sReportsContext.Database.ExecuteSqlCommand(script2);
            sReportsContext.Database.ExecuteSqlCommand(script3);
            sReportsContext.Database.ExecuteSqlCommand(script4);
            sReportsContext.Database.ExecuteSqlCommand(script5);
            sReportsContext.Database.ExecuteSqlCommand(script6);

        }

        public override void Down()
        {
        }
    }
}
