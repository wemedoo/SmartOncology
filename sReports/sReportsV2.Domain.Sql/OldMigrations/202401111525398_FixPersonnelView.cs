namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPersonnelView : DbMigration
    {
        public override void Up()
        {
            Sql($@"CREATE or alter  VIEW [dbo].[PersonnelViews]
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
							and GETDATE() BETWEEN personnelPositionInner.[ActiveFrom] AND personnelPositionInner.[ActiveTo]
							group by personnelPositionInner.PositionCD
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelPositionIds
					,STUFF((SELECT ', ' + thTran.PreferredTerm
						FROM dbo.PersonnelPositions personnelPositionInner
						inner join dbo.Codes code on code.CodeId = personnelPositionInner.PositionCD
						inner join dbo.ThesaurusEntryTranslations thTran on thTran.ThesaurusEntryId = code.ThesaurusEntryId
						WHERE personnel.PersonnelId = personnelPositionInner.PersonnelId
						and GETDATE() BETWEEN personnelPositionInner.[ActiveFrom] AND personnelPositionInner.[ActiveTo]
						and thTran.Language = '{LanguageConstants.EN}'
						group by thTran.PreferredTerm 
						order by thTran.PreferredTerm
						FOR XML PATH('')), 1, 1, '') as PersonnelPositions
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.personnelId = personnelOrg.PersonnelId
							and personnelOrg.StateCD != {(int)UserState.Archived}
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {(int)EntityStateCode.Deleted}))	
							and GETDATE() BETWEEN org.[ActiveFrom] AND org.[ActiveTo] 
							order by org.Name
							FOR XML PATH('')), 1, 1, '') as PersonnelOrganizations
					,(SELECT cast(org.OrganizationId as varchar) 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.PersonnelId = personnelOrg.PersonnelId
							and personnelOrg.StateCD != {(int)UserState.Archived}
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {(int)EntityStateCode.Deleted}))
							and GETDATE() BETWEEN org.[ActiveFrom] AND org.[ActiveTo] 
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelOrganizationIds
					,(SELECT cast(personnelIdentifier.IdentifierTypeCD as varchar) + '{Delimiters.ComplexSegmentDelimiter}' + personnelIdentifier.IdentifierValue
							FROM dbo.PersonnelIdentifiers personnelIdentifier
							WHERE personnel.PersonnelId = personnelIdentifier.PersonnelId
							and GETDATE() BETWEEN personnelIdentifier.[ActiveFrom] AND personnelIdentifier.[ActiveTo]
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
							and GETDATE() BETWEEN personnelAddress.[ActiveFrom] AND personnelAddress.[ActiveTo]
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
					;");
        }
        
        public override void Down()
        {
        }
    }
}
