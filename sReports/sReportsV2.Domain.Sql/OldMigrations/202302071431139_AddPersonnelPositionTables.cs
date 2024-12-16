namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelPositionTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelPositions",
                c => new
                    {
                        PersonnelPositionId = c.Int(nullable: false, identity: true),
                        PositionCD = c.Int(),
                        PersonnelId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.PersonnelPositionId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId)
                .ForeignKey("dbo.Codes", t => t.PositionCD)
                .Index(t => t.PositionCD)
                .Index(t => t.PersonnelId)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.PositionPermissions",
                c => new
                    {
                        PositionPermissionId = c.Int(nullable: false, identity: true),
                        PositionCD = c.Int(),
                        PermissionModuleId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.PositionPermissionId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.PermissionModules", t => t.PermissionModuleId)
                .ForeignKey("dbo.Codes", t => t.PositionCD)
                .Index(t => t.PositionCD)
                .Index(t => t.PermissionModuleId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PositionPermissions", "PositionCD", "dbo.Codes");
            DropForeignKey("dbo.PositionPermissions", "PermissionModuleId", "dbo.PermissionModules");
            DropForeignKey("dbo.PositionPermissions", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelPositions", "PositionCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelPositions", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelPositions", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.PositionPermissions", new[] { "CreatedById" });
            DropIndex("dbo.PositionPermissions", new[] { "PermissionModuleId" });
            DropIndex("dbo.PositionPermissions", new[] { "PositionCD" });
            DropIndex("dbo.PersonnelPositions", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelPositions", new[] { "PersonnelId" });
            DropIndex("dbo.PersonnelPositions", new[] { "PositionCD" });
            DropTable("dbo.PositionPermissions");
            DropTable("dbo.PersonnelPositions");
        }
    }
}
