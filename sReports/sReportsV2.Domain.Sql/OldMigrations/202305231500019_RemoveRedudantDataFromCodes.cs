namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRedudantDataFromCodes : DbMigration
    {
        public override void Up()
        {
            try
            {
                string script = @"delete FROM [dbo].[Codes] where TypeCD !=0 and CodeSetId is null";

                SReportsContext sReportsContext = new SReportsContext();
                sReportsContext.Database.ExecuteSqlCommand(script);
            }
            catch (Exception) { }
        }
        
        public override void Down()
        {
        }
    }
}
