namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedPropertyStoredProcedure : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createFunction = $@"
                CREATE or ALTER FUNCTION NumOfExtendedProperties ( @tableName sysname, @columnName sysname = NULL) 
                RETURNS int
                AS
                BEGIN
	                declare @numOfExtendedProperties int;
	                declare @level2type varchar(128);
	                if @columnName is null
		                set @level2type = @columnName;
	                else
		                set @level2type = 'Column';
	                SELECT @numOfExtendedProperties = COUNT(name)
		                FROM ::fn_listextendedproperty ('Description','Schema', 'dbo', 'Table', @tableName, @level2type, @columnName);
	                return @numOfExtendedProperties;
                END
                ;
            ";
            string createStoredProcedure = $@"
                CREATE OR ALTER PROCEDURE AddExtendedProperty @description sql_variant, @tableName sysname, @columnName sysname = NULL
                AS
	                BEGIN		
		                declare @numOfExtendedProperties int = [dbo].[NumOfExtendedProperties] (@tableName, @columnName);
		                declare @extendedPropertyProcedureName varchar(128) = 'sys.sp_addextendedproperty';
		                IF @numOfExtendedProperties > 0
			                set @extendedPropertyProcedureName = 'sys.sp_updateextendedproperty';
			                declare @level2type sysname = 'Column';
		                if @columnName is null
			                set @level2type = @columnName;
		                exec @extendedPropertyProcedureName
			                @name = 'Description',   
			                @value = @description,   
			                @level0type = 'SCHEMA', @level0name = 'dbo',  
			                @level1type = 'TABLE',  @level1name = @tableName,
			                @level2type = @level2type,  @level2name = @columnName
		                ;   
	                END
            ";

            context.Database.ExecuteSqlCommand(createFunction);
            context.Database.ExecuteSqlCommand(createStoredProcedure);
        }
        
        public override void Down()
        {
        }
    }
}
