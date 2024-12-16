namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRolesTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserRoles", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.Roles", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PermissionRoles", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.PermissionRoles", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.PermissionRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Personnel");
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "CreatedById" });
            DropIndex("dbo.Roles", new[] { "CreatedById" });
            DropIndex("dbo.PermissionRoles", new[] { "ModuleId" });
            DropIndex("dbo.PermissionRoles", new[] { "PermissionId" });
            DropIndex("dbo.PermissionRoles", new[] { "RoleId" });
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.PermissionRoles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PermissionRoles",
                c => new
                    {
                        PermissionRoleId = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PermissionRoleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.UserRoleId);
            
            CreateIndex("dbo.PermissionRoles", "RoleId");
            CreateIndex("dbo.PermissionRoles", "PermissionId");
            CreateIndex("dbo.PermissionRoles", "ModuleId");
            CreateIndex("dbo.Roles", "CreatedById");
            CreateIndex("dbo.UserRoles", "CreatedById");
            CreateIndex("dbo.UserRoles", "RoleId");
            CreateIndex("dbo.UserRoles", "UserId");
            AddForeignKey("dbo.UserRoles", "UserId", "dbo.Personnel", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles", "RoleId", cascadeDelete: true);
            AddForeignKey("dbo.PermissionRoles", "RoleId", "dbo.Roles", "RoleId", cascadeDelete: true);
            AddForeignKey("dbo.PermissionRoles", "PermissionId", "dbo.Permissions", "PermissionId", cascadeDelete: true);
            AddForeignKey("dbo.PermissionRoles", "ModuleId", "dbo.Modules", "ModuleId", cascadeDelete: true);
            AddForeignKey("dbo.Roles", "CreatedById", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.UserRoles", "CreatedById", "dbo.Personnel", "UserId");
        }
    }
}
