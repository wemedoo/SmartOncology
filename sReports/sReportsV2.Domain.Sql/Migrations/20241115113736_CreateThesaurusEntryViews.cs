using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class CreateThesaurusEntryViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE or alter VIEW [ThesaurusEntryViews] AS
                SELECT 
                    te.ThesaurusEntryId,
                    tet.PreferredTerm,
                    tet.Definition,
	                tet.Language,
                    te.StateCD,
                    tetState.PreferredTerm AS State,
                    tet.SynonymsString,
                    tet.AbbreviationsString,
                    te.RowVersion,
                    te.EntryDatetime,
                    te.LastUpdate,
                    te.CreatedById,
                    te.ActiveFrom,
                    te.ActiveTo,
                    te.EntityStateCD,
	                te.StartDateTime,
                    te.EndDateTime
                FROM 
                    [dbo].[ThesaurusEntries] te
                JOIN 
                    [dbo].[ThesaurusEntryTranslations] tet 
                ON 
                    te.ThesaurusEntryId = tet.ThesaurusEntryId
                LEFT JOIN 
                    [dbo].[Codes] c 
                ON 
                    te.StateCD = c.CodeId
                LEFT JOIN 
                    [dbo].[ThesaurusEntryTranslations] tetState 
                ON 
                    c.ThesaurusEntryId = tetState.ThesaurusEntryId
                WHERE 
                    tet.PreferredTerm IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [ThesaurusEntryViews];");
        }
    }
}
