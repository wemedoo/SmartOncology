namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCountryCodes : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();

            string script= $@"
                    update Codes
                    set CodeSetId=40
                    where TypeCD=10";

            dbContext.Database.ExecuteSqlCommand(script);

        }
        
        public override void Down()
        {
        }
    }
}
