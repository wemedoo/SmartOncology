namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissingSqlCommandsOnInit : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Encounters", new[] { "ClassCD" });
            CreateIndex("dbo.Encounters", "ClassCD");
            DropForeignKey("dbo.Encounters", "ClassCD", "dbo.Codes");
            AddForeignKey("dbo.Encounters", "ClassCD", "dbo.Codes", "CodeId");
            DropIndex("dbo.Encounters", new[] { "TypeCD" });
            CreateIndex("dbo.Encounters", "TypeCD");
            DropForeignKey("dbo.Encounters", "TypeCD", "dbo.Codes");
            AddForeignKey("dbo.Encounters", "TypeCD", "dbo.Codes", "CodeId");

            // update views (3/4) --> Remove entity state conditions and apply getdate()
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
							order by org.Name
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

            Sql(@"CREATE OR ALTER VIEW dbo.CodeAliasViews
                AS 
                SELECT
                    inboundAliases.[AliasId],
                    inboundAliases.[CodeId],
                    inboundAliases.[System],
                    inboundAliases.[Alias] as InboundAlias,
                    outboundAliases.[Alias] as OutboundAlias,
                    inboundAliases.[EntityStateCD],
                    inboundAliases.[RowVersion],
                    inboundAliases.[EntryDatetime],
                    inboundAliases.[LastUpdate],
                    inboundAliases.[ActiveFrom],
                    inboundAliases.[ActiveTo],
                    inboundAliases.[CreatedById],
	                inboundAliases.[AliasId] as InboundAliasId,
                    outboundAliases.[AliasId] as OutboundAliasId
                FROM dbo.[InboundAliases] inboundAliases
                LEFT JOIN dbo.[OutboundAliases] outboundAliases
                    ON outboundAliases.[System] = inboundAliases.[System] 
                    AND outboundAliases.[CodeId] = inboundAliases.[CodeId]  AND outboundAliases.[AliasId] = inboundAliases.[OutboundAliasId]
                    AND GETDATE() between outboundAliases.[ActiveFrom] and outboundAliases.[ActiveTo]
                WHERE GETDATE() BETWEEN inboundAliases.[ActiveFrom] AND inboundAliases.[ActiveTo]
			;");

            Sql(@"CREATE or ALTER  VIEW [dbo].[EncounterViews]
                    AS
					select
					encounters.EncounterId
					,patients.NameGiven
					,patients.NameFamily
					,patients.GenderCD
					,patients.BirthDate
					,patients.PatientId
                    ,encounters.StatusCD
				    ,encounters.AdmitDatetime
				    ,encounters.DischargeDatetime
                    ,encounters.EpisodeOfCareId
                    ,encounters.EntityStateCD
                    ,encounters.[RowVersion]
					,encounters.[EntryDatetime]
					,encounters.[LastUpdate]
					,encounters.[ActiveFrom]
					,encounters.[ActiveTo]
					,encounters.[CreatedById]
					from dbo.Encounters encounters
					left join dbo.Patients patients
					on encounters.PatientId = patients.PatientId
                    where GETDATE() BETWEEN patients.[ActiveFrom] AND patients.[ActiveTo]
                    and GETDATE() BETWEEN encounters.[ActiveFrom] AND encounters.[ActiveTo]
                    ");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Encounters", "TypeCD", "dbo.Codes");
            DropIndex("dbo.Encounters", new[] { "TypeCD" });
            DropForeignKey("dbo.Encounters", "ClassCD", "dbo.Codes");
            DropIndex("dbo.Encounters", new[] { "ClassCD" });
        }
    }
}
