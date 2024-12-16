namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class ReplaceLEgacyCodes : DbMigration
    {
        private sealed class CodesReferencesTempObject
        {
            public string ReferencingTable { get; set; }
            public string ReferencingColumn { get; set; }
        }

        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();
            DeleteTempTables(dbContext);
            PopulateCodesReferencesTempTable(dbContext);

            PopulateCodesTempTable(dbContext);
            AlterCodesTempTable(dbContext);

            UpdateReferencingColumns(dbContext);

            DeleteTempTables(dbContext);
        }

        public override void Down()
        {
        }

        private int PopulateCodesReferencesTempTable(SReportsContext dbContext)
        {
            //Create CodesReferencesTemp Table with all the columns from other tables referencing the column 'CodeId' from the 'Codes' table
            string command = $@"
                SELECT 
                    OBJECT_NAME(f.parent_object_id) AS ReferencingTable,
                    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ReferencingColumn
	                INTO CodesReferencesTemp
                FROM 
                    sys.foreign_keys AS f
                INNER JOIN 
                    sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id
                INNER JOIN 
                    sys.objects AS o ON o.object_id = fc.referenced_object_id
                WHERE 
                    o.name = 'Codes'
                    AND COL_NAME(fc.referenced_object_id, fc.referenced_column_id) = 'CodeId';";

            return dbContext.Database.ExecuteSqlCommand(command);
        }

        private int PopulateCodesTempTable(SReportsContext dbContext)
        {
            //Create CodesTemp with all records from the "Codes" table where "PreferredTerm" has a duplicate value
            string command = $@"
                SELECT PreferredTerm, CodeId, CodeSetId INTO dbo.CodesTemp
                From dbo.Codes cd
                inner join dbo.ThesaurusEntryTranslations tet on tet.ThesaurusEntryId = cd.ThesaurusEntryId
                WHERE PreferredTerm IN (
                    SELECT tet2.PreferredTerm
                    From dbo.Codes cd2
	                inner join dbo.ThesaurusEntryTranslations tet2 on tet2.ThesaurusEntryId = cd2.ThesaurusEntryId
                    GROUP BY tet2.PreferredTerm
                    HAVING COUNT(*) > 1)";

            return dbContext.Database.ExecuteSqlCommand(command);
        }

        private void AlterCodesTempTable(SReportsContext dbContext)
        {
            // add a new column named 'newCodeId' to the 'CodesTemp' table and populate it with the CodeId that has the PreferredTerm and has a non-null 'CodeSetId' value
            string command1 = $@"
                ALTER TABLE dbo.CodesTemp
                ADD newCodeId INT;";

            string command2 = $@"
                UPDATE c
                SET newCodeId = sub.CodeId
                FROM dbo.CodesTemp c
                JOIN (
                    SELECT PreferredTerm, MIN(CodeId) AS CodeId
                    FROM dbo.CodesTemp
                    WHERE CodeSetId IS NOT NULL
                    GROUP BY PreferredTerm
                ) sub ON c.PreferredTerm = sub.PreferredTerm;";

            // Remove "Healthy" codes
            string command3 = $@"
                DELETE FROM dbo.CodesTemp WHERE CodeSetId IS NOT NULL;";

            dbContext.Database.ExecuteSqlCommand(command1);
            dbContext.Database.ExecuteSqlCommand(command2);
            dbContext.Database.ExecuteSqlCommand(command3);
        }

        private void UpdateReferencingColumns(SReportsContext dbContext)
        {
            int codesReferencesTempRowCount = dbContext.Database.SqlQuery<int>($@"SELECT COUNT(*) FROM dbo.CodesReferencesTemp").FirstOrDefault();

            for (int i = 1; i <= codesReferencesTempRowCount; i++)
            {
                CodesReferencesTempObject row =
                    dbContext.Database.SqlQuery<CodesReferencesTempObject>($@"
                        SELECT *
                        FROM dbo.CodesReferencesTemp
                        ORDER BY (SELECT NULL)
                        OFFSET ({i} - 1) ROWS
                        FETCH NEXT 1 ROWS ONLY;").FirstOrDefault();

                dbContext.Database.ExecuteSqlCommand($@"
                    UPDATE dbo.{row.ReferencingTable}
                    SET {row.ReferencingColumn} = NewCodeId
                    FROM  dbo.{row.ReferencingTable} ref
                    inner join dbo.CodesTemp temp on temp.CodeId = ref.{row.ReferencingColumn}");
            }
        }

        private void DeleteTempTables(SReportsContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand($@"DROP TABLE if exists dbo.CodesTemp");
            dbContext.Database.ExecuteSqlCommand($@"DROP TABLE if exists dbo.CodesReferencesTemp");
        }
    }
}
