using Microsoft.EntityFrameworkCore.Migrations;
using sReportsV2.Common.Enums;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSmartOncologyEnumColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            int contraceptionCodeSetId = (int)CodeSetList.Contraception;
            int diseaseContextCodeSetId = (int)CodeSetList.DiseaseContext;
            int instanceStateCodeSetId = (int)CodeSetList.InstanceState;
            int chemotherapySchemaInstanceActionTypeCodeSetId = (int)CodeSetList.ChemotherapySchemaInstanceActionType;

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {contraceptionCodeSetId}
                 )
                 UPDATE c
                 SET c.ContraceptionCD = 
                     CASE 
                         WHEN c.Contraception = 0 THEN CodesCTE.CodeId
                         WHEN c.Contraception = 1 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[PatientChemotherapyDatas] c
                 JOIN 
                     CodesCTE ON 
                     (c.Contraception = 0 AND CodesCTE.rn = 1) OR 
                     (c.Contraception = 1 AND CodesCTE.rn = 2);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {diseaseContextCodeSetId}
                 )
                 UPDATE c
                 SET c.DiseaseContextAtCurrentPresentationCD = 
                     CASE 
                         WHEN c.DiseaseContextAtCurrentPresentation = 0 THEN CodesCTE.CodeId
                         WHEN c.DiseaseContextAtCurrentPresentation = 1 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[PatientChemotherapyDatas] c
                 JOIN 
                     CodesCTE ON 
                     (c.DiseaseContextAtCurrentPresentation = 0 AND CodesCTE.rn = 1) OR 
                     (c.DiseaseContextAtCurrentPresentation = 1 AND CodesCTE.rn = 2);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {diseaseContextCodeSetId}
                 )
                 UPDATE c
                 SET c.DiseaseContextAtInitialPresentationCD = 
                     CASE 
                         WHEN c.DiseaseContextAtInitialPresentation = 0 THEN CodesCTE.CodeId
                         WHEN c.DiseaseContextAtInitialPresentation = 1 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[PatientChemotherapyDatas] c
                 JOIN 
                     CodesCTE ON 
                     (c.DiseaseContextAtInitialPresentation = 0 AND CodesCTE.rn = 1) OR 
                     (c.DiseaseContextAtInitialPresentation = 1 AND CodesCTE.rn = 2);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                     SELECT 
                         CodeId,
                         ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                     FROM 
                         Codes
                     WHERE 
                         CodeSetId = {instanceStateCodeSetId}
                 )
                 UPDATE c
                 SET c.StateCD = 
                     CASE 
                         WHEN c.State = 0 THEN CodesCTE.CodeId
                         WHEN c.State = 1 THEN CodesCTE.CodeId
                     END
                 FROM 
                     [dbo].[ChemotherapySchemaInstances] c
                 JOIN 
                     CodesCTE ON 
                     (c.State = 0 AND CodesCTE.rn = 1) OR 
                     (c.State = 1 AND CodesCTE.rn = 2);
            ");

            migrationBuilder.Sql($@"
                ;WITH CodesCTE AS (
                    SELECT 
                        CodeId,
                        ROW_NUMBER() OVER (PARTITION BY CodeSetId ORDER BY CodeId) AS rn
                    FROM 
                        Codes
                    WHERE 
                        CodeSetId = {chemotherapySchemaInstanceActionTypeCodeSetId}
                )
                UPDATE c
                SET c.ActionTypeCd = 
                    CASE 
                        WHEN c.ActionType = 0 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 1 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 2 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 3 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 4 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 5 THEN CodesCTE.CodeId
                        WHEN c.ActionType = 6 THEN CodesCTE.CodeId
                    END
                FROM 
                    [dbo].[ChemotherapySchemaInstanceVersions] c
                JOIN 
                    CodesCTE ON 
                    (c.ActionType = 0 AND CodesCTE.rn = 1) OR 
                    (c.ActionType = 1 AND CodesCTE.rn = 2) OR 
                    (c.ActionType = 2 AND CodesCTE.rn = 3) OR 
                    (c.ActionType = 3 AND CodesCTE.rn = 4) OR 
                    (c.ActionType = 4 AND CodesCTE.rn = 5) OR 
                    (c.ActionType = 5 AND CodesCTE.rn = 6) OR 
                    (c.ActionType = 6 AND CodesCTE.rn = 7);
            ");

            migrationBuilder.DropColumn(
                name: "Contraception",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "DiseaseContextAtCurrentPresentation",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "DiseaseContextAtInitialPresentation",
                table: "PatientChemotherapyDatas");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "ChemotherapySchemaInstanceVersions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ChemotherapySchemaInstances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Contraception",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiseaseContextAtCurrentPresentation",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiseaseContextAtInitialPresentation",
                table: "PatientChemotherapyDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "ChemotherapySchemaInstanceVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ChemotherapySchemaInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
