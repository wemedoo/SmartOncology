namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDuplicateMartialStatus : DbMigration
    {
        public override void Up()
        {
            string deleteCodes =
                  @"delete FROM [Codes]
                    WHERE CodeSetId = (SELECT TOP(1) [CodeSetId] FROM [CodeSets]
                    WHERE ThesaurusEntryId = 10622 ORDER BY [CodeSetId] DESC);";

            string deleteCodeSet =
                  @"DELETE FROM [CodeSets]
                    WHERE CodeSetId = (
                        SELECT TOP(1) CodeSetId 
                        FROM [CodeSets] 
                        WHERE ThesaurusEntryId = 10622 
                        ORDER BY CodeSetId DESC
                    );";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(deleteCodes);
            sReportsContext.Database.ExecuteSqlCommand(deleteCodeSet);
        }
        
        public override void Down()
        {
        }
    }
}
