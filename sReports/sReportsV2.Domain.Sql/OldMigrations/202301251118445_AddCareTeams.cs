namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCareTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CareTeamUsers",
                c => new
                    {
                        UserCareTeamId = c.Int(nullable: false, identity: true),
                        CareTeamUserRole = c.String(),
                        CareTeamId = c.Int(),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.UserCareTeamId)
                .ForeignKey("dbo.CareTeams", t => t.CareTeamId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.CareTeamId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CareTeams",
                c => new
                    {
                        CareTeamId = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Name = c.String(),
                        Type = c.String(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.CareTeamId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CareTeamUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.CareTeamUsers", "CareTeamId", "dbo.CareTeams");
            DropForeignKey("dbo.CareTeams", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.CareTeams", "CreatedById", "dbo.Users");
            DropIndex("dbo.CareTeams", new[] { "CreatedById" });
            DropIndex("dbo.CareTeams", new[] { "OrganizationId" });
            DropIndex("dbo.CareTeamUsers", new[] { "UserId" });
            DropIndex("dbo.CareTeamUsers", new[] { "CareTeamId" });
            DropTable("dbo.CareTeams");
            DropTable("dbo.CareTeamUsers");
        }
    }
}
