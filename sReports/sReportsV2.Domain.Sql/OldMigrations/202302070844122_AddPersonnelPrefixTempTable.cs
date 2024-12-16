namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPersonnelPrefixTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createTempTable = $@"
                create table dbo.PersonnelPrefixTempTable (
	                UserId int, 
	                Prefix nvarchar(max)
                );
            ";

            string saveDataInTempTable = $@"
                insert into dbo.PersonnelPrefixTempTable (UserId, Prefix) 
	                select UserId
	                  ,case PrefixCD
                        when 1 then 'Ms'
                        else 'Mr'
                    end 
                from dbo.Personnel;
            ";

            context.Database.ExecuteSqlCommand(createTempTable);
            context.Database.ExecuteSqlCommand(saveDataInTempTable);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropTempTable = $@"
                drop table if exists dbo.PersonnelPrefixTempTable;
            ";
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
    }
}
