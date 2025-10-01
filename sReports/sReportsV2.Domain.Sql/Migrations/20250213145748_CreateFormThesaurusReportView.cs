using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class CreateFormThesaurusReportView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                create or alter view [dbo].[FormThesaurusReportViews]
                    as
                    SELECT
		                th.ThesaurusEntryId
		                ,trim(thTran.PreferredTerm) as [PreferredTerm]
                        ,codeSystem.Label as [System]
	                    ,code.[Version]
                        ,code.[Code]
                        ,code.[Value]
                        ,code.[VersionPublishDate]
                        FROM [dbo].[O4CodeableConcepts] code
                        inner join [dbo].[ThesaurusEntries] th on th.ThesaurusEntryId = code.ThesaurusEntryId
                        inner join [dbo].[ThesaurusEntryTranslations] thTran on thTran.ThesaurusEntryId = th.ThesaurusEntryId
                        inner join [dbo].[CodeSystems] codeSystem on codeSystem.CodeSystemId = code.CodeSystemId
                        where 
	                    codeSystem.Label = 'Oomnia External ID'
	                    and
	                    thTran.Language = 'en'
                        and
                        code.IsDeleted = 0
                        and 
                        GETDATE() BETWEEN th.[ActiveFrom] AND th.[ActiveTo]
                    go
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
