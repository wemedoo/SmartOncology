namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectIdToClinicalTrials : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalTrials", "ProjectId", c => c.Int());
            CreateIndex("dbo.ClinicalTrials", "ProjectId");
            AddForeignKey("dbo.ClinicalTrials", "ProjectId", "dbo.Projects", "ProjectId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalTrials", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ClinicalTrials", new[] { "ProjectId" });
            DropColumn("dbo.ClinicalTrials", "ProjectId");
        }
    }
}
