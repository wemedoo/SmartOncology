using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class ProjectDocumentRelationPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectPersonnelRelationId",
                table: "ProjectDocumentRelations",
                newName: "ProjectDocumentRelationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectDocumentRelationId",
                table: "ProjectDocumentRelations",
                newName: "ProjectPersonnelRelationId");
        }
    }
}
