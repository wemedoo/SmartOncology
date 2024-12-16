namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateViewsWithEntityState : DbMigration
    {
		public override void Up()
		{
			string codeAliasView =
				@"CREATE OR ALTER VIEW dbo.CodeAliasViews
					as select
							inboundAliases.[AliasId]
							,inboundAliases.[CodeId]
							,inboundAliases.[System]
							,inboundAliases.[Alias] as InboundAlias
							,outboundAliases.[Alias] as OutboundAlias
							,inboundAliases.[ValidFrom]
							,inboundAliases.[ValidTo]
							,inboundAliases.[EntityStateCD]
							,inboundAliases.[RowVersion]
							,inboundAliases.[EntryDatetime]
							,inboundAliases.[LastUpdate]
							,inboundAliases.[ActiveFrom]
							,inboundAliases.[ActiveTo]
							,inboundAliases.[CreatedById]
					from dbo.[InboundAliases] inboundAliases
					inner join dbo.[OutboundAliases] outboundAliases
					on outboundAliases.[System] = inboundAliases.[System] and outboundAliases.[CodeId] = inboundAliases.[CodeId]
					where GETDATE() between inboundAliases.[ValidFrom] and inboundAliases.[ValidTo]
				";

			string userView =
				@"CREATE OR ALTER VIEW dbo.UserViews
					AS
					select
					personnel.[UserId]
					,personnel.[Username]
					,personnel.[FirstName]
					,personnel.[LastName]
					,personnel.[Email]
					,personnel.[EntityStateCD]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,userOrg.[OrganizationId]
					,userOrg.[StateCD]
					,personnel.[ActiveFrom]
					,personnel.[ActiveTo]
					,personnel.[CreatedById]
					,STUFF((SELECT ', ' + cast(personnelPositionInner.PositionCD as varchar)
							FROM dbo.PersonnelPositions personnelPositionInner
							WHERE personnel.UserId = personnelPositionInner.PersonnelId
							group by personnelPositionInner.PositionCD
							FOR XML PATH('')), 1, 1, '') as Roles
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.UserOrganizations userOrg 
							ON userOrg.OrganizationId = org.OrganizationId
							WHERE personnel.UserId = userOrg.UserId
							FOR XML PATH('')), 1, 1, '') as UserOrganizations
					from dbo.Personnel personnel
					left join dbo.[PersonnelPositions] personnelPosition
					on personnelPosition.PersonnelId = personnel.UserId
					left join dbo.[UserOrganizations] userOrg
					on userOrg.UserId = personnel.UserId
					left join dbo.[Organizations] org
					on userOrg.OrganizationId = org.OrganizationId
					group by personnel.[UserId]
					,personnel.[Username]
					,personnel.[FirstName]
					,personnel.[LastName]
					,personnel.[Email]
					,personnel.[EntityStateCD]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,userOrg.[OrganizationId]
					,userOrg.[StateCD]
					,personnel.[ActiveFrom]
					,personnel.[ActiveTo]
					,personnel.[CreatedById];
				";

			string personnelPositionPermissionViews =
				@"CREATE OR ALTER view [dbo].[PersonnelPositionPermissionViews]
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
				";

			SReportsContext sReportsContext = new SReportsContext();
			sReportsContext.Database.ExecuteSqlCommand(codeAliasView);
			sReportsContext.Database.ExecuteSqlCommand(userView);
			sReportsContext.Database.ExecuteSqlCommand(personnelPositionPermissionViews);
		}

		public override void Down()
		{
		}
	}
}
