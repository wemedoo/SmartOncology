namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKsIntoEpisodeOfCare : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();

            string updateCmd = $@"Update [dbo].[EpisodeOfCares]
                                  set TypeCD=1424
                                  where TypeCD=0";

            dbContext.Database.ExecuteSqlCommand(updateCmd);

            CreateIndex("dbo.EpisodeOfCares", "StatusCD");
            CreateIndex("dbo.EpisodeOfCares", "TypeCD");
            AddForeignKey("dbo.EpisodeOfCares", "StatusCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.EpisodeOfCares", "TypeCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EpisodeOfCares", "TypeCD", "dbo.Codes");
            DropForeignKey("dbo.EpisodeOfCares", "StatusCD", "dbo.Codes");
            DropIndex("dbo.EpisodeOfCares", new[] { "TypeCD" });
            DropIndex("dbo.EpisodeOfCares", new[] { "StatusCD" });
        }
    }
}
