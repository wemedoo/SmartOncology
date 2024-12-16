namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonnelTeam_Corrections : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CareTeams", "CareTeamTypeCD", "dbo.Codes");
            DropForeignKey("dbo.CareTeams", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.CareTeams", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.CareTeamUsers", "CareTeamId", "dbo.CareTeams");
            DropForeignKey("dbo.CareTeamUsers", "CareTeamRoleCD", "dbo.Codes");
            DropForeignKey("dbo.CareTeamUsers", "UserId", "dbo.Personnel");
            DropForeignKey("dbo.EpisodeOfCares", "CareTeamId", "dbo.CareTeams");
            DropIndex("dbo.CareTeamUsers", new[] { "CareTeamRoleCD" });
            DropIndex("dbo.CareTeamUsers", new[] { "CareTeamId" });
            DropIndex("dbo.CareTeamUsers", new[] { "UserId" });
            DropIndex("dbo.CareTeams", new[] { "OrganizationId" });
            DropIndex("dbo.CareTeams", new[] { "CareTeamTypeCD" });
            DropIndex("dbo.CareTeams", new[] { "CreatedById" });
            DropIndex("dbo.EpisodeOfCares", new[] { "CareTeamId" });
            CreateTable(
                "dbo.PersonnelTeamOrganizationRelations",
                c => new
                    {
                        PersonnelTeamOrganizationRelationId = c.Int(nullable: false, identity: true),
                        PersonnelTeamId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        RelationTypeCD = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.PersonnelTeamOrganizationRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.PersonnelTeams", t => t.PersonnelTeamId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.RelationTypeCD)
                .Index(t => t.PersonnelTeamId)
                .Index(t => t.OrganizationId)
                .Index(t => t.RelationTypeCD)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.PersonnelTeams",
                c => new
                    {
                        PersonnelTeamId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TypeCD = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.PersonnelTeamId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.TypeCD)
                .Index(t => t.TypeCD)
                .Index(t => t.CreatedById);

            CreateTable(
                "dbo.PersonnelTeamRelations",
                c => new
                {
                    PersonnelTeamRelationId = c.Int(nullable: false, identity: true),
                    RelationTypeCD = c.Int(),
                    PersonnelTeamId = c.Int(),
                    PersonnelId = c.Int(),
                    Active = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                })
                .PrimaryKey(t => t.PersonnelTeamRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId)
                .ForeignKey("dbo.PersonnelTeams", t => t.PersonnelTeamId)
                .ForeignKey("dbo.Codes", t => t.RelationTypeCD)
                .Index(t => t.RelationTypeCD)
                .Index(t => t.PersonnelTeamId)
                .Index(t => t.PersonnelId)
                .Index(t => t.CreatedById);
            
            AddColumn("dbo.EpisodeOfCares", "PersonnelTeamId", c => c.Int());
            CreateIndex("dbo.EpisodeOfCares", "PersonnelTeamId");
            AddForeignKey("dbo.EpisodeOfCares", "PersonnelTeamId", "dbo.PersonnelTeams", "PersonnelTeamId");
            DropColumn("dbo.EpisodeOfCares", "CareTeamId");
            DropTable("dbo.CareTeamUsers");
            DropTable("dbo.CareTeams");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CareTeams",
                c => new
                    {
                        CareTeamId = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Name = c.String(),
                        CareTeamTypeCD = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.CareTeamId);
            
            CreateTable(
                "dbo.CareTeamUsers",
                c => new
                    {
                        UserCareTeamId = c.Int(nullable: false, identity: true),
                        CareTeamRoleCD = c.Int(),
                        CareTeamId = c.Int(),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.UserCareTeamId);
            
            AddColumn("dbo.EpisodeOfCares", "CareTeamId", c => c.Int());
            DropForeignKey("dbo.EpisodeOfCares", "PersonnelTeamId", "dbo.PersonnelTeams");
            DropForeignKey("dbo.PersonnelTeamOrganizationRelations", "RelationTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelTeams", "TypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelTeamRelations", "RelationTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelTeamRelations", "PersonnelTeamId", "dbo.PersonnelTeams");
            DropForeignKey("dbo.PersonnelTeamRelations", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelTeamRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelTeamOrganizationRelations", "PersonnelTeamId", "dbo.PersonnelTeams");
            DropForeignKey("dbo.PersonnelTeams", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelTeamOrganizationRelations", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.PersonnelTeamOrganizationRelations", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.EpisodeOfCares", new[] { "PersonnelTeamId" });
            DropIndex("dbo.PersonnelTeamRelations", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelTeamRelations", new[] { "PersonnelId" });
            DropIndex("dbo.PersonnelTeamRelations", new[] { "PersonnelTeamId" });
            DropIndex("dbo.PersonnelTeamRelations", new[] { "RelationTypeCD" });
            DropIndex("dbo.PersonnelTeams", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelTeams", new[] { "TypeCD" });
            DropIndex("dbo.PersonnelTeamOrganizationRelations", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelTeamOrganizationRelations", new[] { "RelationTypeCD" });
            DropIndex("dbo.PersonnelTeamOrganizationRelations", new[] { "OrganizationId" });
            DropIndex("dbo.PersonnelTeamOrganizationRelations", new[] { "PersonnelTeamId" });
            DropColumn("dbo.EpisodeOfCares", "PersonnelTeamId");
            DropTable("dbo.PersonnelTeamRelations");
            DropTable("dbo.PersonnelTeams");
            DropTable("dbo.PersonnelTeamOrganizationRelations");
            CreateIndex("dbo.EpisodeOfCares", "CareTeamId");
            CreateIndex("dbo.CareTeams", "CreatedById");
            CreateIndex("dbo.CareTeams", "CareTeamTypeCD");
            CreateIndex("dbo.CareTeams", "OrganizationId");
            CreateIndex("dbo.CareTeamUsers", "UserId");
            CreateIndex("dbo.CareTeamUsers", "CareTeamId");
            CreateIndex("dbo.CareTeamUsers", "CareTeamRoleCD");
            AddForeignKey("dbo.EpisodeOfCares", "CareTeamId", "dbo.CareTeams", "CareTeamId");
            AddForeignKey("dbo.CareTeamUsers", "UserId", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.CareTeamUsers", "CareTeamRoleCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.CareTeamUsers", "CareTeamId", "dbo.CareTeams", "CareTeamId");
            AddForeignKey("dbo.CareTeams", "OrganizationId", "dbo.Organizations", "OrganizationId", cascadeDelete: true);
            AddForeignKey("dbo.CareTeams", "CreatedById", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.CareTeams", "CareTeamTypeCD", "dbo.Codes", "CodeId");
        }
    }
}
