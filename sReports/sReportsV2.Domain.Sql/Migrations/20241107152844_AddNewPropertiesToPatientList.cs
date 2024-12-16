using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPropertiesToPatientList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeDeceasedPatient",
                table: "PatientLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeDischargedPatient",
                table: "PatientLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnlyDischargedPatient",
                table: "PatientLists",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeDeceasedPatient",
                table: "PatientLists");

            migrationBuilder.DropColumn(
                name: "IncludeDischargedPatient",
                table: "PatientLists");

            migrationBuilder.DropColumn(
                name: "ShowOnlyDischargedPatient",
                table: "PatientLists");
        }
    }
}
