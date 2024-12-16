namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetEntityStateCDAndOrganizationMissing : DbMigration
    {
        public override void Up()
        {
            /// Migration could break if EntityState code values are not set to expected values
            
            //Sql(@"
            //    DECLARE @sqlText VARCHAR(MAX)
            //    SET @sqlText = ''

            //    SELECT @sqlText = @sqlText 
            //    +
            //    ' update ' + QUOTENAME(columns.TABLE_NAME) + ' set EntityStateCD = 2001'
            //    + ' where EntityStateCD is null ;' 
            //    + CHAR(13)
            //    FROM
	           //     INFORMATION_SCHEMA.COLUMNS columns
	           //     inner join INFORMATION_SCHEMA.TABLES tables on columns.TABLE_NAME = tables.TABLE_NAME
	           //     WHERE tables.table_type = 'BASE TABLE'
	           //     and tables.TABLE_NAME not like '%History'
	           //     and tables.TABLE_NAME != '__MigrationHistory'
	           //     and columns.COLUMN_NAME = 'EntityStateCD'
            //      order by columns.TABLE_NAME
            //      ;

            //    EXEC(@sqlText);
            //");
            //Sql(@"update [dbo].[Patients] set OrganizationId = 1 where OrganizationId = 0;");
        }
        
        public override void Down()
        {
            
        }
    }
}
