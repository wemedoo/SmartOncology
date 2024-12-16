namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixThesaurusTranslationNamingConvention : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ThesaurusEntryTranslations", name: "Id", newName: "ThesaurusEntryTranslationId");
        }

        public override void Down()
        {
            RenameColumn(table: "dbo.ThesaurusEntryTranslations", name: "ThesaurusEntryTranslationId", newName: "Id");
        }
    }
}
