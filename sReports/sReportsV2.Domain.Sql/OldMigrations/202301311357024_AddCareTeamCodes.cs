namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCareTeamCodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CareTeamUsers", "CareTeamRoleCD", c => c.Int());
            AddColumn("dbo.CareTeams", "CareTeamTypeCD", c => c.Int());
            CreateIndex("dbo.CareTeamUsers", "CareTeamRoleCD");
            CreateIndex("dbo.CareTeams", "CareTeamTypeCD");
            AddForeignKey("dbo.CareTeams", "CareTeamTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.CareTeamUsers", "CareTeamRoleCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.CareTeamUsers", "CareTeamUserRole");
            DropColumn("dbo.CareTeams", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CareTeams", "Type", c => c.String());
            AddColumn("dbo.CareTeamUsers", "CareTeamUserRole", c => c.String());
            DropForeignKey("dbo.CareTeamUsers", "CareTeamRoleCD", "dbo.Codes");
            DropForeignKey("dbo.CareTeams", "CareTeamTypeCD", "dbo.Codes");
            DropIndex("dbo.CareTeams", new[] { "CareTeamTypeCD" });
            DropIndex("dbo.CareTeamUsers", new[] { "CareTeamRoleCD" });
            DropColumn("dbo.CareTeams", "CareTeamTypeCD");
            DropColumn("dbo.CareTeamUsers", "CareTeamRoleCD");
        }
    }
}
