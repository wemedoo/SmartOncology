using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddQueriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Queries",
                columns: table => new
                {
                    QueryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormInstanceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateById = table.Column<int>(type: "int", nullable: true),
                    ReasonCD = table.Column<int>(type: "int", nullable: false),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Queries", x => x.QueryId);
                    table.ForeignKey(
                        name: "FK_Queries_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Queries_Codes_ReasonCD",
                        column: x => x.ReasonCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Queries_Codes_StatusCD",
                        column: x => x.StatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Queries_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_Queries_Personnel_LastUpdateById",
                        column: x => x.LastUpdateById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "QueryHistories",
                columns: table => new
                {
                    QueryHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
                    LastUpdateById = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_QueryHistories", x => x.QueryHistoryId);
                    table.ForeignKey(
                        name: "FK_QueryHistories_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_QueryHistories_Codes_StatusCD",
                        column: x => x.StatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueryHistories_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_QueryHistories_Personnel_LastUpdateById",
                        column: x => x.LastUpdateById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_QueryHistories_Queries_QueryId",
                        column: x => x.QueryId,
                        principalTable: "Queries",
                        principalColumn: "QueryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Queries_CreatedById",
                table: "Queries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_EntityStateCD",
                table: "Queries",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_LastUpdateById",
                table: "Queries",
                column: "LastUpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_ReasonCD",
                table: "Queries",
                column: "ReasonCD");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_StatusCD",
                table: "Queries",
                column: "StatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_CreatedById",
                table: "QueryHistories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_EntityStateCD",
                table: "QueryHistories",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_LastUpdateById",
                table: "QueryHistories",
                column: "LastUpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_QueryId",
                table: "QueryHistories",
                column: "QueryId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_StatusCD",
                table: "QueryHistories",
                column: "StatusCD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryHistories");

            migrationBuilder.DropTable(
                name: "Queries");
        }
    }
}
