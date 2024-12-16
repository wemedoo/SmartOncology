using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddFksForSmartOncologyCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContraceptionCD",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateCD",
                table: "ChemotherapySchemaInstances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_ContraceptionCD",
                table: "PatientChemotherapyDatas",
                column: "ContraceptionCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas",
                column: "DiseaseContextAtCurrentPresentationCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas",
                column: "DiseaseContextAtInitialPresentationCD");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions",
                column: "ActionTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_StateCD",
                table: "ChemotherapySchemaInstances",
                column: "StateCD");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_Codes_StateCD",
                table: "ChemotherapySchemaInstances",
                column: "StateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstanceVersions_Codes_ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions",
                column: "ActionTypeCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_ContraceptionCD",
                table: "PatientChemotherapyDatas",
                column: "ContraceptionCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas",
                column: "DiseaseContextAtCurrentPresentationCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas",
                column: "DiseaseContextAtInitialPresentationCD",
                principalTable: "Codes",
                principalColumn: "CodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemotherapySchemaInstances_Codes_StateCD",
                table: "ChemotherapySchemaInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemotherapySchemaInstanceVersions_Codes_ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_ContraceptionCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientChemotherapyDatas_Codes_DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropIndex(
                name: "IX_PatientChemotherapyDatas_ContraceptionCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropIndex(
                name: "IX_PatientChemotherapyDatas_DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropIndex(
                name: "IX_PatientChemotherapyDatas_DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions");

            migrationBuilder.DropIndex(
                name: "IX_ChemotherapySchemaInstances_StateCD",
                table: "ChemotherapySchemaInstances");

            migrationBuilder.DropColumn(
                name: "ContraceptionCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "DiseaseContextAtCurrentPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "DiseaseContextAtInitialPresentationCD",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "ActionTypeCD",
                table: "ChemotherapySchemaInstanceVersions");

            migrationBuilder.DropColumn(
                name: "StateCD",
                table: "ChemotherapySchemaInstances");
        }
    }
}
