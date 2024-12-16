namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateReligionCDValues : DbMigration
    {
        public override void Up()
        {
            string script =
                  @"UPDATE p
                    SET p.ReligionCD = c.CodeId
                    FROM [dbo].[Patients] p
                    LEFT JOIN [dbo].[Codes] c
                    ON c.ThesaurusEntryId IN (
                        SELECT c.ThesaurusEntryId
                        FROM [dbo].[Patients] p
                        INNER JOIN [dbo].[Codes] c
                        ON p.ReligionCD = c.CodeId
                        WHERE p.ReligionCD IS NOT NULL
                    )
                    WHERE  p.ReligionCD IS NOT NULL and c.CodeSetId is not null
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(script);
        }
        
        public override void Down()
        {
        }
    }
}
