namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEpisodeOfCareFK : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Encounters", "EpisodeOfCareId");
            AddForeignKey("dbo.Encounters", "EpisodeOfCareId", "dbo.EpisodeOfCares", "EpisodeOfCareId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Encounters", "EpisodeOfCareId", "dbo.EpisodeOfCares");
            DropIndex("dbo.Encounters", new[] { "EpisodeOfCareId" });
        }
    }
}
