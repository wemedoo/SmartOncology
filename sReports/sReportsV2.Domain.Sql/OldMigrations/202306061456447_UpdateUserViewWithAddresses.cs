namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserViewWithAddresses : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updatePersonnelViewWithAddresses = $@"
				CREATE or alter  VIEW [dbo].[PersonnelViews]
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
							and personnelPositionInner.EntityStateCD != {(int)EntityStateCode.Deleted}
							group by personnelPositionInner.PositionCD
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as Roles
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.personnelId = personnelOrg.PersonnelId
							and personnelOrg.StateCD != {(int)UserState.Archived}
							FOR XML PATH('')), 1, 1, '') as PersonnelOrganizations
					,(SELECT cast(org.OrganizationId as varchar) 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.PersonnelId = personnelOrg.PersonnelId
							and personnelOrg.StateCD != {(int)UserState.Archived}
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelOrganizationIds
					,(SELECT cast(personnelIdentifier.IdentifierTypeCD as varchar) + '{Delimiters.ComplexSegmentDelimiter}' + personnelIdentifier.IdentifierValue
							FROM dbo.PersonnelIdentifiers personnelIdentifier
							WHERE personnel.PersonnelId = personnelIdentifier.PersonnelId
							and personnelIdentifier.EntityStateCD != {(int)EntityStateCode.Deleted}
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelIdentifiers
					, (SELECT 
							(case when personnelAddress.CountryCD is null then '' else cast(personnelAddress.CountryCD as varchar) end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.City is null then '' else personnelAddress.City end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.Street is null then '' else personnelAddress.Street end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.PostalCode is null then '' else personnelAddress.PostalCode end)
							FROM dbo.PersonnelAddresses personnelAddress
							WHERE personnel.PersonnelId = personnelAddress.PersonnelId
							and personnelAddress.EntityStateCD != {(int)EntityStateCode.Deleted}
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelAddresses
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
            ";
            context.Database.ExecuteSqlCommand(updatePersonnelViewWithAddresses);
        }

        public override void Down()
        {
        }
    }
}
