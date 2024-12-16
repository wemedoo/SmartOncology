namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryCommunicationSystemCodeSet : DbMigration
    {
        public override void Up()
        {
			SReportsContext context = new SReportsContext();
			string script = $@"delete FROM [dbo].[CodeSets] where CodeSetId=80";
			context.Database.ExecuteSqlCommand(script);
		}
        
        public override void Down()
        {
        }
    }
}
