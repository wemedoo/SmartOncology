namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncDeletedStateWhereIncosistent : DbMigration
    {
        public override void Up()
        {
            Sql(@"DECLARE @sqlText VARCHAR(MAX)
                SET @sqlText = ''

                SELECT @sqlText = @sqlText 
                +
                ' update ' + QUOTENAME(columns.TABLE_NAME) + ' set ActiveTo = EntryDatetime'
                + ' where EntityStateCD = 2003 and ActiveTo = ''9999-12-31 23:59:59.9999999 +00:00'' ;' 
                + CHAR(13)
                FROM
	                INFORMATION_SCHEMA.COLUMNS columns
	                inner join INFORMATION_SCHEMA.TABLES tables on columns.TABLE_NAME = tables.TABLE_NAME
	                WHERE tables.table_type = 'BASE TABLE'
	                and tables.TABLE_NAME not like '%History'
	                and tables.TABLE_NAME != '__MigrationHistory'
	                and columns.COLUMN_NAME = 'EntityStateCD'
                  order by columns.TABLE_NAME
                  ;

                EXEC(@sqlText);"
            );
        }
        
        public override void Down()
        {
        }
    }
}
