namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveWronglyAddedCodeSets : DbMigration
    {
        public override void Up()
        {
            string deleteCodes =
                @"delete FROM [Codes]
                    WHERE CodeSetId = (SELECT TOP(1) [CodeSetId] FROM [CodeSets]
                    WHERE ThesaurusEntryId = 10621 ORDER BY [CodeSetId] DESC);";

            string deleteCodeSet =
                  @"DELETE FROM [CodeSets]
                    WHERE CodeSetId = (
                        SELECT TOP(1) CodeSetId 
                        FROM [CodeSets] 
                        WHERE ThesaurusEntryId = 10621 
                        ORDER BY CodeSetId DESC
                    );";

            string removeReligionCodes =
                @"delete FROM [Codes]
                    WHERE CodeSetId = 46";

            string removeReligionCodeSet =
                  @"DELETE FROM [CodeSets]
                    WHERE CodeSetId = 46;";


            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(deleteCodes);
            sReportsContext.Database.ExecuteSqlCommand(deleteCodeSet);
            sReportsContext.Database.ExecuteSqlCommand(removeReligionCodes);
            sReportsContext.Database.ExecuteSqlCommand(removeReligionCodeSet);
        }
        
        public override void Down()
        {
        }
    }
}
