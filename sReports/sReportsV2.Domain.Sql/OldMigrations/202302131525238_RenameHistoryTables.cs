namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameHistoryTables : DbMigration
    {
        public override void Up()
        {
            string sql = $@"
                DECLARE @SQL nvarchar(MAX);
                SET @SQL = (SELECT
                string_agg(N'exec sp_rename N' + QUOTENAME(tables.TABLE_NAME,'''') + N', N' 
	                + QUOTENAME(replace(tables.TABLE_NAME, '_History', 'History'),'''') 
	                + N';', NCHAR(13))
                FROM
	                INFORMATION_SCHEMA.TABLES tables
	                WHERE tables.table_type = 'BASE TABLE'
	                and tables.TABLE_NAME like '%[_]History'
	                and tables.TABLE_NAME != '__MigrationHistory'
                  );
                EXEC sys.sp_executesql @SQL;
            ";
            Sql(sql);
        }
        
        public override void Down()
        {
            string sql = $@"
                DECLARE @SQL nvarchar(MAX);
                SET @SQL = (SELECT
                string_agg(N'exec sp_rename N' + QUOTENAME(tables.TABLE_NAME,'''') + N', N' 
	                + QUOTENAME(replace(tables.TABLE_NAME, 'History', '_History'),'''') 
	                + N';', NCHAR(13))
                FROM
	                INFORMATION_SCHEMA.TABLES tables
	                WHERE tables.table_type = 'BASE TABLE'
	                and tables.TABLE_NAME like '%History'
	                and tables.TABLE_NAME != '__MigrationHistory'
                  );
                EXEC sys.sp_executesql @SQL;
            ";
            Sql(sql);
        }

        
    }
}
