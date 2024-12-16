namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAfterRenameUserView : DbMigration
    {
        public override void Up()
        {
            string updateUserViewAfterRename =
                $@"CREATE or alter VIEW [dbo].[PersonnelViews]
				AS
				select
				personnel.[PersonnelId]
				,personnel.[Username]
				,personnel.[FirstName]
				,personnel.[LastName]
				,personnel.[Email]
				,personnel.[EntityStateCD]
				,personnel.[RowVersion]
				,personnel.[EntryDatetime]
				,personnel.[LastUpdate]
				,personnelOrg.[OrganizationId]
				,personnelOrg.[StateCD]
				,personnel.[ActiveFrom]
				,personnel.[ActiveTo]
				,personnel.[CreatedById]
				,personnel.[DayOfBirth]
				,personnel.[PersonnelTypeCD]
				,(SELECT cast(personnelPositionInner.PositionCD as varchar)
						FROM dbo.PersonnelPositions personnelPositionInner
						WHERE personnel.PersonnelId = personnelPositionInner.PersonnelId
						group by personnelPositionInner.PositionCD
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as Roles
				,STUFF((SELECT ', ' + org.Name 
						FROM dbo.Organizations org
						INNER JOIN dbo.PersonnelOrganizations personnelOrg 
						ON personnelOrg.OrganizationId = org.OrganizationId
						WHERE personnel.personnelId = personnelOrg.PersonnelId
						FOR XML PATH('')), 1, 1, '') as PersonnelOrganizations
				,(SELECT cast(org.OrganizationId as varchar) 
						FROM dbo.Organizations org
						INNER JOIN dbo.PersonnelOrganizations personnelOrg 
						ON personnelOrg.OrganizationId = org.OrganizationId
						WHERE personnel.PersonnelId = personnelOrg.PersonnelId
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelOrganizationIds
				,(SELECT cast(personnelIdentifier.IdentifierTypeCD as varchar) + '{Delimiters.ComplexSegmentDelimiter}' + personnelIdentifier.IdentifierValue
						FROM dbo.PersonnelIdentifiers personnelIdentifier
						WHERE personnel.PersonnelId = personnelIdentifier.PersonnelId
						FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelIdentifiers
				from dbo.Personnel personnel
				left join dbo.[PersonnelPositions] personnelPosition
				on personnelPosition.PersonnelId = personnel.PersonnelId
				left join dbo.[PersonnelOrganizations] personnelOrg
				on personnelOrg.PersonnelId = personnel.PersonnelId
				left join dbo.[Organizations] org
				on personnelOrg.OrganizationId = org.OrganizationId
				group by personnel.[PersonnelId]
				,personnel.[Username]
				,personnel.[FirstName]
				,personnel.[LastName]
				,personnel.[Email]
				,personnel.[EntityStateCD]
				,personnel.[RowVersion]
				,personnel.[EntryDatetime]
				,personnel.[LastUpdate]
				,personnelOrg.[OrganizationId]
				,personnelOrg.[StateCD]
				,personnel.[ActiveFrom]
				,personnel.[ActiveTo]
				,personnel.[CreatedById]
				,personnel.[DayOfBirth]
				,personnel.[PersonnelTypeCD]
				;
			"
            ;

            string dropOldUseriview = "drop view if exists dbo.UserViews;";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(updateUserViewAfterRename);
            sReportsContext.Database.ExecuteSqlCommand(dropOldUseriview);
        }

        public override void Down()
        {
        }
    }
}
