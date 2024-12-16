namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSimilarTermsTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SimilarTerms");
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string command =
                @"CREATE TABLE [dbo].[SimilarTerms] (
                    [SimilarTermId] [int] NOT NULL IDENTITY,
                    [Name] [nvarchar](max),
                    [Definition] [nvarchar](max),
                    [SourceCD] [int] NOT NULL,
                    [EntryDateTime] [datetime],
                    [ThesaurusEntryTranslationId] [int] NOT NULL,
                    CONSTRAINT [PK_dbo.SimilarTerms] PRIMARY KEY ([SimilarTermId])
                    )
                    CREATE INDEX [IX_ThesaurusEntryTranslationId] ON [dbo].[SimilarTerms]([ThesaurusEntryTranslationId])
                    ALTER TABLE [dbo].[SimilarTerms] ADD CONSTRAINT [FK_dbo.SimilarTerms_dbo.ThesaurusEntryTranslations_ThesaurusEntryTranslationId] FOREIGN KEY ([ThesaurusEntryTranslationId]) REFERENCES [dbo].[ThesaurusEntryTranslations] ([ThesaurusEntryTranslationId])";
            context.Database.ExecuteSqlCommand(command);
        }
    }
}
