namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelPossitionPermissionViews : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createOrAlterView = @"
                create or alter view dbo.PersonnelPositionPermissionViews
                as SELECT
                      NEWID() AS Id
					  ,perPosition.PersonnelPositionId
	                  ,perPosition.PersonnelId
                      ,m.ModuleId
	                  ,m.Name ModuleName
                      ,p.PermissionId
	                  ,p.Name PermissionName
	                  ,posPermission.PositionCD
                  FROM dbo.PermissionModules pM
                  inner join dbo.Modules m on m.ModuleId = pM.ModuleId 
                  inner join dbo.Permissions p on p.PermissionId = pM.PermissionId
                  inner join dbo.PositionPermissions posPermission on posPermission.PermissionModuleId = pm.PermissionModuleId
                  inner join dbo.PersonnelPositions perPosition on perPosition.PositionCD = posPermission.PositionCD
                  where perPosition.IsDeleted = 0 and posPermission.IsDeleted = 0
                  ;
            ";
            context.Database.ExecuteSqlCommand(createOrAlterView);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropView = @"
                  drop view if exists dbo.PersonnelPositionPermissionViews
            ";
            context.Database.ExecuteSqlCommand(dropView);
        }
    }
}
