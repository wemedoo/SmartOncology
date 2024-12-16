namespace sReportsV2.Domain.Sql.Migrations
{
	using sReportsV2.Common.Constants;
	using sReportsV2.Common.Enums;
	using sReportsV2.DAL.Sql.Sql;
	using System;
	using System.Data.Entity.Migrations;

	public partial class ChangeViewsToUseDateTimeOffset : DbMigration
	{
		public override void Up()
		{
			using (var context = new SReportsContext())
			{
				string updatePersonnelView = $@"
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
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelPositionIds
					,STUFF((SELECT ', ' + thTran.PreferredTerm
						FROM dbo.PersonnelPositions personnelPositionInner
						inner join dbo.Codes code on code.CodeId = personnelPositionInner.PositionCD
						inner join dbo.ThesaurusEntryTranslations thTran on thTran.ThesaurusEntryId = code.ThesaurusEntryId
						WHERE personnel.PersonnelId = personnelPositionInner.PersonnelId
						and personnelPositionInner.EntityStateCD != {(int)EntityStateCode.Deleted}
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
					;";

				string updateEncounterView = $@"
                CREATE or ALTER  VIEW [dbo].[EncounterViews]
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
					where patients.EntityStateCD != ${(int)EntityStateCode.Deleted};
            ";

				context.Database.ExecuteSqlCommand(updateEncounterView);
				context.Database.ExecuteSqlCommand(updatePersonnelView);
			}
		}

		public override void Down()
		{
		}
	}
}
