namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserViewWithRoles : DbMigration
    {
        public override void Up()
        {
			string script =
				@"CREATE or alter  VIEW [dbo].[UserViews]
					AS
					select
					personnel.[UserId]
					,personnel.[Username]
					,personnel.[FirstName]
					,personnel.[LastName]
					,personnel.[Email]
					,personnel.[IsDeleted]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,userOrg.[OrganizationId]
					,userOrg.[StateCD]
					,personnel.[Active]
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
					,personnel.[IsDeleted]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,userOrg.[OrganizationId]
					,userOrg.[StateCD]
					,personnel.[Active]
					,personnel.[CreatedById];
				";

			SReportsContext sReportsContext = new SReportsContext();
			sReportsContext.Database.ExecuteSqlCommand(script);
		}
        
        public override void Down()
        {
            
        }
    }
}
