namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCareTeamsToEoC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EpisodeOfCares", "CareTeamId", c => c.Int());
            CreateIndex("dbo.EpisodeOfCares", "CareTeamId");
            AddForeignKey("dbo.EpisodeOfCares", "CareTeamId", "dbo.CareTeams", "CareTeamId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.EpisodeOfCares", "CareTeamId", "dbo.CareTeams");
            DropIndex("dbo.EpisodeOfCares", new[] { "CareTeamId" });
            DropColumn("dbo.EpisodeOfCares", "CareTeamId");
        }
    }
}
