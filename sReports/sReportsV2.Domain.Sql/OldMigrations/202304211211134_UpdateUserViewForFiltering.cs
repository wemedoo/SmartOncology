namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateUserViewForFiltering : DbMigration
    {
        public override void Up()
        {
            string userViewForFiltering =
                $@"CREATE  or alter VIEW [dbo].[UserViews]
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
				,personnel.[DayOfBirth]
				,personnel.[PersonnelTypeCD]
				,(SELECT cast(personnelPositionInner.PositionCD as varchar)
						FROM dbo.PersonnelPositions personnelPositionInner
						WHERE personnel.UserId = personnelPositionInner.PersonnelId
						group by personnelPositionInner.PositionCD
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as Roles
				,STUFF((SELECT ', ' + org.Name 
						FROM dbo.Organizations org
						INNER JOIN dbo.UserOrganizations userOrg 
						ON userOrg.OrganizationId = org.OrganizationId
						WHERE personnel.UserId = userOrg.UserId
						FOR XML PATH('')), 1, 1, '') as UserOrganizations
				,(SELECT cast(org.OrganizationId as varchar) 
						FROM dbo.Organizations org
						INNER JOIN dbo.UserOrganizations userOrg 
						ON userOrg.OrganizationId = org.OrganizationId
						WHERE personnel.UserId = userOrg.UserId
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as UserOrganizationIds
				,(SELECT cast(personnelIdentifier.IdentifierTypeCD as varchar) + '{Delimiters.ComplexSegmentDelimiter}' + personnelIdentifier.IdentifierValue
						FROM dbo.PersonnelIdentifiers personnelIdentifier
						WHERE personnel.UserId = personnelIdentifier.PersonnelId
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelIdentifiers
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
				,personnel.[CreatedById]
				,personnel.[DayOfBirth]
				,personnel.[PersonnelTypeCD]
				;
			"
            ;

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(userViewForFiltering);
        }

        public override void Down()
        {
        }
    }
}
