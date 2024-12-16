namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(),
                        ProjectTypeCD = c.Int(),
                        ProjectStartDateTime = c.DateTime(),
                        ProjectEndDateTime = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Codes", t => t.ProjectTypeCD)
                .Index(t => t.ProjectTypeCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ProjectTypeCD", "dbo.Codes");
            DropForeignKey("dbo.Projects", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.Projects", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.Projects", new[] { "EntityStateCD" });
            DropIndex("dbo.Projects", new[] { "CreatedById" });
            DropIndex("dbo.Projects", new[] { "ProjectTypeCD" });
            DropTable("dbo.Projects");
        }
    }
}
