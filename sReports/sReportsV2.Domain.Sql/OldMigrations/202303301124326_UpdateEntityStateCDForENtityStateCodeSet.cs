namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntityStateCDForENtityStateCodeSet : DbMigration
    {
        public override void Up()
        {
			string script =
                @"update [CodeSets]
                  set [EntityStateCD]=2001
                  where [EntityStateCD] is null";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(script);
        }
        
        public override void Down()
        {
        }
    }
}
