using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraPropertiesInEpisodeOfCare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpisodeOfCareWorkflows");

            migrationBuilder.DropColumn(
                name: "DiagnosisCondition",
                table: "EpisodeOfCares");

            migrationBuilder.DropColumn(
                name: "DiagnosisRank",
                table: "EpisodeOfCares");

            migrationBuilder.DropColumn(
                name: "DiagnosisRole",
                table: "EpisodeOfCares");

            migrationBuilder.AddColumn<int>(
                name: "DiagnosisConditionId",
                table: "EpisodeOfCares",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_DiagnosisConditionId",
                table: "EpisodeOfCares",
                column: "DiagnosisConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_ThesaurusEntries_DiagnosisConditionId",
                table: "EpisodeOfCares",
                column: "DiagnosisConditionId",
                principalTable: "ThesaurusEntries",
                principalColumn: "ThesaurusEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeOfCares_ThesaurusEntries_DiagnosisConditionId",
                table: "EpisodeOfCares");

            migrationBuilder.DropIndex(
                name: "IX_EpisodeOfCares_DiagnosisConditionId",
                table: "EpisodeOfCares");

            migrationBuilder.DropColumn(
                name: "DiagnosisConditionId",
                table: "EpisodeOfCares");

            migrationBuilder.AddColumn<string>(
                name: "DiagnosisCondition",
                table: "EpisodeOfCares",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagnosisRank",
                table: "EpisodeOfCares",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiagnosisRole",
                table: "EpisodeOfCares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EpisodeOfCareWorkflows",
                columns: table => new
                {
                    EpisodeOfCareWorkflowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeOfCareId = table.Column<int>(type: "int", nullable: false),
                    DiagnosisCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosisRole = table.Column<int>(type: "int", nullable: false),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
                    Submited = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeOfCareWorkflows", x => x.EpisodeOfCareWorkflowId);
                    table.ForeignKey(
                        name: "FK_EpisodeOfCareWorkflows_EpisodeOfCares_EpisodeOfCareId",
                        column: x => x.EpisodeOfCareId,
                        principalTable: "EpisodeOfCares",
                        principalColumn: "EpisodeOfCareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCareWorkflows_EpisodeOfCareId",
                table: "EpisodeOfCareWorkflows",
                column: "EpisodeOfCareId");
        }
    }
}
