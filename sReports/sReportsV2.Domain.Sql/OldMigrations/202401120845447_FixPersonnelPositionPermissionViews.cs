namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPersonnelPositionPermissionViews : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE  or alter   view [dbo].[PersonnelPositionPermissionViews]
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
					  inner join dbo.Codes code on code.CodeId = posPermission.PositionCD
					  where GETDATE() BETWEEN perPosition.[ActiveFrom] AND perPosition.[ActiveTo]
					  and GETDATE() BETWEEN posPermission.[ActiveFrom] AND posPermission.[ActiveTo]
					  and GETDATE() BETWEEN code.[ActiveFrom] AND code.[ActiveTo]
				");
        }
        
        public override void Down()
        {
        }
    }
}
