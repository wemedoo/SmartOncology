using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadPatientDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadPatientData",
                columns: table => new
                {
                    UploadPatientDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadPatientData", x => x.UploadPatientDataId);
                    table.ForeignKey(
                        name: "FK_UploadPatientData_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_UploadPatientData_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadPatientData_CreatedById",
                table: "UploadPatientData",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UploadPatientData_EntityStateCD",
                table: "UploadPatientData",
                column: "EntityStateCD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormThesaurusReportViews");

            migrationBuilder.DropTable(
                name: "UploadPatientData");
        }
    }
}
