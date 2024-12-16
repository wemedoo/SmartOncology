namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserView : DbMigration
    {
		public override void Up()
		{
			string script =
				@"CREATE OR ALTER VIEW dbo.UserViews
                    AS
					select
					users.[UserId]
					,users.[Username]
					,users.[FirstName]
					,users.[LastName]
				    ,users.[Email]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
					,userOrg.[UserOrganizationId]
					,userOrg.[StateCD]
					,users.[Active]
					,users.[CreatedById]
					,STUFF((SELECT ', ' + roles.Name 
							FROM dbo.Roles roles
							INNER JOIN dbo.UserRoles userRoles 
							ON userRoles.UserRoleId = roles.RoleId
							WHERE users.UserId = userRoles.UserRoleId
							FOR XML PATH('')), 1, 1, '') as Roles
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.UserOrganizations userOrg 
							ON userOrg.UserOrganizationId = org.OrganizationId
							WHERE users.UserId = userOrg.UserOrganizationId
							FOR XML PATH('')), 1, 1, '') as UserOrganizations
					from dbo.Users users
					left join dbo.[UserRoles] userRoles
                    on userRoles.UserRoleId = users.UserId
					left join dbo.[Roles] roles
					on userRoles.UserRoleId = roles.RoleId
					left join dbo.[UserOrganizations] userOrg
                    on userOrg.UserOrganizationId = users.UserId
					left join dbo.[Organizations] org
					on userOrg.UserOrganizationId = org.OrganizationId
					group by users.[UserId]
					,users.[Username]
					,users.[FirstName]
					,users.[LastName]
				    ,users.[Email]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
					,userOrg.[UserOrganizationId]
					,userOrg.[StateCD]
					,users.[Active]
					,users.[CreatedById]
				";

			SReportsContext sReportsContext = new SReportsContext();
			sReportsContext.Database.ExecuteSqlCommand(script);
		}

		public override void Down()
        {
        }
    }
}
