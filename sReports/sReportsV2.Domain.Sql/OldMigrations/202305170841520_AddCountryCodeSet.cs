namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class AddCountryCodeSet : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext dbContext = new SReportsContext())
            {
                CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(dbContext);

                if(dbContext.Thesauruses.Any())
                {
                    int thesaurusId = codeMigrationHelper.InsertThesaurusForCodeSet(CodeSetAttributeNames.Country);

                    string countryCodeSet = $@"
                    insert into CodeSets (CodeSetId, ThesaurusEntryId, EntityStateCD, ActiveFrom, ActiveTo, EntryDatetime) values ({40}, {thesaurusId}, null, GETDATE(), CONVERT(datetime, '9999-12-31 23:59:59.000'), GETDATE());
                ";

                    dbContext.Database.ExecuteSqlCommand(countryCodeSet);
                }
            }
        }
        
        public override void Down()
        {
        }
    }
}
