using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class ChemotherapySchemaPatientEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateModel(migrationBuilder);
            SetSystemVersion(migrationBuilder); 
            UpdateData(migrationBuilder);
        }

        protected void UpdateModel(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemotherapySchemaInstances_SmartOncologyPatients_PatientId",
                table: "ChemotherapySchemaInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_Communications_SmartOncologyPatients_SmartOncologyPatientId",
                table: "Communications");

            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeOfCares_SmartOncologyPatients_SmartOncologyPatientId",
                table: "EpisodeOfCares");

            migrationBuilder.DropIndex(
                name: "IX_EpisodeOfCares_SmartOncologyPatientId",
                table: "EpisodeOfCares");

            migrationBuilder.DropIndex(
                name: "IX_Communications_SmartOncologyPatientId",
                table: "Communications");

            migrationBuilder.DropColumn(
                name: "SmartOncologyPatientId",
                table: "EpisodeOfCares");

            migrationBuilder.DropColumn(
                name: "SmartOncologyPatientId",
                table: "Communications");

            migrationBuilder.CreateTable(
                name: "PatientChemotherapyDatas",
                columns: table => new
                {
                    PatientChemotherapyDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientInformedFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientInformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientInfoSignedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CopyDeliveredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapabilityToWork = table.Column<int>(type: "int", nullable: false),
                    DesireToHaveChildren = table.Column<bool>(type: "bit", nullable: false),
                    FertilityConservation = table.Column<bool>(type: "bit", nullable: false),
                    SemenCryopreservation = table.Column<bool>(type: "bit", nullable: false),
                    EggCellCryopreservation = table.Column<bool>(type: "bit", nullable: false),
                    SexualHealthAddressed = table.Column<bool>(type: "bit", nullable: false),
                    Contraception = table.Column<int>(type: "int", nullable: false),
                    ClinicalTrials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousTreatment = table.Column<bool>(type: "bit", nullable: false),
                    TreatmentInCantonalHospitalGraubunden = table.Column<bool>(type: "bit", nullable: false),
                    HistoryOfOncologicalDisease = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HospitalOrPraxisOfPreviousTreatments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiseaseContextAtInitialPresentation = table.Column<int>(type: "int", nullable: false),
                    StageAtInitialPresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiseaseContextAtCurrentPresentation = table.Column<int>(type: "int", nullable: false),
                    StageAtCurrentPresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anatomy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Morphology = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TherapeuticContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemotherapyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemotherapyCourse = table.Column<int>(type: "int", nullable: false),
                    ChemotherapyCycle = table.Column<int>(type: "int", nullable: false),
                    FirstDayOfChemotherapy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsecutiveChemotherapyDays = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PatientChemotherapyDatas", x => x.PatientChemotherapyDataId);
                    table.ForeignKey(
                        name: "FK_PatientChemotherapyDatas_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientChemotherapyDatas_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientChemotherapyDatas_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_CreatedById",
                table: "PatientChemotherapyDatas",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_EntityStateCD",
                table: "PatientChemotherapyDatas",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientChemotherapyDatas_PatientId",
                table: "PatientChemotherapyDatas",
                column: "PatientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_Patients_PatientId",
                table: "ChemotherapySchemaInstances",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        protected void SetSystemVersion(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.PatientChemotherapyDatas");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.PatientChemotherapyDatas");
        }

        protected void UpdateData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE dbo.Patients SET ( SYSTEM_VERSIONING = OFF);
                ALTER TABLE dbo.Patients DROP PERIOD FOR SYSTEM_TIME;
                ALTER TABLE dbo.PatientChemotherapyDatas SET ( SYSTEM_VERSIONING = OFF);
                ALTER TABLE dbo.PatientChemotherapyDatas DROP PERIOD FOR SYSTEM_TIME;
                GO

                DECLARE @TotalPatients INT;
                SELECT @TotalPatients = max(PatientId) FROM dbo.Patients;

                insert into dbo.Patients
	                (OrganizationId
	                ,EntryDatetime
	                ,LastUpdate
	                ,CreatedById
	                ,ActiveFrom
	                ,ActiveTo
	                ,EntityStateCD
	                ,NameGiven
	                ,NameFamily
	                ,GenderCD
	                ,BirthDate
	                ,MultipleBirthId
	                ,StartDateTime
	                ,EndDateTime
	                )
                SELECT 
      
	                1
	                ,EntryDatetime
	                ,LastUpdate
	                ,CreatedById
	                ,ActiveFrom
	                ,ActiveTo
	                ,EntityStateCD
	                ,NameGiven
	                ,NameFamily
	                ,GenderCD
	                ,BirthDate
	                ,MultipleBirthId
	                ,StartDateTime
	                ,EndDateTime
                FROM [dbo].[SmartOncologyPatients]
                ;

                insert into dbo.PatientsHistory
	                (
	                PatientId
	                ,OrganizationId
	                ,EntryDatetime
	                ,LastUpdate
	                ,CreatedById
	                ,ActiveFrom
	                ,ActiveTo
	                ,EntityStateCD
	                ,NameGiven
	                ,NameFamily
	                ,GenderCD
	                ,BirthDate
	                ,MultipleBirthId
	                ,StartDateTime
	                ,EndDateTime
	                )
                SELECT 
                      SmartOncologyPatientId + @TotalPatients
	                ,1
	                ,EntryDatetime
	                ,LastUpdate
	                ,CreatedById
	                ,ActiveFrom
	                ,ActiveTo
	                ,EntityStateCD
	                ,NameGiven
	                ,NameFamily
	                ,GenderCD
	                ,BirthDate
	                ,MultipleBirthId
	                ,StartDateTime
	                ,EndDateTime
                FROM [dbo].[SmartOncologyPatientsHistory];

                insert into dbo.PatientChemotherapyDatas
	                (
	                  PatientId
                      ,IdentificationNumber
                      ,Allergies
                      ,PatientInformedFor
                      ,PatientInformedBy
                      ,PatientInfoSignedOn
                      ,CopyDeliveredOn
                      ,CapabilityToWork
                      ,DesireToHaveChildren
                      ,FertilityConservation
                      ,SemenCryopreservation
                      ,EggCellCryopreservation
                      ,SexualHealthAddressed
                      ,Contraception
                      ,ClinicalTrials
                      ,PreviousTreatment
                      ,TreatmentInCantonalHospitalGraubunden
                      ,HistoryOfOncologicalDisease
                      ,HospitalOrPraxisOfPreviousTreatments
                      ,DiseaseContextAtInitialPresentation
                      ,StageAtInitialPresentation
                      ,DiseaseContextAtCurrentPresentation
                      ,StageAtCurrentPresentation
                      ,Anatomy
                      ,Morphology
                      ,TherapeuticContext
                      ,ChemotherapyType
                      ,ChemotherapyCourse
                      ,ChemotherapyCycle
                      ,FirstDayOfChemotherapy
                      ,ConsecutiveChemotherapyDays
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                      ,ActiveFrom
                      ,ActiveTo
                      ,EntityStateCD
                      ,StartDateTime
                      ,EndDateTime
	                )
                SELECT SmartOncologyPatientId + @TotalPatients
                      ,IdentificationNumber
                      ,Allergies
                      ,PatientInformedFor
                      ,PatientInformedBy
                      ,PatientInfoSignedOn
                      ,CopyDeliveredOn
                      ,CapabilityToWork
                      ,DesireToHaveChildren
                      ,FertilityConservation
                      ,SemenCryopreservation
                      ,EggCellCryopreservation
                      ,SexualHealthAddressed
                      ,Contraception
                      ,ClinicalTrials
                      ,PreviousTreatment
                      ,TreatmentInCantonalHospitalGraubunden
                      ,HistoryOfOncologicalDisease
                      ,HospitalOrPraxisOfPreviousTreatments
                      ,DiseaseContextAtInitialPresentation
                      ,StageAtInitialPresentation
                      ,DiseaseContextAtCurrentPresentation
                      ,StageAtCurrentPresentation
                      ,Anatomy
                      ,Morphology
                      ,TherapeuticContext
                      ,ChemotherapyType
                      ,ChemotherapyCourse
                      ,ChemotherapyCycle
                      ,FirstDayOfChemotherapy
                      ,ConsecutiveChemotherapyDays
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                      ,ActiveFrom
                      ,ActiveTo
                      ,EntityStateCD
                      ,StartDateTime
                      ,EndDateTime
                  FROM [dbo].[SmartOncologyPatients]


                insert into dbo.PatientChemotherapyDatasHistory
	                (PatientChemotherapyDataId
	                  ,PatientId
                      ,IdentificationNumber
                      ,Allergies
                      ,PatientInformedFor
                      ,PatientInformedBy
                      ,PatientInfoSignedOn
                      ,CopyDeliveredOn
                      ,CapabilityToWork
                      ,DesireToHaveChildren
                      ,FertilityConservation
                      ,SemenCryopreservation
                      ,EggCellCryopreservation
                      ,SexualHealthAddressed
                      ,Contraception
                      ,ClinicalTrials
                      ,PreviousTreatment
                      ,TreatmentInCantonalHospitalGraubunden
                      ,HistoryOfOncologicalDisease
                      ,HospitalOrPraxisOfPreviousTreatments
                      ,DiseaseContextAtInitialPresentation
                      ,StageAtInitialPresentation
                      ,DiseaseContextAtCurrentPresentation
                      ,StageAtCurrentPresentation
                      ,Anatomy
                      ,Morphology
                      ,TherapeuticContext
                      ,ChemotherapyType
                      ,ChemotherapyCourse
                      ,ChemotherapyCycle
                      ,FirstDayOfChemotherapy
                      ,ConsecutiveChemotherapyDays
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                      ,ActiveFrom
                      ,ActiveTo
                      ,EntityStateCD
                      ,StartDateTime
                      ,EndDateTime
	                )
                SELECT SmartOncologyPatientId
	                   ,SmartOncologyPatientId + @TotalPatients
                      ,IdentificationNumber
                      ,Allergies
                      ,PatientInformedFor
                      ,PatientInformedBy
                      ,PatientInfoSignedOn
                      ,CopyDeliveredOn
                      ,CapabilityToWork
                      ,DesireToHaveChildren
                      ,FertilityConservation
                      ,SemenCryopreservation
                      ,EggCellCryopreservation
                      ,SexualHealthAddressed
                      ,Contraception
                      ,ClinicalTrials
                      ,PreviousTreatment
                      ,TreatmentInCantonalHospitalGraubunden
                      ,HistoryOfOncologicalDisease
                      ,HospitalOrPraxisOfPreviousTreatments
                      ,DiseaseContextAtInitialPresentation
                      ,StageAtInitialPresentation
                      ,DiseaseContextAtCurrentPresentation
                      ,StageAtCurrentPresentation
                      ,Anatomy
                      ,Morphology
                      ,TherapeuticContext
                      ,ChemotherapyType
                      ,ChemotherapyCourse
                      ,ChemotherapyCycle
                      ,FirstDayOfChemotherapy
                      ,ConsecutiveChemotherapyDays
                      ,EntryDatetime
                      ,LastUpdate
                      ,CreatedById
                      ,ActiveFrom
                      ,ActiveTo
                      ,EntityStateCD
                      ,StartDateTime
                      ,EndDateTime
                  FROM [dbo].[SmartOncologyPatientsHistory]
                  ;

                update dbo.ChemotherapySchemaInstances set PatientId = PatientId + @TotalPatients  from dbo.ChemotherapySchemaInstances;
                GO

                ALTER TABLE dbo.Patients
                    ADD PERIOD FOR SYSTEM_TIME (StartDateTime, EndDateTime);
                ALTER TABLE dbo.Patients
                    SET(SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.PatientsHistory, DATA_CONSISTENCY_CHECK = ON));
                ALTER TABLE dbo.PatientChemotherapyDatas
                    ADD PERIOD FOR SYSTEM_TIME (StartDateTime, EndDateTime);
                ALTER TABLE dbo.PatientChemotherapyDatas
                    SET(SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.PatientChemotherapyDatasHistory, DATA_CONSISTENCY_CHECK = ON));
                GO

                ALTER TABLE dbo.SmartOncologyPatients SET ( SYSTEM_VERSIONING = OFF);
                ALTER TABLE dbo.SmartOncologyPatients DROP PERIOD FOR SYSTEM_TIME;
                GO
                drop table dbo.SmartOncologyPatients;
                drop table dbo.SmartOncologyPatientsHistory;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.PatientChemotherapyDatas");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.PatientChemotherapyDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemotherapySchemaInstances_Patients_PatientId",
                table: "ChemotherapySchemaInstances");

            migrationBuilder.DropTable(
                name: "PatientChemotherapyDatas");

            migrationBuilder.AddColumn<int>(
                name: "SmartOncologyPatientId",
                table: "EpisodeOfCares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmartOncologyPatientId",
                table: "Communications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmartOncologyPatients",
                columns: table => new
                {
                    SmartOncologyPatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    GenderCD = table.Column<int>(type: "int", nullable: true),
                    MultipleBirthId = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anatomy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapabilityToWork = table.Column<int>(type: "int", nullable: false),
                    ChemotherapyCourse = table.Column<int>(type: "int", nullable: false),
                    ChemotherapyCycle = table.Column<int>(type: "int", nullable: false),
                    ChemotherapyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalTrials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsecutiveChemotherapyDays = table.Column<int>(type: "int", nullable: true),
                    Contraception = table.Column<int>(type: "int", nullable: false),
                    CopyDeliveredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DesireToHaveChildren = table.Column<bool>(type: "bit", nullable: false),
                    DiseaseContextAtCurrentPresentation = table.Column<int>(type: "int", nullable: false),
                    DiseaseContextAtInitialPresentation = table.Column<int>(type: "int", nullable: false),
                    EggCellCryopreservation = table.Column<bool>(type: "bit", nullable: false),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FertilityConservation = table.Column<bool>(type: "bit", nullable: false),
                    FirstDayOfChemotherapy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HistoryOfOncologicalDisease = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HospitalOrPraxisOfPreviousTreatments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Morphology = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFamily = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGiven = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientInfoSignedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientInformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientInformedFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousTreatment = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    SemenCryopreservation = table.Column<bool>(type: "bit", nullable: false),
                    SexualHealthAddressed = table.Column<bool>(type: "bit", nullable: false),
                    StageAtCurrentPresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StageAtInitialPresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TherapeuticContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentInCantonalHospitalGraubunden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartOncologyPatients", x => x.SmartOncologyPatientId);
                    table.ForeignKey(
                        name: "FK_SmartOncologyPatients_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_SmartOncologyPatients_Codes_GenderCD",
                        column: x => x.GenderCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_SmartOncologyPatients_MultipleBirths_MultipleBirthId",
                        column: x => x.MultipleBirthId,
                        principalTable: "MultipleBirths",
                        principalColumn: "MultipleBirthId");
                    table.ForeignKey(
                        name: "FK_SmartOncologyPatients_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_SmartOncologyPatientId",
                table: "EpisodeOfCares",
                column: "SmartOncologyPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_SmartOncologyPatientId",
                table: "Communications",
                column: "SmartOncologyPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartOncologyPatients_CreatedById",
                table: "SmartOncologyPatients",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmartOncologyPatients_EntityStateCD",
                table: "SmartOncologyPatients",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_SmartOncologyPatients_GenderCD",
                table: "SmartOncologyPatients",
                column: "GenderCD");

            migrationBuilder.CreateIndex(
                name: "IX_SmartOncologyPatients_MultipleBirthId",
                table: "SmartOncologyPatients",
                column: "MultipleBirthId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_SmartOncologyPatients_PatientId",
                table: "ChemotherapySchemaInstances",
                column: "PatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Communications_SmartOncologyPatients_SmartOncologyPatientId",
                table: "Communications",
                column: "SmartOncologyPatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_SmartOncologyPatients_SmartOncologyPatientId",
                table: "EpisodeOfCares",
                column: "SmartOncologyPatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId");
        }
    }
}
