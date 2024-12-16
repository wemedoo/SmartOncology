using Microsoft.EntityFrameworkCore.Migrations;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnumColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            int versionTypeCodeSetId = (int)CodeSetList.VersionType;
            int thesaurusStateCodeSetId = (int)CodeSetList.ThesaurusState;
            int thesaurusMergeStateCodeSetId = (int)CodeSetList.ThesaurusMergeState;
            int userStateCodeSetId = (int)CodeSetList.UserState;
            int commentStateCodeSetId = (int)CodeSetList.CommentState;
            int globalUserSourceCodeSetId = (int)CodeSetList.GlobalUserSource;
            int globalUserStatusCodeSetId = (int)CodeSetList.GlobalUserStatus;
            int deletedStateCode = (int)EntityStateCode.Deleted;

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {versionTypeCodeSetId}
                 )
                 UPDATE c
                 SET c.TypeCD = 
                     CASE 
                         WHEN c.[Type] = 0 THEN CodesCTE.CodeId
                         WHEN c.[Type] = 1 THEN CodesCTE.CodeId
						 WHEN c.[Type] = 2 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[Versions] c
                 JOIN 
                     CodesCTE ON 
                     (c.[Type] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[Type] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[Type] = 2 AND CodesCTE.rn = 3);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {thesaurusStateCodeSetId}
                 )
                 UPDATE c
                 SET c.StateCD = 
                     CASE 
                         WHEN c.[State] IS NULL THEN CodesCTE.CodeId
						 WHEN c.[State] = 0 THEN CodesCTE.CodeId
                         WHEN c.[State] = 1 THEN CodesCTE.CodeId
						 WHEN c.[State] = 2 THEN CodesCTE.CodeId
						 WHEN c.[State] = 3 THEN CodesCTE.CodeId
                         WHEN c.[State] = 4 THEN CodesCTE.CodeId
						 WHEN c.[State] = 5 THEN CodesCTE.CodeId
						 WHEN c.[State] = 6 THEN CodesCTE.CodeId
                         WHEN c.[State] = 7 THEN CodesCTE.CodeId
						 WHEN c.[State] = 8 THEN CodesCTE.CodeId
						 WHEN c.[State] = 9 THEN CodesCTE.CodeId
                         WHEN c.[State] = 10 THEN CodesCTE.CodeId
						 WHEN c.[State] = 11 THEN CodesCTE.CodeId
						 WHEN c.[State] = 12 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[Versions] c
                 JOIN 
                     CodesCTE ON 
					 (c.[State] IS NULL AND CodesCTE.rn = 1) OR 
                     (c.[State] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[State] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[State] = 2 AND CodesCTE.rn = 3) OR 
					 (c.[State] = 3 AND CodesCTE.rn = 4) OR 
					 (c.[State] = 4 AND CodesCTE.rn = 5) OR 
					 (c.[State] = 5 AND CodesCTE.rn = 6) OR 
					 (c.[State] = 6 AND CodesCTE.rn = 7) OR 
					 (c.[State] = 7 AND CodesCTE.rn = 8) OR 
					 (c.[State] = 8 AND CodesCTE.rn = 9) OR 
					 (c.[State] = 9 AND CodesCTE.rn = 10) OR 
					 (c.[State] = 10 AND CodesCTE.rn = 11) OR 
					 (c.[State] = 11 AND CodesCTE.rn = 12) OR 
					 (c.[State] = 12 AND CodesCTE.rn = 13);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {thesaurusStateCodeSetId}
                 )
                 UPDATE c
                 SET c.StateCD = 
                     CASE 
                         WHEN c.[State] IS NULL THEN CodesCTE.CodeId
						 WHEN c.[State] = 0 THEN CodesCTE.CodeId
                         WHEN c.[State] = 1 THEN CodesCTE.CodeId
						 WHEN c.[State] = 2 THEN CodesCTE.CodeId
						 WHEN c.[State] = 3 THEN CodesCTE.CodeId
                         WHEN c.[State] = 4 THEN CodesCTE.CodeId
						 WHEN c.[State] = 5 THEN CodesCTE.CodeId
						 WHEN c.[State] = 6 THEN CodesCTE.CodeId
                         WHEN c.[State] = 7 THEN CodesCTE.CodeId
						 WHEN c.[State] = 8 THEN CodesCTE.CodeId
						 WHEN c.[State] = 9 THEN CodesCTE.CodeId
                         WHEN c.[State] = 10 THEN CodesCTE.CodeId
						 WHEN c.[State] = 11 THEN CodesCTE.CodeId
						 WHEN c.[State] = 12 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[ThesaurusEntries] c
                 JOIN 
                     CodesCTE ON 
					 (c.[State] IS NULL AND CodesCTE.rn = 1) OR 
                     (c.[State] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[State] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[State] = 2 AND CodesCTE.rn = 3) OR 
					 (c.[State] = 3 AND CodesCTE.rn = 4) OR 
					 (c.[State] = 4 AND CodesCTE.rn = 5) OR 
					 (c.[State] = 5 AND CodesCTE.rn = 6) OR 
					 (c.[State] = 6 AND CodesCTE.rn = 7) OR 
					 (c.[State] = 7 AND CodesCTE.rn = 8) OR 
					 (c.[State] = 8 AND CodesCTE.rn = 9) OR 
					 (c.[State] = 9 AND CodesCTE.rn = 10) OR 
					 (c.[State] = 10 AND CodesCTE.rn = 11) OR 
					 (c.[State] = 11 AND CodesCTE.rn = 12) OR 
					 (c.[State] = 12 AND CodesCTE.rn = 13);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {thesaurusMergeStateCodeSetId}
                 )
                 UPDATE c
                 SET c.StateCD = 
                     CASE 
                         WHEN c.[State] = 0 THEN CodesCTE.CodeId
                         WHEN c.[State] = 1 THEN CodesCTE.CodeId
						 WHEN c.[State] = 2 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[ThesaurusMerges] c
                 JOIN 
                     CodesCTE ON 
                     (c.[State] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[State] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[State] = 2 AND CodesCTE.rn = 3);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {userStateCodeSetId}
                 )
                 UPDATE c
                 SET c.[StateCD] = 
                     CASE 
                         WHEN c.[State] = 0 THEN CodesCTE.CodeId
                         WHEN c.[State] = 1 THEN CodesCTE.CodeId
						 WHEN c.[State] = 2 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[PersonnelOrganizations] c
                 JOIN 
                     CodesCTE ON 
                     (c.[State] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[State] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[State] = 2 AND CodesCTE.rn = 3);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {commentStateCodeSetId}
                 )
                 UPDATE c
                 SET c.CommentStateCD = 
                     CASE 
                         WHEN c.[CommentState] = 0 THEN CodesCTE.CodeId
                         WHEN c.[CommentState] = 1 THEN CodesCTE.CodeId
						 WHEN c.[CommentState] = 2 THEN CodesCTE.CodeId
                         WHEN c.[CommentState] = 3 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[Comments] c
                 JOIN 
                     CodesCTE ON 
                     (c.[CommentState] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[CommentState] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[CommentState] = 2 AND CodesCTE.rn = 3) OR 
					 (c.[CommentState] = 3 AND CodesCTE.rn = 4);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {globalUserSourceCodeSetId}
                 )
                 UPDATE c
                 SET c.[SourceCD] = 
                     CASE 
                         WHEN c.[Source] = 0 THEN CodesCTE.CodeId
                         WHEN c.[Source] = 1 THEN CodesCTE.CodeId
						 WHEN c.[Source] = 2 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[GlobalThesaurusUsers] c
                 JOIN 
                     CodesCTE ON 
                     (c.[Source] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[Source] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[Source] = 2 AND CodesCTE.rn = 3);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {globalUserStatusCodeSetId}
                 )
                 UPDATE c
                 SET c.[StatusCD] = 
                     CASE 
                         WHEN c.[Status] = 0 THEN CodesCTE.CodeId
                         WHEN c.[Status] = 1 THEN CodesCTE.CodeId
						 WHEN c.[Status] = 2 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[GlobalThesaurusUsers] c
                 JOIN 
                     CodesCTE ON 
                     (c.[Status] = 0 AND CodesCTE.rn = 1) OR 
                     (c.[Status] = 1 AND CodesCTE.rn = 2) OR 
					 (c.[Status] = 2 AND CodesCTE.rn = 3);
            ");

            migrationBuilder.Sql($@"CREATE or alter  VIEW [dbo].[PersonnelViews]
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
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {deletedStateCode}))	
							and GETDATE() BETWEEN org.[ActiveFrom] AND org.[ActiveTo] 
							order by org.Name
							FOR XML PATH('')), 1, 1, '') as PersonnelOrganizations
					,(SELECT cast(org.OrganizationId as varchar) 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.PersonnelId = personnelOrg.PersonnelId
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {deletedStateCode}))
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
            ");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ThesaurusMerges");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ThesaurusEntries");

            migrationBuilder.DropColumn(
                name: "State",
                table: "PersonnelOrganizations");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropColumn(
                name: "CommentState",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Versions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Versions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ThesaurusMerges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ThesaurusEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "PersonnelOrganizations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "GlobalThesaurusUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GlobalThesaurusUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommentState",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
