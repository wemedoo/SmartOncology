using System;
using Microsoft.EntityFrameworkCore.Migrations;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdministrativeDatas",
                columns: table => new
                {
                    AdministrativeDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeDatas", x => x.AdministrativeDataId);
                });

            migrationBuilder.CreateTable(
                name: "ApiRequestLogs",
                columns: table => new
                {
                    ApiRequestLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiRequestDirection = table.Column<int>(type: "int", nullable: false),
                    RequestTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RequestPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestUriAbsolutePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HttpStatusCode = table.Column<short>(type: "smallint", nullable: true),
                    ApiName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiRequestLogs", x => x.ApiRequestLogId);
                });

            migrationBuilder.CreateTable(
                name: "BodySurfaceCalculationFormulas",
                columns: table => new
                {
                    BodySurfaceCalculationFormulaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySurfaceCalculationFormulas", x => x.BodySurfaceCalculationFormulaId);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalDomains",
                columns: table => new
                {
                    ClinicalDomainId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalDomains", x => x.ClinicalDomainId);
                });

            migrationBuilder.CreateTable(
                name: "CodeSystems",
                columns: table => new
                {
                    CodeSystemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAB = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSystems", x => x.CodeSystemId);
                });

            migrationBuilder.CreateTable(
                name: "MedicationDoseTypes",
                columns: table => new
                {
                    MedicationDoseTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Intervals = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDoseTypes", x => x.MedicationDoseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "MultipleBirths",
                columns: table => new
                {
                    MultipleBirthId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    isMultipleBorn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleBirths", x => x.MultipleBirthId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "RouteOfAdministrations",
                columns: table => new
                {
                    RouteOfAdministrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FDACode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCICondeptId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteOfAdministrations", x => x.RouteOfAdministrationId);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.UnitId);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    VersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCD = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StateCD = table.Column<int>(type: "int", nullable: true),
                    AdministrativeDataId = table.Column<int>(type: "int", nullable: false),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.VersionId);
                    table.ForeignKey(
                        name: "FK_Versions_AdministrativeDatas_AdministrativeDataId",
                        column: x => x.AdministrativeDataId,
                        principalTable: "AdministrativeDatas",
                        principalColumn: "AdministrativeDataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionModules",
                columns: table => new
                {
                    PermissionModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionModules", x => x.PermissionModuleId);
                    table.ForeignKey(
                        name: "FK_PermissionModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionModules_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChemotherapySchemaInstances",
                columns: table => new
                {
                    ChemotherapySchemaInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    ChemotherapySchemaId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemotherapySchemaInstances", x => x.ChemotherapySchemaInstanceId);
                });

            migrationBuilder.CreateTable(
                name: "ChemotherapySchemaInstanceVersions",
                columns: table => new
                {
                    ChemotherapySchemaInstanceVersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemotherapySchemaInstanceId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    FirstDelayDay = table.Column<int>(type: "int", nullable: false),
                    DelayFor = table.Column<int>(type: "int", nullable: false),
                    ReasonForDelay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemotherapySchemaInstanceVersions", x => x.ChemotherapySchemaInstanceVersionId);
                    table.ForeignKey(
                        name: "FK_ChemotherapySchemaInstanceVersions_ChemotherapySchemaInstances_ChemotherapySchemaInstanceId",
                        column: x => x.ChemotherapySchemaInstanceId,
                        principalTable: "ChemotherapySchemaInstances",
                        principalColumn: "ChemotherapySchemaInstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChemotherapySchemas",
                columns: table => new
                {
                    ChemotherapySchemaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    LengthOfCycle = table.Column<int>(type: "int", nullable: false),
                    NumOfCycles = table.Column<int>(type: "int", nullable: false),
                    AreCoursesLimited = table.Column<bool>(type: "bit", nullable: false),
                    NumOfLimitedCourses = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemotherapySchemas", x => x.ChemotherapySchemaId);
                });

            migrationBuilder.CreateTable(
                name: "Indications",
                columns: table => new
                {
                    IndicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemotherapySchemaId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indications", x => x.IndicationId);
                    table.ForeignKey(
                        name: "FK_Indications_ChemotherapySchemas_ChemotherapySchemaId",
                        column: x => x.ChemotherapySchemaId,
                        principalTable: "ChemotherapySchemas",
                        principalColumn: "ChemotherapySchemaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiteratureReferences",
                columns: table => new
                {
                    LiteratureReferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PubMedLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PubMedID = table.Column<int>(type: "int", nullable: false),
                    ShortReferenceNotation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChemotherapySchemaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiteratureReferences", x => x.LiteratureReferenceId);
                    table.ForeignKey(
                        name: "FK_LiteratureReferences_ChemotherapySchemas_ChemotherapySchemaId",
                        column: x => x.ChemotherapySchemaId,
                        principalTable: "ChemotherapySchemas",
                        principalColumn: "ChemotherapySchemaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalTrials",
                columns: table => new
                {
                    ClinicalTrialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClinicalTrialTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalTrialAcronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalTrialSponsorIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalTrialDataProviderIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalTrialRecruitmentStatusCD = table.Column<int>(type: "int", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: true),
                    ClinicalTrialIdentifier = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ClinicalTrialSponsorName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ClinicalTrialDataManagementProvider = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ClinicalTrialIdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    ClinicalTrialSponsorIdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalTrials", x => x.ClinicalTrialId);
                });

            migrationBuilder.CreateTable(
                name: "CodeAssociations",
                columns: table => new
                {
                    CodeAssociationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeAssociations", x => x.CodeAssociationId);
                });

            migrationBuilder.CreateTable(
                name: "Codes",
                columns: table => new
                {
                    CodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: false),
                    CodeSetId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codes", x => x.CodeId);
                    table.ForeignKey(
                        name: "FK_Codes_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "CodeSets",
                columns: table => new
                {
                    CodeSetId = table.Column<int>(type: "int", nullable: false),
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: false),
                    ApplicableInDesigner = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSets", x => x.CodeSetId);
                    table.ForeignKey(
                        name: "FK_CodeSets_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentStateCD = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentRef = table.Column<int>(type: "int", nullable: true),
                    FormRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "Communications",
                columns: table => new
                {
                    CommunicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preferred = table.Column<bool>(type: "bit", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    SmartOncologyPatientId = table.Column<int>(type: "int", nullable: true),
                    LanguageCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communications", x => x.CommunicationId);
                    table.ForeignKey(
                        name: "FK_Communications_Codes_LanguageCD",
                        column: x => x.LanguageCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "EncounterIdentifiers",
                columns: table => new
                {
                    EncounterIdentifierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncounterId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierPoolCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierUseCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterIdentifiers", x => x.EncounterIdentifierId);
                    table.ForeignKey(
                        name: "FK_EncounterIdentifiers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_EncounterIdentifiers_Codes_IdentifierPoolCD",
                        column: x => x.IdentifierPoolCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_EncounterIdentifiers_Codes_IdentifierTypeCD",
                        column: x => x.IdentifierTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_EncounterIdentifiers_Codes_IdentifierUseCD",
                        column: x => x.IdentifierUseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "Encounters",
                columns: table => new
                {
                    EncounterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeOfCareId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    AdmitSourceCD = table.Column<int>(type: "int", nullable: true),
                    StatusCD = table.Column<int>(type: "int", nullable: true),
                    ClassCD = table.Column<int>(type: "int", nullable: true),
                    TypeCD = table.Column<int>(type: "int", nullable: true),
                    ServiceTypeCD = table.Column<int>(type: "int", nullable: true),
                    AdmissionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DischargeDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encounters", x => x.EncounterId);
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_AdmitSourceCD",
                        column: x => x.AdmitSourceCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_ClassCD",
                        column: x => x.ClassCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_ServiceTypeCD",
                        column: x => x.ServiceTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_StatusCD",
                        column: x => x.StatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Encounters_Codes_TypeCD",
                        column: x => x.TypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "EpisodeOfCares",
                columns: table => new
                {
                    EpisodeOfCareId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
                    TypeCD = table.Column<int>(type: "int", nullable: false),
                    DiagnosisCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosisRole = table.Column<int>(type: "int", nullable: false),
                    DiagnosisRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Period_Start = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Period_End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmartOncologyPatientId = table.Column<int>(type: "int", nullable: true),
                    PersonnelTeamId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeOfCares", x => x.EpisodeOfCareId);
                    table.ForeignKey(
                        name: "FK_EpisodeOfCares_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_EpisodeOfCares_Codes_StatusCD",
                        column: x => x.StatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_EpisodeOfCares_Codes_TypeCD",
                        column: x => x.TypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "EpisodeOfCareWorkflows",
                columns: table => new
                {
                    EpisodeOfCareWorkflowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosisCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosisRole = table.Column<int>(type: "int", nullable: false),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    Submited = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
                    EpisodeOfCareId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ErrorMessageLogs",
                columns: table => new
                {
                    ErrorMessageLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HL7MessageLogId = table.Column<int>(type: "int", nullable: false),
                    ErrorTypeCD = table.Column<int>(type: "int", nullable: true),
                    ErrorText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HL7EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceSystemCD = table.Column<int>(type: "int", nullable: true),
                    TransactionDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorMessageLogs", x => x.ErrorMessageLogId);
                    table.ForeignKey(
                        name: "FK_ErrorMessageLogs_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ErrorMessageLogs_Codes_ErrorTypeCD",
                        column: x => x.ErrorTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ErrorMessageLogs_Codes_SourceSystemCD",
                        column: x => x.SourceSystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "FormCodeRelations",
                columns: table => new
                {
                    FormCodeRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeCD = table.Column<int>(type: "int", nullable: true),
                    FormId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormCodeRelations", x => x.FormCodeRelationId);
                    table.ForeignKey(
                        name: "FK_FormCodeRelations_Codes_CodeCD",
                        column: x => x.CodeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_FormCodeRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "GlobalThesaurusRoles",
                columns: table => new
                {
                    GlobalThesaurusRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalThesaurusRoles", x => x.GlobalThesaurusRoleId);
                    table.ForeignKey(
                        name: "FK_GlobalThesaurusRoles_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "GlobalThesaurusUserRoles",
                columns: table => new
                {
                    GlobalThesaurusUserRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalThesaurusUserId = table.Column<int>(type: "int", nullable: false),
                    GlobalThesaurusRoleId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalThesaurusUserRoles", x => x.GlobalThesaurusUserRoleId);
                    table.ForeignKey(
                        name: "FK_GlobalThesaurusUserRoles_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_GlobalThesaurusUserRoles_GlobalThesaurusRoles_GlobalThesaurusRoleId",
                        column: x => x.GlobalThesaurusRoleId,
                        principalTable: "GlobalThesaurusRoles",
                        principalColumn: "GlobalThesaurusRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlobalThesaurusUsers",
                columns: table => new
                {
                    GlobalThesaurusUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceCD = table.Column<int>(type: "int", nullable: true),
                    Affiliation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCD = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalThesaurusUsers", x => x.GlobalThesaurusUserId);
                    table.ForeignKey(
                        name: "FK_GlobalThesaurusUsers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "HL7MessageLogs",
                columns: table => new
                {
                    HL7MessageLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageControlId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HL7MessageLogs", x => x.HL7MessageLogId);
                    table.ForeignKey(
                        name: "FK_HL7MessageLogs_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "InboundAliases",
                columns: table => new
                {
                    AliasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeId = table.Column<int>(type: "int", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutboundAliasId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundAliases", x => x.AliasId);
                    table.ForeignKey(
                        name: "FK_InboundAliases_Codes_CodeId",
                        column: x => x.CodeId,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundAliases_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "MedicationDoseInstances",
                columns: table => new
                {
                    MedicationDoseInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntervalId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MedicationInstanceId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDoseInstances", x => x.MedicationDoseInstanceId);
                    table.ForeignKey(
                        name: "FK_MedicationDoseInstances_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "MedicationDoseTimeInstances",
                columns: table => new
                {
                    MedicationDoseTimeInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MedicationDoseInstanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDoseTimeInstances", x => x.MedicationDoseTimeInstanceId);
                    table.ForeignKey(
                        name: "FK_MedicationDoseTimeInstances_MedicationDoseInstances_MedicationDoseInstanceId",
                        column: x => x.MedicationDoseInstanceId,
                        principalTable: "MedicationDoseInstances",
                        principalColumn: "MedicationDoseInstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationDoses",
                columns: table => new
                {
                    MedicationDoseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IntervalId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDoses", x => x.MedicationDoseId);
                    table.ForeignKey(
                        name: "FK_MedicationDoses_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "MedicationDoseTimes",
                columns: table => new
                {
                    MedicationDoseTimeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicationDoseId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDoseTimes", x => x.MedicationDoseTimeId);
                    table.ForeignKey(
                        name: "FK_MedicationDoseTimes_MedicationDoses_MedicationDoseId",
                        column: x => x.MedicationDoseId,
                        principalTable: "MedicationDoses",
                        principalColumn: "MedicationDoseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationInstances",
                columns: table => new
                {
                    MedicationInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    ChemotherapySchemaInstanceId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationInstances", x => x.MedicationInstanceId);
                    table.ForeignKey(
                        name: "FK_MedicationInstances_ChemotherapySchemaInstances_ChemotherapySchemaInstanceId",
                        column: x => x.ChemotherapySchemaInstanceId,
                        principalTable: "ChemotherapySchemaInstances",
                        principalColumn: "ChemotherapySchemaInstanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationInstances_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "MedicationReplacements",
                columns: table => new
                {
                    MedicationReplacementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemotherapySchemaInstanceId = table.Column<int>(type: "int", nullable: false),
                    ReplaceMedicationId = table.Column<int>(type: "int", nullable: false),
                    ReplaceWithMedicationId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationReplacements", x => x.MedicationReplacementId);
                    table.ForeignKey(
                        name: "FK_MedicationReplacements_ChemotherapySchemaInstances_ChemotherapySchemaInstanceId",
                        column: x => x.ChemotherapySchemaInstanceId,
                        principalTable: "ChemotherapySchemaInstances",
                        principalColumn: "ChemotherapySchemaInstanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationReplacements_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_MedicationReplacements_MedicationInstances_ReplaceMedicationId",
                        column: x => x.ReplaceMedicationId,
                        principalTable: "MedicationInstances",
                        principalColumn: "MedicationInstanceId");
                    table.ForeignKey(
                        name: "FK_MedicationReplacements_MedicationInstances_ReplaceWithMedicationId",
                        column: x => x.ReplaceWithMedicationId,
                        principalTable: "MedicationInstances",
                        principalColumn: "MedicationInstanceId");
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreparationInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteOfAdministration = table.Column<int>(type: "int", nullable: false),
                    BodySurfaceCalculationFormula = table.Column<int>(type: "int", nullable: false),
                    SameDoseForEveryAplication = table.Column<bool>(type: "bit", nullable: false),
                    HasMaximalCumulativeDose = table.Column<bool>(type: "bit", nullable: false),
                    CumulativeDose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeekendHolidaysExcluded = table.Column<bool>(type: "bit", nullable: false),
                    MaxDayNumberOfApplicationiDelay = table.Column<int>(type: "int", nullable: true),
                    IsSupportiveMedication = table.Column<bool>(type: "bit", nullable: false),
                    SupportiveMedicationReserve = table.Column<bool>(type: "bit", nullable: false),
                    SupportiveMedicationAlternative = table.Column<bool>(type: "bit", nullable: false),
                    ChemotherapySchemaId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    CumulativeDoseUnitId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.MedicationId);
                    table.ForeignKey(
                        name: "FK_Medications_ChemotherapySchemas_ChemotherapySchemaId",
                        column: x => x.ChemotherapySchemaId,
                        principalTable: "ChemotherapySchemas",
                        principalColumn: "ChemotherapySchemaId");
                    table.ForeignKey(
                        name: "FK_Medications_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Medications_Units_CumulativeDoseUnitId",
                        column: x => x.CumulativeDoseUnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                    table.ForeignKey(
                        name: "FK_Medications_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "O4CodeableConcepts",
                columns: table => new
                {
                    O4CodeableConceptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionPublishDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CodeSystemId = table.Column<int>(type: "int", nullable: false),
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_O4CodeableConcepts", x => x.O4CodeableConceptId);
                    table.ForeignKey(
                        name: "FK_O4CodeableConcepts_CodeSystems_CodeSystemId",
                        column: x => x.CodeSystemId,
                        principalTable: "CodeSystems",
                        principalColumn: "CodeSystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAddresses",
                columns: table => new
                {
                    OrganizationAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCD = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    AddressTypeCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAddresses", x => x.OrganizationAddressId);
                    table.ForeignKey(
                        name: "FK_OrganizationAddresses_Codes_AddressTypeCD",
                        column: x => x.AddressTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationAddresses_Codes_CountryCD",
                        column: x => x.CountryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationAddresses_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationClinicalDomains",
                columns: table => new
                {
                    OrganizationClinicalDomainId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    ClinicalDomainCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationClinicalDomains", x => x.OrganizationClinicalDomainId);
                    table.ForeignKey(
                        name: "FK_OrganizationClinicalDomains_Codes_ClinicalDomainCD",
                        column: x => x.ClinicalDomainCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationClinicalDomains_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationCommunicationEntities",
                columns: table => new
                {
                    OrgCommunicationEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCommunicationEntityCD = table.Column<int>(type: "int", nullable: true),
                    PrimaryCommunicationSystemCD = table.Column<int>(type: "int", nullable: true),
                    SecondaryCommunicationSystemCD = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationCommunicationEntities", x => x.OrgCommunicationEntityId);
                    table.ForeignKey(
                        name: "FK_OrganizationCommunicationEntities_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationCommunicationEntities_Codes_OrgCommunicationEntityCD",
                        column: x => x.OrgCommunicationEntityCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationCommunicationEntities_Codes_PrimaryCommunicationSystemCD",
                        column: x => x.PrimaryCommunicationSystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationCommunicationEntities_Codes_SecondaryCommunicationSystemCD",
                        column: x => x.SecondaryCommunicationSystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationIdentifiers",
                columns: table => new
                {
                    OrganizationIdentifierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierPoolCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierUseCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationIdentifiers", x => x.OrganizationIdentifierId);
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Codes_IdentifierPoolCD",
                        column: x => x.IdentifierPoolCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Codes_IdentifierTypeCD",
                        column: x => x.IdentifierTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Codes_IdentifierUseCD",
                        column: x => x.IdentifierUseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRelations",
                columns: table => new
                {
                    OrganizationRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRelations", x => x.OrganizationRelationId);
                    table.ForeignKey(
                        name: "FK_OrganizationRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    OrganizationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypesString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Impressum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumOfUsers = table.Column<int>(type: "int", nullable: false),
                    OrganizationRelationId = table.Column<int>(type: "int", nullable: true),
                    OrganizationAddressId = table.Column<int>(type: "int", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneOffset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.OrganizationId);
                    table.ForeignKey(
                        name: "FK_Organizations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Organizations_OrganizationAddresses_OrganizationAddressId",
                        column: x => x.OrganizationAddressId,
                        principalTable: "OrganizationAddresses",
                        principalColumn: "OrganizationAddressId");
                    table.ForeignKey(
                        name: "FK_Organizations_OrganizationRelations_OrganizationRelationId",
                        column: x => x.OrganizationRelationId,
                        principalTable: "OrganizationRelations",
                        principalColumn: "OrganizationRelationId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelConfigs",
                columns: table => new
                {
                    PersonnelConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageSize = table.Column<int>(type: "int", nullable: false),
                    ActiveLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneOffset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveOrganizationId = table.Column<int>(type: "int", nullable: true),
                    SuggestedFormsString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PredefinedFormsString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelConfigs", x => x.PersonnelConfigId);
                    table.ForeignKey(
                        name: "FK_PersonnelConfigs_Organizations_ActiveOrganizationId",
                        column: x => x.ActiveOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTelecoms",
                columns: table => new
                {
                    OrganizationTelecomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    SystemCD = table.Column<int>(type: "int", nullable: true),
                    UseCD = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTelecoms", x => x.OrganizationTelecomId);
                    table.ForeignKey(
                        name: "FK_OrganizationTelecoms_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationTelecoms_Codes_SystemCD",
                        column: x => x.SystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationTelecoms_Codes_UseCD",
                        column: x => x.UseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OrganizationTelecoms_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboundAliases",
                columns: table => new
                {
                    AliasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeId = table.Column<int>(type: "int", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundAliases", x => x.AliasId);
                    table.ForeignKey(
                        name: "FK_OutboundAliases_Codes_CodeId",
                        column: x => x.CodeId,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundAliases_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OutsideUserAddresses",
                columns: table => new
                {
                    OutsideUserAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCD = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    AddressTypeCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsideUserAddresses", x => x.OutsideUserAddressId);
                    table.ForeignKey(
                        name: "FK_OutsideUserAddresses_Codes_AddressTypeCD",
                        column: x => x.AddressTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OutsideUserAddresses_Codes_CountryCD",
                        column: x => x.CountryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OutsideUserAddresses_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "OutsideUsers",
                columns: table => new
                {
                    OutsideUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutsideUserAddressId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsideUsers", x => x.OutsideUserId);
                    table.ForeignKey(
                        name: "FK_OutsideUsers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_OutsideUsers_OutsideUserAddresses_OutsideUserAddressId",
                        column: x => x.OutsideUserAddressId,
                        principalTable: "OutsideUserAddresses",
                        principalColumn: "OutsideUserAddressId");
                });

            migrationBuilder.CreateTable(
                name: "PatientAddresses",
                columns: table => new
                {
                    PatientAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCD = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    AddressTypeCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAddresses", x => x.PatientAddressId);
                    table.ForeignKey(
                        name: "FK_PatientAddresses_Codes_AddressTypeCD",
                        column: x => x.AddressTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientAddresses_Codes_CountryCD",
                        column: x => x.CountryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientAddresses_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientContactAddresses",
                columns: table => new
                {
                    PatientContactAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientContactId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCD = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    AddressTypeCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientContactAddresses", x => x.PatientContactAddressId);
                    table.ForeignKey(
                        name: "FK_PatientContactAddresses_Codes_AddressTypeCD",
                        column: x => x.AddressTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContactAddresses_Codes_CountryCD",
                        column: x => x.CountryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContactAddresses_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientContacts",
                columns: table => new
                {
                    PatientContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    NameGiven = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFamily = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenderCD = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactRelationshipCD = table.Column<int>(type: "int", nullable: true),
                    ContactRoleCD = table.Column<int>(type: "int", nullable: true),
                    ContactRoleStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactRoleEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientContacts", x => x.PatientContactId);
                    table.ForeignKey(
                        name: "FK_PatientContacts_Codes_ContactRelationshipCD",
                        column: x => x.ContactRelationshipCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContacts_Codes_ContactRoleCD",
                        column: x => x.ContactRoleCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContacts_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContacts_Codes_GenderCD",
                        column: x => x.GenderCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientContactTelecoms",
                columns: table => new
                {
                    PatientContactTelecomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientContactId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    SystemCD = table.Column<int>(type: "int", nullable: true),
                    UseCD = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientContactTelecoms", x => x.PatientContactTelecomId);
                    table.ForeignKey(
                        name: "FK_PatientContactTelecoms_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContactTelecoms_Codes_SystemCD",
                        column: x => x.SystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContactTelecoms_Codes_UseCD",
                        column: x => x.UseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientContactTelecoms_PatientContacts_PatientContactId",
                        column: x => x.PatientContactId,
                        principalTable: "PatientContacts",
                        principalColumn: "PatientContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientIdentifiers",
                columns: table => new
                {
                    PatientIdentifierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierPoolCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierUseCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientIdentifiers", x => x.PatientIdentifierId);
                    table.ForeignKey(
                        name: "FK_PatientIdentifiers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientIdentifiers_Codes_IdentifierPoolCD",
                        column: x => x.IdentifierPoolCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientIdentifiers_Codes_IdentifierTypeCD",
                        column: x => x.IdentifierTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientIdentifiers_Codes_IdentifierUseCD",
                        column: x => x.IdentifierUseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientListPatientRelations",
                columns: table => new
                {
                    PatientListPatientRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PatientListId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientListPatientRelations", x => x.PatientListPatientRelationId);
                    table.ForeignKey(
                        name: "FK_PatientListPatientRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientListPersonnelRelations",
                columns: table => new
                {
                    PatientListPersonnelRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    PatientListId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientListPersonnelRelations", x => x.PatientListPersonnelRelationId);
                    table.ForeignKey(
                        name: "FK_PatientListPersonnelRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                });

            migrationBuilder.CreateTable(
                name: "PatientLists",
                columns: table => new
                {
                    PatientListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientListName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArePatientsSelected = table.Column<bool>(type: "bit", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EpisodeOfCareTypeCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelTeamId = table.Column<int>(type: "int", nullable: true),
                    EncounterTypeCD = table.Column<int>(type: "int", nullable: true),
                    AttendingDoctorId = table.Column<int>(type: "int", nullable: true),
                    EncounterStatusCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientLists", x => x.PatientListId);
                    table.ForeignKey(
                        name: "FK_PatientLists_Codes_EncounterStatusCD",
                        column: x => x.EncounterStatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientLists_Codes_EncounterTypeCD",
                        column: x => x.EncounterTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientLists_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientLists_Codes_EpisodeOfCareTypeCD",
                        column: x => x.EpisodeOfCareTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    CitizenshipCD = table.Column<int>(type: "int", nullable: true),
                    ReligionCD = table.Column<int>(type: "int", nullable: true),
                    MaritalStatusCD = table.Column<int>(type: "int", nullable: true),
                    DeceasedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deceased = table.Column<bool>(type: "bit", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(129)", maxLength: 129, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    NameGiven = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFamily = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenderCD = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MultipleBirthId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Codes_CitizenshipCD",
                        column: x => x.CitizenshipCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Patients_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Patients_Codes_GenderCD",
                        column: x => x.GenderCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Patients_Codes_MaritalStatusCD",
                        column: x => x.MaritalStatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Patients_Codes_ReligionCD",
                        column: x => x.ReligionCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Patients_MultipleBirths_MultipleBirthId",
                        column: x => x.MultipleBirthId,
                        principalTable: "MultipleBirths",
                        principalColumn: "MultipleBirthId");
                });

            migrationBuilder.CreateTable(
                name: "PatientTelecoms",
                columns: table => new
                {
                    PatientTelecomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    SystemCD = table.Column<int>(type: "int", nullable: true),
                    UseCD = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTelecoms", x => x.PatientTelecomId);
                    table.ForeignKey(
                        name: "FK_PatientTelecoms_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientTelecoms_Codes_SystemCD",
                        column: x => x.SystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientTelecoms_Codes_UseCD",
                        column: x => x.UseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PatientTelecoms_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    PersonnelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DayOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDoctor = table.Column<bool>(type: "bit", nullable: false),
                    PrefixCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelTypeCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelConfigId = table.Column<int>(type: "int", nullable: false),
                    PersonnelOccupationId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_Personnel_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Personnel_Codes_PersonnelTypeCD",
                        column: x => x.PersonnelTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Personnel_Codes_PrefixCD",
                        column: x => x.PrefixCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Personnel_PersonnelConfigs_PersonnelConfigId",
                        column: x => x.PersonnelConfigId,
                        principalTable: "PersonnelConfigs",
                        principalColumn: "PersonnelConfigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Personnel_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelAcademicPositions",
                columns: table => new
                {
                    PersonnelAcademicPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    AcademicPositionCD = table.Column<int>(type: "int", nullable: true),
                    AcademicPositionTypeCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelAcademicPositions", x => x.PersonnelAcademicPositionId);
                    table.ForeignKey(
                        name: "FK_PersonnelAcademicPositions_Codes_AcademicPositionCD",
                        column: x => x.AcademicPositionCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAcademicPositions_Codes_AcademicPositionTypeCD",
                        column: x => x.AcademicPositionTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAcademicPositions_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAcademicPositions_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelAcademicPositions_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelAddresses",
                columns: table => new
                {
                    PersonnelAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCD = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    AddressTypeCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelAddresses", x => x.PersonnelAddressId);
                    table.ForeignKey(
                        name: "FK_PersonnelAddresses_Codes_AddressTypeCD",
                        column: x => x.AddressTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAddresses_Codes_CountryCD",
                        column: x => x.CountryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAddresses_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelAddresses_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelAddresses_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelEncounterRelations",
                columns: table => new
                {
                    PersonnelEncounterRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncounterId = table.Column<int>(type: "int", nullable: false),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    RelationTypeCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelEncounterRelations", x => x.PersonnelEncounterRelationId);
                    table.ForeignKey(
                        name: "FK_PersonnelEncounterRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelEncounterRelations_Codes_RelationTypeCD",
                        column: x => x.RelationTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelEncounterRelations_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "EncounterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelEncounterRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_PersonnelEncounterRelations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelIdentifiers",
                columns: table => new
                {
                    PersonnelIdentifierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdentifierTypeCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierPoolCD = table.Column<int>(type: "int", nullable: true),
                    IdentifierUseCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelIdentifiers", x => x.PersonnelIdentifierId);
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Codes_IdentifierPoolCD",
                        column: x => x.IdentifierPoolCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Codes_IdentifierTypeCD",
                        column: x => x.IdentifierTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Codes_IdentifierUseCD",
                        column: x => x.IdentifierUseCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelIdentifiers_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelOccupations",
                columns: table => new
                {
                    PersonnelOccupationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    OccupationCategoryCD = table.Column<int>(type: "int", nullable: false),
                    OccupationSubCategoryCD = table.Column<int>(type: "int", nullable: false),
                    OccupationCD = table.Column<int>(type: "int", nullable: false),
                    PersonnelSeniorityCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelOccupations", x => x.PersonnelOccupationId);
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Codes_OccupationCD",
                        column: x => x.OccupationCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Codes_OccupationCategoryCD",
                        column: x => x.OccupationCategoryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Codes_OccupationSubCategoryCD",
                        column: x => x.OccupationSubCategoryCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Codes_PersonnelSeniorityCD",
                        column: x => x.PersonnelSeniorityCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_PersonnelOccupations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelOrganizations",
                columns: table => new
                {
                    PersonnelOrganizationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPracticioner = table.Column<bool>(type: "bit", nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeniorityLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubSpeciality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelOrganizations", x => x.PersonnelOrganizationId);
                    table.ForeignKey(
                        name: "FK_PersonnelOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelOrganizations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelPositions",
                columns: table => new
                {
                    PersonnelPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPositions", x => x.PersonnelPositionId);
                    table.ForeignKey(
                        name: "FK_PersonnelPositions_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelPositions_Codes_PositionCD",
                        column: x => x.PositionCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelPositions_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelPositions_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelTeams",
                columns: table => new
                {
                    PersonnelTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelTeams", x => x.PersonnelTeamId);
                    table.ForeignKey(
                        name: "FK_PersonnelTeams_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeams_Codes_TypeCD",
                        column: x => x.TypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeams_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PositionPermissions",
                columns: table => new
                {
                    PositionPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCD = table.Column<int>(type: "int", nullable: true),
                    PermissionModuleId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionPermissions", x => x.PositionPermissionId);
                    table.ForeignKey(
                        name: "FK_PositionPermissions_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PositionPermissions_Codes_PositionCD",
                        column: x => x.PositionCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PositionPermissions_PermissionModules_PermissionModuleId",
                        column: x => x.PermissionModuleId,
                        principalTable: "PermissionModules",
                        principalColumn: "PermissionModuleId");
                    table.ForeignKey(
                        name: "FK_PositionPermissions_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectTypeCD = table.Column<int>(type: "int", nullable: true),
                    ProjectStartDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ProjectEndDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Projects_Codes_ProjectTypeCD",
                        column: x => x.ProjectTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Projects_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "SmartOncologyPatients",
                columns: table => new
                {
                    SmartOncologyPatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true),
                    NameGiven = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFamily = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenderCD = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MultipleBirthId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TaskDocuments",
                columns: table => new
                {
                    TaskDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskDocumentCD = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDocuments", x => x.TaskDocumentId);
                    table.ForeignKey(
                        name: "FK_TaskDocuments_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_TaskDocuments_Codes_TaskDocumentCD",
                        column: x => x.TaskDocumentCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskDocuments_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "ThesaurusEntries",
                columns: table => new
                {
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateCD = table.Column<int>(type: "int", nullable: true),
                    AdministrativeDataId = table.Column<int>(type: "int", nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriClassLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriClassGUI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriSourceLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriSourceGUI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesaurusEntries", x => x.ThesaurusEntryId);
                    table.ForeignKey(
                        name: "FK_ThesaurusEntries_AdministrativeDatas_AdministrativeDataId",
                        column: x => x.AdministrativeDataId,
                        principalTable: "AdministrativeDatas",
                        principalColumn: "AdministrativeDataId");
                    table.ForeignKey(
                        name: "FK_ThesaurusEntries_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThesaurusEntries_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "ThesaurusMerges",
                columns: table => new
                {
                    ThesaurusMergeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewThesaurus = table.Column<int>(type: "int", nullable: false),
                    OldThesaurus = table.Column<int>(type: "int", nullable: false),
                    StateCD = table.Column<int>(type: "int", nullable: false),
                    CompletedCollectionsString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailedCollectionsString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesaurusMerges", x => x.ThesaurusMergeId);
                    table.ForeignKey(
                        name: "FK_ThesaurusMerges_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ThesaurusMerges_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HL7MessageLogId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    EncounterId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FhirResource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HL7EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceSystemCD = table.Column<int>(type: "int", nullable: true),
                    TransactionDirectionCD = table.Column<int>(type: "int", nullable: true),
                    TransactionDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Transactions_Codes_SourceSystemCD",
                        column: x => x.SourceSystemCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Transactions_Codes_TransactionDirectionCD",
                        column: x => x.TransactionDirectionCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Transactions_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "EncounterId");
                    table.ForeignKey(
                        name: "FK_Transactions_HL7MessageLogs_HL7MessageLogId",
                        column: x => x.HL7MessageLogId,
                        principalTable: "HL7MessageLogs",
                        principalColumn: "HL7MessageLogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_Transactions_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelTeamOrganizationRelations",
                columns: table => new
                {
                    PersonnelTeamOrganizationRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelTeamId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    RelationTypeCD = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelTeamOrganizationRelations", x => x.PersonnelTeamOrganizationRelationId);
                    table.ForeignKey(
                        name: "FK_PersonnelTeamOrganizationRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeamOrganizationRelations_Codes_RelationTypeCD",
                        column: x => x.RelationTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeamOrganizationRelations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelTeamOrganizationRelations_PersonnelTeams_PersonnelTeamId",
                        column: x => x.PersonnelTeamId,
                        principalTable: "PersonnelTeams",
                        principalColumn: "PersonnelTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelTeamOrganizationRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelTeamRelations",
                columns: table => new
                {
                    PersonnelTeamRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationTypeCD = table.Column<int>(type: "int", nullable: true),
                    PersonnelTeamId = table.Column<int>(type: "int", nullable: true),
                    PersonnelId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelTeamRelations", x => x.PersonnelTeamRelationId);
                    table.ForeignKey(
                        name: "FK_PersonnelTeamRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeamRelations_Codes_RelationTypeCD",
                        column: x => x.RelationTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeamRelations_PersonnelTeams_PersonnelTeamId",
                        column: x => x.PersonnelTeamId,
                        principalTable: "PersonnelTeams",
                        principalColumn: "PersonnelTeamId");
                    table.ForeignKey(
                        name: "FK_PersonnelTeamRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelTeamRelations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectDocumentRelations",
                columns: table => new
                {
                    ProjectPersonnelRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    FormId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDocumentRelations", x => x.ProjectPersonnelRelationId);
                    table.ForeignKey(
                        name: "FK_ProjectDocumentRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ProjectDocumentRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_ProjectDocumentRelations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectPatientRelations",
                columns: table => new
                {
                    ProjectPatientRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPatientRelations", x => x.ProjectPatientRelationId);
                    table.ForeignKey(
                        name: "FK_ProjectPatientRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ProjectPatientRelations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectPatientRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_ProjectPatientRelations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectPersonnelRelations",
                columns: table => new
                {
                    ProjectPersonnelRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPersonnelRelations", x => x.ProjectPersonnelRelationId);
                    table.ForeignKey(
                        name: "FK_ProjectPersonnelRelations_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_ProjectPersonnelRelations_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_ProjectPersonnelRelations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectPersonnelRelations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    EncounterId = table.Column<int>(type: "int", nullable: false),
                    TaskTypeCD = table.Column<int>(type: "int", nullable: false),
                    TaskStatusCD = table.Column<int>(type: "int", nullable: false),
                    TaskPriorityCD = table.Column<int>(type: "int", nullable: true),
                    TaskClassCD = table.Column<int>(type: "int", nullable: true),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskEntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskStartDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TaskEndDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ScheduledDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TaskDocumentId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true),
                    EntryDatetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityStateCD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Tasks_Codes_EntityStateCD",
                        column: x => x.EntityStateCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId");
                    table.ForeignKey(
                        name: "FK_Tasks_Codes_TaskClassCD",
                        column: x => x.TaskClassCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Codes_TaskPriorityCD",
                        column: x => x.TaskPriorityCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Codes_TaskStatusCD",
                        column: x => x.TaskStatusCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Codes_TaskTypeCD",
                        column: x => x.TaskTypeCD,
                        principalTable: "Codes",
                        principalColumn: "CodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "EncounterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Personnel_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_Tasks_TaskDocuments_TaskDocumentId",
                        column: x => x.TaskDocumentId,
                        principalTable: "TaskDocuments",
                        principalColumn: "TaskDocumentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThesaurusEntryTranslations",
                columns: table => new
                {
                    ThesaurusEntryTranslationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThesaurusEntryId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredTerm = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    SynonymsString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AbbreviationsString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesaurusEntryTranslations", x => x.ThesaurusEntryTranslationId);
                    table.ForeignKey(
                        name: "FK_ThesaurusEntryTranslations_ThesaurusEntries_ThesaurusEntryId",
                        column: x => x.ThesaurusEntryId,
                        principalTable: "ThesaurusEntries",
                        principalColumn: "ThesaurusEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_ChemotherapySchemaId",
                table: "ChemotherapySchemaInstances",
                column: "ChemotherapySchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_CreatedById",
                table: "ChemotherapySchemaInstances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_CreatorId",
                table: "ChemotherapySchemaInstances",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_EntityStateCD",
                table: "ChemotherapySchemaInstances",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstances_PatientId",
                table: "ChemotherapySchemaInstances",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_ChemotherapySchemaInstanceId",
                table: "ChemotherapySchemaInstanceVersions",
                column: "ChemotherapySchemaInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_CreatedById",
                table: "ChemotherapySchemaInstanceVersions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_CreatorId",
                table: "ChemotherapySchemaInstanceVersions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemaInstanceVersions_EntityStateCD",
                table: "ChemotherapySchemaInstanceVersions",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemas_CreatedById",
                table: "ChemotherapySchemas",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemas_CreatorId",
                table: "ChemotherapySchemas",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChemotherapySchemas_EntityStateCD",
                table: "ChemotherapySchemas",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_ClinicalTrialIdentifierTypeCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialIdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_ClinicalTrialRecruitmentStatusCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialRecruitmentStatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_ClinicalTrialSponsorIdentifierTypeCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialSponsorIdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_CreatedById",
                table: "ClinicalTrials",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_EntityStateCD",
                table: "ClinicalTrials",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalTrials_ProjectId",
                table: "ClinicalTrials",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeAssociations_ChildId",
                table: "CodeAssociations",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeAssociations_CreatedById",
                table: "CodeAssociations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CodeAssociations_EntityStateCD",
                table: "CodeAssociations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_CodeAssociations_ParentId",
                table: "CodeAssociations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_CodeSetId",
                table: "Codes",
                column: "CodeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_CreatedById",
                table: "Codes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_EntityStateCD",
                table: "Codes",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_ThesaurusEntryId",
                table: "Codes",
                column: "ThesaurusEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeSets_CreatedById",
                table: "CodeSets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CodeSets_EntityStateCD",
                table: "CodeSets",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_CodeSets_ThesaurusEntryId",
                table: "CodeSets",
                column: "ThesaurusEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_EntityStateCD",
                table: "Comments",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_LanguageCD",
                table: "Communications",
                column: "LanguageCD");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_PatientId",
                table: "Communications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_SmartOncologyPatientId",
                table: "Communications",
                column: "SmartOncologyPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_CreatedById",
                table: "EncounterIdentifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_EncounterId",
                table: "EncounterIdentifiers",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_EntityStateCD",
                table: "EncounterIdentifiers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_IdentifierPoolCD",
                table: "EncounterIdentifiers",
                column: "IdentifierPoolCD");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_IdentifierTypeCD",
                table: "EncounterIdentifiers",
                column: "IdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterIdentifiers_IdentifierUseCD",
                table: "EncounterIdentifiers",
                column: "IdentifierUseCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_AdmitSourceCD",
                table: "Encounters",
                column: "AdmitSourceCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_ClassCD",
                table: "Encounters",
                column: "ClassCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_CreatedById",
                table: "Encounters",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_EntityStateCD",
                table: "Encounters",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_EpisodeOfCareId",
                table: "Encounters",
                column: "EpisodeOfCareId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_PatientId",
                table: "Encounters",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_ServiceTypeCD",
                table: "Encounters",
                column: "ServiceTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_StatusCD",
                table: "Encounters",
                column: "StatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_TypeCD",
                table: "Encounters",
                column: "TypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_CreatedById",
                table: "EpisodeOfCares",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_EntityStateCD",
                table: "EpisodeOfCares",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_PatientId",
                table: "EpisodeOfCares",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_PersonnelTeamId",
                table: "EpisodeOfCares",
                column: "PersonnelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_SmartOncologyPatientId",
                table: "EpisodeOfCares",
                column: "SmartOncologyPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_StatusCD",
                table: "EpisodeOfCares",
                column: "StatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCares_TypeCD",
                table: "EpisodeOfCares",
                column: "TypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeOfCareWorkflows_EpisodeOfCareId",
                table: "EpisodeOfCareWorkflows",
                column: "EpisodeOfCareId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorMessageLogs_CreatedById",
                table: "ErrorMessageLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorMessageLogs_EntityStateCD",
                table: "ErrorMessageLogs",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorMessageLogs_ErrorTypeCD",
                table: "ErrorMessageLogs",
                column: "ErrorTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorMessageLogs_HL7MessageLogId",
                table: "ErrorMessageLogs",
                column: "HL7MessageLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorMessageLogs_SourceSystemCD",
                table: "ErrorMessageLogs",
                column: "SourceSystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_FormCodeRelations_CodeCD",
                table: "FormCodeRelations",
                column: "CodeCD");

            migrationBuilder.CreateIndex(
                name: "IX_FormCodeRelations_CreatedById",
                table: "FormCodeRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormCodeRelations_EntityStateCD",
                table: "FormCodeRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusRoles_CreatedById",
                table: "GlobalThesaurusRoles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusRoles_EntityStateCD",
                table: "GlobalThesaurusRoles",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUserRoles_CreatedById",
                table: "GlobalThesaurusUserRoles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUserRoles_EntityStateCD",
                table: "GlobalThesaurusUserRoles",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUserRoles_GlobalThesaurusRoleId",
                table: "GlobalThesaurusUserRoles",
                column: "GlobalThesaurusRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUserRoles_GlobalThesaurusUserId",
                table: "GlobalThesaurusUserRoles",
                column: "GlobalThesaurusUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUsers_CreatedById",
                table: "GlobalThesaurusUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUsers_EntityStateCD",
                table: "GlobalThesaurusUsers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_HL7MessageLogs_CreatedById",
                table: "HL7MessageLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HL7MessageLogs_EntityStateCD",
                table: "HL7MessageLogs",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_InboundAliases_CodeId",
                table: "InboundAliases",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundAliases_CreatedById",
                table: "InboundAliases",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InboundAliases_EntityStateCD",
                table: "InboundAliases",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_InboundAliases_OutboundAliasId",
                table: "InboundAliases",
                column: "OutboundAliasId");

            migrationBuilder.CreateIndex(
                name: "IX_Indications_ChemotherapySchemaId",
                table: "Indications",
                column: "ChemotherapySchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReferences_ChemotherapySchemaId",
                table: "LiteratureReferences",
                column: "ChemotherapySchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoseInstances_MedicationInstanceId",
                table: "MedicationDoseInstances",
                column: "MedicationInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoseInstances_UnitId",
                table: "MedicationDoseInstances",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoses_MedicationId",
                table: "MedicationDoses",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoses_UnitId",
                table: "MedicationDoses",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoseTimeInstances_MedicationDoseInstanceId",
                table: "MedicationDoseTimeInstances",
                column: "MedicationDoseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDoseTimes_MedicationDoseId",
                table: "MedicationDoseTimes",
                column: "MedicationDoseId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstances_ChemotherapySchemaInstanceId",
                table: "MedicationInstances",
                column: "ChemotherapySchemaInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstances_CreatedById",
                table: "MedicationInstances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstances_EntityStateCD",
                table: "MedicationInstances",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstances_MedicationId",
                table: "MedicationInstances",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_ChemotherapySchemaInstanceId",
                table: "MedicationReplacements",
                column: "ChemotherapySchemaInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_CreatedById",
                table: "MedicationReplacements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_CreatorId",
                table: "MedicationReplacements",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_EntityStateCD",
                table: "MedicationReplacements",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_ReplaceMedicationId",
                table: "MedicationReplacements",
                column: "ReplaceMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReplacements_ReplaceWithMedicationId",
                table: "MedicationReplacements",
                column: "ReplaceWithMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_ChemotherapySchemaId",
                table: "Medications",
                column: "ChemotherapySchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_CreatedById",
                table: "Medications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_CumulativeDoseUnitId",
                table: "Medications",
                column: "CumulativeDoseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_EntityStateCD",
                table: "Medications",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_UnitId",
                table: "Medications",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_O4CodeableConcepts_CodeSystemId",
                table: "O4CodeableConcepts",
                column: "CodeSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_O4CodeableConcepts_ThesaurusEntryId",
                table: "O4CodeableConcepts",
                column: "ThesaurusEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddresses_AddressTypeCD",
                table: "OrganizationAddresses",
                column: "AddressTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddresses_CountryCD",
                table: "OrganizationAddresses",
                column: "CountryCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddresses_CreatedById",
                table: "OrganizationAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddresses_EntityStateCD",
                table: "OrganizationAddresses",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationClinicalDomains_ClinicalDomainCD",
                table: "OrganizationClinicalDomains",
                column: "ClinicalDomainCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationClinicalDomains_CreatedById",
                table: "OrganizationClinicalDomains",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationClinicalDomains_EntityStateCD",
                table: "OrganizationClinicalDomains",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationClinicalDomains_OrganizationId",
                table: "OrganizationClinicalDomains",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_CreatedById",
                table: "OrganizationCommunicationEntities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_EntityStateCD",
                table: "OrganizationCommunicationEntities",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_OrganizationId",
                table: "OrganizationCommunicationEntities",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_OrgCommunicationEntityCD",
                table: "OrganizationCommunicationEntities",
                column: "OrgCommunicationEntityCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_PrimaryCommunicationSystemCD",
                table: "OrganizationCommunicationEntities",
                column: "PrimaryCommunicationSystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCommunicationEntities_SecondaryCommunicationSystemCD",
                table: "OrganizationCommunicationEntities",
                column: "SecondaryCommunicationSystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_CreatedById",
                table: "OrganizationIdentifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_EntityStateCD",
                table: "OrganizationIdentifiers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_IdentifierPoolCD",
                table: "OrganizationIdentifiers",
                column: "IdentifierPoolCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_IdentifierTypeCD",
                table: "OrganizationIdentifiers",
                column: "IdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_IdentifierUseCD",
                table: "OrganizationIdentifiers",
                column: "IdentifierUseCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_OrganizationId",
                table: "OrganizationIdentifiers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelations_ChildId",
                table: "OrganizationRelations",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelations_CreatedById",
                table: "OrganizationRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelations_EntityStateCD",
                table: "OrganizationRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelations_ParentId",
                table: "OrganizationRelations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedById",
                table: "Organizations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_EntityStateCD",
                table: "Organizations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationAddressId",
                table: "Organizations",
                column: "OrganizationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationRelationId",
                table: "Organizations",
                column: "OrganizationRelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTelecoms_CreatedById",
                table: "OrganizationTelecoms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTelecoms_EntityStateCD",
                table: "OrganizationTelecoms",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTelecoms_OrganizationId",
                table: "OrganizationTelecoms",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTelecoms_SystemCD",
                table: "OrganizationTelecoms",
                column: "SystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTelecoms_UseCD",
                table: "OrganizationTelecoms",
                column: "UseCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundAliases_CodeId",
                table: "OutboundAliases",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundAliases_CreatedById",
                table: "OutboundAliases",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundAliases_EntityStateCD",
                table: "OutboundAliases",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUserAddresses_AddressTypeCD",
                table: "OutsideUserAddresses",
                column: "AddressTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUserAddresses_CountryCD",
                table: "OutsideUserAddresses",
                column: "CountryCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUserAddresses_CreatedById",
                table: "OutsideUserAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUserAddresses_EntityStateCD",
                table: "OutsideUserAddresses",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUsers_CreatedById",
                table: "OutsideUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUsers_EntityStateCD",
                table: "OutsideUsers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_OutsideUsers_OutsideUserAddressId",
                table: "OutsideUsers",
                column: "OutsideUserAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAddresses_AddressTypeCD",
                table: "PatientAddresses",
                column: "AddressTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAddresses_CountryCD",
                table: "PatientAddresses",
                column: "CountryCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAddresses_CreatedById",
                table: "PatientAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAddresses_EntityStateCD",
                table: "PatientAddresses",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAddresses_PatientId",
                table: "PatientAddresses",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactAddresses_AddressTypeCD",
                table: "PatientContactAddresses",
                column: "AddressTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactAddresses_CountryCD",
                table: "PatientContactAddresses",
                column: "CountryCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactAddresses_CreatedById",
                table: "PatientContactAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactAddresses_EntityStateCD",
                table: "PatientContactAddresses",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactAddresses_PatientContactId",
                table: "PatientContactAddresses",
                column: "PatientContactId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_ContactRelationshipCD",
                table: "PatientContacts",
                column: "ContactRelationshipCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_ContactRoleCD",
                table: "PatientContacts",
                column: "ContactRoleCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_CreatedById",
                table: "PatientContacts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_EntityStateCD",
                table: "PatientContacts",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_GenderCD",
                table: "PatientContacts",
                column: "GenderCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContacts_PatientId",
                table: "PatientContacts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactTelecoms_CreatedById",
                table: "PatientContactTelecoms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactTelecoms_EntityStateCD",
                table: "PatientContactTelecoms",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactTelecoms_PatientContactId",
                table: "PatientContactTelecoms",
                column: "PatientContactId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactTelecoms_SystemCD",
                table: "PatientContactTelecoms",
                column: "SystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactTelecoms_UseCD",
                table: "PatientContactTelecoms",
                column: "UseCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_CreatedById",
                table: "PatientIdentifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_EntityStateCD",
                table: "PatientIdentifiers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_IdentifierPoolCD",
                table: "PatientIdentifiers",
                column: "IdentifierPoolCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_IdentifierTypeCD",
                table: "PatientIdentifiers",
                column: "IdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_IdentifierUseCD",
                table: "PatientIdentifiers",
                column: "IdentifierUseCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIdentifiers_PatientId",
                table: "PatientIdentifiers",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPatientRelations_CreatedById",
                table: "PatientListPatientRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPatientRelations_EntityStateCD",
                table: "PatientListPatientRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPatientRelations_PatientId",
                table: "PatientListPatientRelations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPatientRelations_PatientListId",
                table: "PatientListPatientRelations",
                column: "PatientListId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPersonnelRelations_CreatedById",
                table: "PatientListPersonnelRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPersonnelRelations_EntityStateCD",
                table: "PatientListPersonnelRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPersonnelRelations_PatientListId",
                table: "PatientListPersonnelRelations",
                column: "PatientListId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientListPersonnelRelations_PersonnelId",
                table: "PatientListPersonnelRelations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_AttendingDoctorId",
                table: "PatientLists",
                column: "AttendingDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_CreatedById",
                table: "PatientLists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_EncounterStatusCD",
                table: "PatientLists",
                column: "EncounterStatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_EncounterTypeCD",
                table: "PatientLists",
                column: "EncounterTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_EntityStateCD",
                table: "PatientLists",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_EpisodeOfCareTypeCD",
                table: "PatientLists",
                column: "EpisodeOfCareTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLists_PersonnelTeamId",
                table: "PatientLists",
                column: "PersonnelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CitizenshipCD",
                table: "Patients",
                column: "CitizenshipCD");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CreatedById",
                table: "Patients",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_EntityStateCD",
                table: "Patients",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_GenderCD",
                table: "Patients",
                column: "GenderCD");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MaritalStatusCD",
                table: "Patients",
                column: "MaritalStatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MultipleBirthId",
                table: "Patients",
                column: "MultipleBirthId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ReligionCD",
                table: "Patients",
                column: "ReligionCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTelecoms_CreatedById",
                table: "PatientTelecoms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTelecoms_EntityStateCD",
                table: "PatientTelecoms",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTelecoms_PatientId",
                table: "PatientTelecoms",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTelecoms_SystemCD",
                table: "PatientTelecoms",
                column: "SystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTelecoms_UseCD",
                table: "PatientTelecoms",
                column: "UseCD");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionModules_ModuleId",
                table: "PermissionModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionModules_PermissionId",
                table: "PermissionModules",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_CreatedById",
                table: "Personnel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_EntityStateCD",
                table: "Personnel",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_PersonnelConfigId",
                table: "Personnel",
                column: "PersonnelConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_PersonnelOccupationId",
                table: "Personnel",
                column: "PersonnelOccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_PersonnelTypeCD",
                table: "Personnel",
                column: "PersonnelTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_PrefixCD",
                table: "Personnel",
                column: "PrefixCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAcademicPositions_AcademicPositionCD",
                table: "PersonnelAcademicPositions",
                column: "AcademicPositionCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAcademicPositions_AcademicPositionTypeCD",
                table: "PersonnelAcademicPositions",
                column: "AcademicPositionTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAcademicPositions_CreatedById",
                table: "PersonnelAcademicPositions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAcademicPositions_EntityStateCD",
                table: "PersonnelAcademicPositions",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAcademicPositions_PersonnelId",
                table: "PersonnelAcademicPositions",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAddresses_AddressTypeCD",
                table: "PersonnelAddresses",
                column: "AddressTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAddresses_CountryCD",
                table: "PersonnelAddresses",
                column: "CountryCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAddresses_CreatedById",
                table: "PersonnelAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAddresses_EntityStateCD",
                table: "PersonnelAddresses",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAddresses_PersonnelId",
                table: "PersonnelAddresses",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelConfigs_ActiveOrganizationId",
                table: "PersonnelConfigs",
                column: "ActiveOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEncounterRelations_CreatedById",
                table: "PersonnelEncounterRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEncounterRelations_EncounterId",
                table: "PersonnelEncounterRelations",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEncounterRelations_EntityStateCD",
                table: "PersonnelEncounterRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEncounterRelations_PersonnelId",
                table: "PersonnelEncounterRelations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEncounterRelations_RelationTypeCD",
                table: "PersonnelEncounterRelations",
                column: "RelationTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_CreatedById",
                table: "PersonnelIdentifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_EntityStateCD",
                table: "PersonnelIdentifiers",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_IdentifierPoolCD",
                table: "PersonnelIdentifiers",
                column: "IdentifierPoolCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_IdentifierTypeCD",
                table: "PersonnelIdentifiers",
                column: "IdentifierTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_IdentifierUseCD",
                table: "PersonnelIdentifiers",
                column: "IdentifierUseCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelIdentifiers_PersonnelId",
                table: "PersonnelIdentifiers",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_CreatedById",
                table: "PersonnelOccupations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_EntityStateCD",
                table: "PersonnelOccupations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_OccupationCategoryCD",
                table: "PersonnelOccupations",
                column: "OccupationCategoryCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_OccupationCD",
                table: "PersonnelOccupations",
                column: "OccupationCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_OccupationSubCategoryCD",
                table: "PersonnelOccupations",
                column: "OccupationSubCategoryCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_PersonnelId",
                table: "PersonnelOccupations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOccupations_PersonnelSeniorityCD",
                table: "PersonnelOccupations",
                column: "PersonnelSeniorityCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOrganizations_OrganizationId",
                table: "PersonnelOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOrganizations_PersonnelId",
                table: "PersonnelOrganizations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_CreatedById",
                table: "PersonnelPositions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_EntityStateCD",
                table: "PersonnelPositions",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_PersonnelId",
                table: "PersonnelPositions",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_PositionCD",
                table: "PersonnelPositions",
                column: "PositionCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamOrganizationRelations_CreatedById",
                table: "PersonnelTeamOrganizationRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamOrganizationRelations_EntityStateCD",
                table: "PersonnelTeamOrganizationRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamOrganizationRelations_OrganizationId",
                table: "PersonnelTeamOrganizationRelations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamOrganizationRelations_PersonnelTeamId",
                table: "PersonnelTeamOrganizationRelations",
                column: "PersonnelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamOrganizationRelations_RelationTypeCD",
                table: "PersonnelTeamOrganizationRelations",
                column: "RelationTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamRelations_CreatedById",
                table: "PersonnelTeamRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamRelations_EntityStateCD",
                table: "PersonnelTeamRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamRelations_PersonnelId",
                table: "PersonnelTeamRelations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamRelations_PersonnelTeamId",
                table: "PersonnelTeamRelations",
                column: "PersonnelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeamRelations_RelationTypeCD",
                table: "PersonnelTeamRelations",
                column: "RelationTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeams_CreatedById",
                table: "PersonnelTeams",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeams_EntityStateCD",
                table: "PersonnelTeams",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTeams_TypeCD",
                table: "PersonnelTeams",
                column: "TypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_PositionPermissions_CreatedById",
                table: "PositionPermissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PositionPermissions_EntityStateCD",
                table: "PositionPermissions",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PositionPermissions_PermissionModuleId",
                table: "PositionPermissions",
                column: "PermissionModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionPermissions_PositionCD",
                table: "PositionPermissions",
                column: "PositionCD");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocumentRelations_CreatedById",
                table: "ProjectDocumentRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocumentRelations_EntityStateCD",
                table: "ProjectDocumentRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocumentRelations_ProjectId",
                table: "ProjectDocumentRelations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPatientRelations_CreatedById",
                table: "ProjectPatientRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPatientRelations_EntityStateCD",
                table: "ProjectPatientRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPatientRelations_PatientId",
                table: "ProjectPatientRelations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPatientRelations_ProjectId",
                table: "ProjectPatientRelations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPersonnelRelations_CreatedById",
                table: "ProjectPersonnelRelations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPersonnelRelations_EntityStateCD",
                table: "ProjectPersonnelRelations",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPersonnelRelations_PersonnelId",
                table: "ProjectPersonnelRelations",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPersonnelRelations_ProjectId",
                table: "ProjectPersonnelRelations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_EntityStateCD",
                table: "Projects",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeCD",
                table: "Projects",
                column: "ProjectTypeCD");

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

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocuments_CreatedById",
                table: "TaskDocuments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocuments_EntityStateCD",
                table: "TaskDocuments",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocuments_TaskDocumentCD",
                table: "TaskDocuments",
                column: "TaskDocumentCD");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedById",
                table: "Tasks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EncounterId",
                table: "Tasks",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EntityStateCD",
                table: "Tasks",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PatientId",
                table: "Tasks",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskClassCD",
                table: "Tasks",
                column: "TaskClassCD");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskDocumentId",
                table: "Tasks",
                column: "TaskDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskPriorityCD",
                table: "Tasks",
                column: "TaskPriorityCD");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusCD",
                table: "Tasks",
                column: "TaskStatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskTypeCD",
                table: "Tasks",
                column: "TaskTypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusEntries_AdministrativeDataId",
                table: "ThesaurusEntries",
                column: "AdministrativeDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusEntries_CreatedById",
                table: "ThesaurusEntries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusEntries_EntityStateCD",
                table: "ThesaurusEntries",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusEntryTranslations_ThesaurusEntryId",
                table: "ThesaurusEntryTranslations",
                column: "ThesaurusEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusMerges_CreatedById",
                table: "ThesaurusMerges",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusMerges_EntityStateCD",
                table: "ThesaurusMerges",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedById",
                table: "Transactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EncounterId",
                table: "Transactions",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EntityStateCD",
                table: "Transactions",
                column: "EntityStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_HL7MessageLogId",
                table: "Transactions",
                column: "HL7MessageLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PatientId",
                table: "Transactions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourceSystemCD",
                table: "Transactions",
                column: "SourceSystemCD");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDirectionCD",
                table: "Transactions",
                column: "TransactionDirectionCD");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_AdministrativeDataId",
                table: "Versions",
                column: "AdministrativeDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_ChemotherapySchemas_ChemotherapySchemaId",
                table: "ChemotherapySchemaInstances",
                column: "ChemotherapySchemaId",
                principalTable: "ChemotherapySchemas",
                principalColumn: "ChemotherapySchemaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_Codes_EntityStateCD",
                table: "ChemotherapySchemaInstances",
                column: "EntityStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_Personnel_CreatedById",
                table: "ChemotherapySchemaInstances",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_Personnel_CreatorId",
                table: "ChemotherapySchemaInstances",
                column: "CreatorId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstances_SmartOncologyPatients_PatientId",
                table: "ChemotherapySchemaInstances",
                column: "PatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstanceVersions_Codes_EntityStateCD",
                table: "ChemotherapySchemaInstanceVersions",
                column: "EntityStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstanceVersions_Personnel_CreatedById",
                table: "ChemotherapySchemaInstanceVersions",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemaInstanceVersions_Personnel_CreatorId",
                table: "ChemotherapySchemaInstanceVersions",
                column: "CreatorId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemas_Codes_EntityStateCD",
                table: "ChemotherapySchemas",
                column: "EntityStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemas_Personnel_CreatedById",
                table: "ChemotherapySchemas",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemotherapySchemas_Personnel_CreatorId",
                table: "ChemotherapySchemas",
                column: "CreatorId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Codes_ClinicalTrialIdentifierTypeCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialIdentifierTypeCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Codes_ClinicalTrialRecruitmentStatusCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialRecruitmentStatusCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Codes_ClinicalTrialSponsorIdentifierTypeCD",
                table: "ClinicalTrials",
                column: "ClinicalTrialSponsorIdentifierTypeCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Codes_EntityStateCD",
                table: "ClinicalTrials",
                column: "EntityStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Personnel_CreatedById",
                table: "ClinicalTrials",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalTrials_Projects_ProjectId",
                table: "ClinicalTrials",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeAssociations_Codes_ChildId",
                table: "CodeAssociations",
                column: "ChildId",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeAssociations_Codes_EntityStateCD",
                table: "CodeAssociations",
                column: "EntityStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeAssociations_Codes_ParentId",
                table: "CodeAssociations",
                column: "ParentId",
                principalTable: "Codes",
                principalColumn: "CodeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CodeAssociations_Personnel_CreatedById",
                table: "CodeAssociations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_CodeSets_CodeSetId",
                table: "Codes",
                column: "CodeSetId",
                principalTable: "CodeSets",
                principalColumn: "CodeSetId",
                onDelete: ReferentialAction.Cascade,
                onUpdate: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_Personnel_CreatedById",
                table: "Codes",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_ThesaurusEntries_ThesaurusEntryId",
                table: "Codes",
                column: "ThesaurusEntryId",
                principalTable: "ThesaurusEntries",
                principalColumn: "ThesaurusEntryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CodeSets_Personnel_CreatedById",
                table: "CodeSets",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeSets_ThesaurusEntries_ThesaurusEntryId",
                table: "CodeSets",
                column: "ThesaurusEntryId",
                principalTable: "ThesaurusEntries",
                principalColumn: "ThesaurusEntryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Personnel_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communications_Patients_PatientId",
                table: "Communications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communications_SmartOncologyPatients_SmartOncologyPatientId",
                table: "Communications",
                column: "SmartOncologyPatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterIdentifiers_Encounters_EncounterId",
                table: "EncounterIdentifiers",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "EncounterId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterIdentifiers_Personnel_CreatedById",
                table: "EncounterIdentifiers",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encounters_EpisodeOfCares_EpisodeOfCareId",
                table: "Encounters",
                column: "EpisodeOfCareId",
                principalTable: "EpisodeOfCares",
                principalColumn: "EpisodeOfCareId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Encounters_Patients_PatientId",
                table: "Encounters",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Encounters_Personnel_CreatedById",
                table: "Encounters",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_Patients_PatientId",
                table: "EpisodeOfCares",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_PersonnelTeams_PersonnelTeamId",
                table: "EpisodeOfCares",
                column: "PersonnelTeamId",
                principalTable: "PersonnelTeams",
                principalColumn: "PersonnelTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_Personnel_CreatedById",
                table: "EpisodeOfCares",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeOfCares_SmartOncologyPatients_SmartOncologyPatientId",
                table: "EpisodeOfCares",
                column: "SmartOncologyPatientId",
                principalTable: "SmartOncologyPatients",
                principalColumn: "SmartOncologyPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorMessageLogs_HL7MessageLogs_HL7MessageLogId",
                table: "ErrorMessageLogs",
                column: "HL7MessageLogId",
                principalTable: "HL7MessageLogs",
                principalColumn: "HL7MessageLogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorMessageLogs_Personnel_CreatedById",
                table: "ErrorMessageLogs",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormCodeRelations_Personnel_CreatedById",
                table: "FormCodeRelations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusRoles_Personnel_CreatedById",
                table: "GlobalThesaurusRoles",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusUserRoles_GlobalThesaurusUsers_GlobalThesaurusUserId",
                table: "GlobalThesaurusUserRoles",
                column: "GlobalThesaurusUserId",
                principalTable: "GlobalThesaurusUsers",
                principalColumn: "GlobalThesaurusUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusUserRoles_Personnel_CreatedById",
                table: "GlobalThesaurusUserRoles",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusUsers_Personnel_CreatedById",
                table: "GlobalThesaurusUsers",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_HL7MessageLogs_Personnel_CreatedById",
                table: "HL7MessageLogs",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundAliases_OutboundAliases_OutboundAliasId",
                table: "InboundAliases",
                column: "OutboundAliasId",
                principalTable: "OutboundAliases",
                principalColumn: "AliasId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundAliases_Personnel_CreatedById",
                table: "InboundAliases",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationDoseInstances_MedicationInstances_MedicationInstanceId",
                table: "MedicationDoseInstances",
                column: "MedicationInstanceId",
                principalTable: "MedicationInstances",
                principalColumn: "MedicationInstanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationDoses_Medications_MedicationId",
                table: "MedicationDoses",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstances_Medications_MedicationId",
                table: "MedicationInstances",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstances_Personnel_CreatedById",
                table: "MedicationInstances",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationReplacements_Personnel_CreatedById",
                table: "MedicationReplacements",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationReplacements_Personnel_CreatorId",
                table: "MedicationReplacements",
                column: "CreatorId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Personnel_CreatedById",
                table: "Medications",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_O4CodeableConcepts_ThesaurusEntries_ThesaurusEntryId",
                table: "O4CodeableConcepts",
                column: "ThesaurusEntryId",
                principalTable: "ThesaurusEntries",
                principalColumn: "ThesaurusEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAddresses_Personnel_CreatedById",
                table: "OrganizationAddresses",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationClinicalDomains_Organizations_OrganizationId",
                table: "OrganizationClinicalDomains",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationClinicalDomains_Personnel_CreatedById",
                table: "OrganizationClinicalDomains",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationCommunicationEntities_Organizations_OrganizationId",
                table: "OrganizationCommunicationEntities",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationCommunicationEntities_Personnel_CreatedById",
                table: "OrganizationCommunicationEntities",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationIdentifiers_Organizations_OrganizationId",
                table: "OrganizationIdentifiers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationIdentifiers_Personnel_CreatedById",
                table: "OrganizationIdentifiers",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationRelations_Organizations_ChildId",
                table: "OrganizationRelations",
                column: "ChildId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationRelations_Organizations_ParentId",
                table: "OrganizationRelations",
                column: "ParentId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationRelations_Personnel_CreatedById",
                table: "OrganizationRelations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Personnel_CreatedById",
                table: "Organizations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTelecoms_Personnel_CreatedById",
                table: "OrganizationTelecoms",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboundAliases_Personnel_CreatedById",
                table: "OutboundAliases",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutsideUserAddresses_Personnel_CreatedById",
                table: "OutsideUserAddresses",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutsideUsers_Personnel_CreatedById",
                table: "OutsideUsers",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAddresses_Patients_PatientId",
                table: "PatientAddresses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAddresses_Personnel_CreatedById",
                table: "PatientAddresses",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientContactAddresses_PatientContacts_PatientContactId",
                table: "PatientContactAddresses",
                column: "PatientContactId",
                principalTable: "PatientContacts",
                principalColumn: "PatientContactId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientContactAddresses_Personnel_CreatedById",
                table: "PatientContactAddresses",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientContacts_Patients_PatientId",
                table: "PatientContacts",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientContacts_Personnel_CreatedById",
                table: "PatientContacts",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientContactTelecoms_Personnel_CreatedById",
                table: "PatientContactTelecoms",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientIdentifiers_Patients_PatientId",
                table: "PatientIdentifiers",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientIdentifiers_Personnel_CreatedById",
                table: "PatientIdentifiers",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPatientRelations_PatientLists_PatientListId",
                table: "PatientListPatientRelations",
                column: "PatientListId",
                principalTable: "PatientLists",
                principalColumn: "PatientListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPatientRelations_Patients_PatientId",
                table: "PatientListPatientRelations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPatientRelations_Personnel_CreatedById",
                table: "PatientListPatientRelations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPersonnelRelations_PatientLists_PatientListId",
                table: "PatientListPersonnelRelations",
                column: "PatientListId",
                principalTable: "PatientLists",
                principalColumn: "PatientListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPersonnelRelations_Personnel_CreatedById",
                table: "PatientListPersonnelRelations",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientListPersonnelRelations_Personnel_PersonnelId",
                table: "PatientListPersonnelRelations",
                column: "PersonnelId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLists_PersonnelTeams_PersonnelTeamId",
                table: "PatientLists",
                column: "PersonnelTeamId",
                principalTable: "PersonnelTeams",
                principalColumn: "PersonnelTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLists_Personnel_AttendingDoctorId",
                table: "PatientLists",
                column: "AttendingDoctorId",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLists_Personnel_CreatedById",
                table: "PatientLists",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Personnel_CreatedById",
                table: "Patients",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTelecoms_Personnel_CreatedById",
                table: "PatientTelecoms",
                column: "CreatedById",
                principalTable: "Personnel",
                principalColumn: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_PersonnelOccupations_PersonnelOccupationId",
                table: "Personnel",
                column: "PersonnelOccupationId",
                principalTable: "PersonnelOccupations",
                principalColumn: "PersonnelOccupationId");

            int deletedStateCode = (int)EntityStateCode.Deleted;

            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [dbo].[PersonnelPositionPermissionViews]
                AS 
                SELECT
                    NEWID() AS Id,
                    perPosition.PersonnelPositionId,
                    perPosition.PersonnelId,
                    m.ModuleId,
                    m.Name AS ModuleName,
                    p.PermissionId,
                    p.Name AS PermissionName,
                    posPermission.PositionCD
                FROM dbo.PermissionModules pM
                INNER JOIN dbo.Modules m ON m.ModuleId = pM.ModuleId 
                INNER JOIN dbo.Permissions p ON p.PermissionId = pM.PermissionId
                INNER JOIN dbo.PositionPermissions posPermission ON posPermission.PermissionModuleId = pM.PermissionModuleId
                INNER JOIN dbo.PersonnelPositions perPosition ON perPosition.PositionCD = posPermission.PositionCD
                INNER JOIN dbo.Codes code ON code.CodeId = posPermission.PositionCD
                WHERE GETDATE() BETWEEN perPosition.[ActiveFrom] AND perPosition.[ActiveTo]
                  AND GETDATE() BETWEEN posPermission.[ActiveFrom] AND posPermission.[ActiveTo]
                  AND GETDATE() BETWEEN code.[ActiveFrom] AND code.[ActiveTo];
            ");

            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [dbo].[EncounterViews]
                AS
                SELECT
                    encounters.EncounterId,
                    patients.NameGiven,
                    patients.NameFamily,
                    patients.GenderCD,
                    patients.BirthDate,
                    patients.PatientId,
                    encounters.TypeCD,
                    encounters.StatusCD,
                    encounters.AdmissionDate,
                    encounters.DischargeDate,
                    encounters.EpisodeOfCareId,
                    episodeOfCare.TypeCD AS EpisodeOfCareTypeCD,
                    encounters.EntityStateCD,
                    encounters.[RowVersion],
                    encounters.[EntryDatetime],
                    encounters.[LastUpdate],
                    encounters.[ActiveFrom],
                    encounters.[ActiveTo],
                    encounters.[CreatedById]
                FROM dbo.Encounters encounters
                LEFT JOIN dbo.Patients patients ON encounters.PatientId = patients.PatientId
                LEFT JOIN dbo.EpisodeOfCares episodeOfCare ON encounters.EpisodeOfCareId = episodeOfCare.EpisodeOfCareId
                WHERE GETDATE() BETWEEN encounters.[ActiveFrom] AND encounters.[ActiveTo]
                  AND (encounters.EntityStateCD != {deletedStateCode} OR encounters.EntityStateCD IS NULL)
                  AND GETDATE() BETWEEN patients.[ActiveFrom] AND patients.[ActiveTo]
                  AND (patients.EntityStateCD != {deletedStateCode} OR patients.EntityStateCD IS NULL)
                  AND GETDATE() BETWEEN episodeOfCare.[ActiveFrom] AND episodeOfCare.[ActiveTo]
                  AND (episodeOfCare.EntityStateCD != {deletedStateCode} OR episodeOfCare.EntityStateCD IS NULL);
            ");

            migrationBuilder.Sql($@"CREATE OR ALTER VIEW dbo.CodeAliasViews
                AS 
                SELECT
                    inboundAliases.[AliasId],
                    inboundAliases.[CodeId],
		            codes.[CodeSetId],
                    inboundAliases.[System],
                    inboundAliases.[Alias] as InboundAlias,
                    outboundAliases.[Alias] as OutboundAlias,
                    inboundAliases.[EntityStateCD],
                    inboundAliases.[RowVersion],
                    inboundAliases.[EntryDatetime],
                    inboundAliases.[LastUpdate],
                    inboundAliases.[ActiveFrom],
                    inboundAliases.[ActiveTo],
                    inboundAliases.[CreatedById],
                    inboundAliases.[AliasId] as InboundAliasId,
                    outboundAliases.[AliasId] as OutboundAliasId
                FROM dbo.[InboundAliases] inboundAliases
                LEFT JOIN dbo.[OutboundAliases] outboundAliases
                    ON outboundAliases.[System] = inboundAliases.[System] 
                    AND outboundAliases.[CodeId] = inboundAliases.[CodeId]  AND outboundAliases.[AliasId] = inboundAliases.[OutboundAliasId]
                    AND GETDATE() between outboundAliases.[ActiveFrom] and outboundAliases.[ActiveTo]
	            INNER JOIN dbo.[Codes] codes on codes.CodeId = inboundAliases.CodeId
                WHERE GETDATE() BETWEEN inboundAliases.[ActiveFrom] AND inboundAliases.[ActiveTo]
            ");

            migrationBuilder.Sql($@"CREATE or alter  VIEW [dbo].[PersonnelViews]
				AS
				select
					personnel.[PersonnelId]
					,personnel.[Username]
					,personnel.[FirstName]
					,personnel.[LastName]
					,personnel.[Email]
					,personnel.[EntityStateCD]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,personnelOrg.[OrganizationId]
					,personnelOrg.[StateCD]
					,personnel.[ActiveFrom]
					,personnel.[ActiveTo]
					,personnel.[CreatedById]
					,personnel.[DayOfBirth]
					,personnel.[PersonnelTypeCD]
					,(SELECT cast(personnelPositionInner.PositionCD as varchar)
							FROM dbo.PersonnelPositions personnelPositionInner
							WHERE personnel.PersonnelId = personnelPositionInner.PersonnelId
							and GETDATE() BETWEEN personnelPositionInner.[ActiveFrom] AND personnelPositionInner.[ActiveTo]
							group by personnelPositionInner.PositionCD
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelPositionIds
					,STUFF((SELECT ', ' + thTran.PreferredTerm
						FROM dbo.PersonnelPositions personnelPositionInner
						inner join dbo.Codes code on code.CodeId = personnelPositionInner.PositionCD
						inner join dbo.ThesaurusEntryTranslations thTran on thTran.ThesaurusEntryId = code.ThesaurusEntryId
						WHERE personnel.PersonnelId = personnelPositionInner.PersonnelId
						and GETDATE() BETWEEN personnelPositionInner.[ActiveFrom] AND personnelPositionInner.[ActiveTo]
						and thTran.Language = '{LanguageConstants.EN}'
						group by thTran.PreferredTerm 
						order by thTran.PreferredTerm
						FOR XML PATH('')), 1, 1, '') as PersonnelPositions
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.personnelId = personnelOrg.PersonnelId
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {deletedStateCode}))	
							and GETDATE() BETWEEN org.[ActiveFrom] AND org.[ActiveTo] 
							order by org.Name
							FOR XML PATH('')), 1, 1, '') as PersonnelOrganizations
					,(SELECT cast(org.OrganizationId as varchar) 
							FROM dbo.Organizations org
							INNER JOIN dbo.PersonnelOrganizations personnelOrg 
							ON personnelOrg.OrganizationId = org.OrganizationId
							WHERE personnel.PersonnelId = personnelOrg.PersonnelId
							and (org.EntityStateCD is null or (org.EntityStateCD is not null and org.EntityStateCD != {deletedStateCode}))
							and GETDATE() BETWEEN org.[ActiveFrom] AND org.[ActiveTo] 
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelOrganizationIds
					,(SELECT cast(personnelIdentifier.IdentifierTypeCD as varchar) + '{Delimiters.ComplexSegmentDelimiter}' + personnelIdentifier.IdentifierValue
							FROM dbo.PersonnelIdentifiers personnelIdentifier
							WHERE personnel.PersonnelId = personnelIdentifier.PersonnelId
							and GETDATE() BETWEEN personnelIdentifier.[ActiveFrom] AND personnelIdentifier.[ActiveTo]
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelIdentifiers
					, (SELECT 
							(case when personnelAddress.CountryCD is null then '' else cast(personnelAddress.CountryCD as varchar) end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.City is null then '' else personnelAddress.City end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.Street is null then '' else personnelAddress.Street end),
							'{Delimiters.ComplexSegmentDelimiter}',
							(case when personnelAddress.PostalCode is null then '' else personnelAddress.PostalCode end)
							FROM dbo.PersonnelAddresses personnelAddress
							WHERE personnel.PersonnelId = personnelAddress.PersonnelId
							and GETDATE() BETWEEN personnelAddress.[ActiveFrom] AND personnelAddress.[ActiveTo]
							FOR XML PATH('{Delimiters.ComplexColumnDelimiter}')) as PersonnelAddresses
					from dbo.Personnel personnel
					left join dbo.[PersonnelPositions] personnelPosition
					on personnelPosition.PersonnelId = personnel.PersonnelId
					left join dbo.[PersonnelOrganizations] personnelOrg
					on personnelOrg.PersonnelId = personnel.PersonnelId
					left join dbo.[Organizations] org
					on personnelOrg.OrganizationId = org.OrganizationId
					group by personnel.[PersonnelId]
					,personnel.[Username]
					,personnel.[FirstName]
					,personnel.[LastName]
					,personnel.[Email]
					,personnel.[EntityStateCD]
					,personnel.[RowVersion]
					,personnel.[EntryDatetime]
					,personnel.[LastUpdate]
					,personnelOrg.[OrganizationId]
					,personnelOrg.[StateCD]
					,personnel.[ActiveFrom]
					,personnel.[ActiveTo]
					,personnel.[CreatedById]
					,personnel.[DayOfBirth]
			,personnel.[PersonnelTypeCD]
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeSets_Codes_EntityStateCD",
                table: "CodeSets");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddresses_Codes_AddressTypeCD",
                table: "OrganizationAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddresses_Codes_CountryCD",
                table: "OrganizationAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddresses_Codes_EntityStateCD",
                table: "OrganizationAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationRelations_Codes_EntityStateCD",
                table: "OrganizationRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Codes_EntityStateCD",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_Codes_EntityStateCD",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_Codes_PersonnelTypeCD",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_Codes_PrefixCD",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Codes_EntityStateCD",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Codes_OccupationCD",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Codes_OccupationCategoryCD",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Codes_OccupationSubCategoryCD",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Codes_PersonnelSeniorityCD",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_ThesaurusEntries_Codes_EntityStateCD",
                table: "ThesaurusEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddresses_Personnel_CreatedById",
                table: "OrganizationAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationRelations_Personnel_CreatedById",
                table: "OrganizationRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Personnel_CreatedById",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Personnel_CreatedById",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOccupations_Personnel_PersonnelId",
                table: "PersonnelOccupations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationRelations_Organizations_ChildId",
                table: "OrganizationRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationRelations_Organizations_ParentId",
                table: "OrganizationRelations");

            migrationBuilder.DropTable(
                name: "ApiRequestLogs");

            migrationBuilder.DropTable(
                name: "BodySurfaceCalculationFormulas");

            migrationBuilder.DropTable(
                name: "ChemotherapySchemaInstanceVersions");

            migrationBuilder.DropTable(
                name: "ClinicalDomains");

            migrationBuilder.DropTable(
                name: "ClinicalTrials");

            migrationBuilder.DropTable(
                name: "CodeAssociations");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Communications");

            migrationBuilder.DropTable(
                name: "EncounterIdentifiers");

            migrationBuilder.DropTable(
                name: "EpisodeOfCareWorkflows");

            migrationBuilder.DropTable(
                name: "ErrorMessageLogs");

            migrationBuilder.DropTable(
                name: "FormCodeRelations");

            migrationBuilder.DropTable(
                name: "GlobalThesaurusUserRoles");

            migrationBuilder.DropTable(
                name: "InboundAliases");

            migrationBuilder.DropTable(
                name: "Indications");

            migrationBuilder.DropTable(
                name: "LiteratureReferences");

            migrationBuilder.DropTable(
                name: "MedicationDoseTimeInstances");

            migrationBuilder.DropTable(
                name: "MedicationDoseTimes");

            migrationBuilder.DropTable(
                name: "MedicationDoseTypes");

            migrationBuilder.DropTable(
                name: "MedicationReplacements");

            migrationBuilder.DropTable(
                name: "O4CodeableConcepts");

            migrationBuilder.DropTable(
                name: "OrganizationClinicalDomains");

            migrationBuilder.DropTable(
                name: "OrganizationCommunicationEntities");

            migrationBuilder.DropTable(
                name: "OrganizationIdentifiers");

            migrationBuilder.DropTable(
                name: "OrganizationTelecoms");

            migrationBuilder.DropTable(
                name: "OutsideUsers");

            migrationBuilder.DropTable(
                name: "PatientAddresses");

            migrationBuilder.DropTable(
                name: "PatientContactAddresses");

            migrationBuilder.DropTable(
                name: "PatientContactTelecoms");

            migrationBuilder.DropTable(
                name: "PatientIdentifiers");

            migrationBuilder.DropTable(
                name: "PatientListPatientRelations");

            migrationBuilder.DropTable(
                name: "PatientListPersonnelRelations");

            migrationBuilder.DropTable(
                name: "PatientTelecoms");

            migrationBuilder.DropTable(
                name: "PersonnelAcademicPositions");

            migrationBuilder.DropTable(
                name: "PersonnelAddresses");

            migrationBuilder.DropTable(
                name: "PersonnelEncounterRelations");

            migrationBuilder.DropTable(
                name: "PersonnelIdentifiers");

            migrationBuilder.DropTable(
                name: "PersonnelOrganizations");

            migrationBuilder.DropTable(
                name: "PersonnelPositions");

            migrationBuilder.DropTable(
                name: "PersonnelTeamOrganizationRelations");

            migrationBuilder.DropTable(
                name: "PersonnelTeamRelations");

            migrationBuilder.DropTable(
                name: "PositionPermissions");

            migrationBuilder.DropTable(
                name: "ProjectDocumentRelations");

            migrationBuilder.DropTable(
                name: "ProjectPatientRelations");

            migrationBuilder.DropTable(
                name: "ProjectPersonnelRelations");

            migrationBuilder.DropTable(
                name: "RouteOfAdministrations");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "ThesaurusEntryTranslations");

            migrationBuilder.DropTable(
                name: "ThesaurusMerges");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "GlobalThesaurusRoles");

            migrationBuilder.DropTable(
                name: "GlobalThesaurusUsers");

            migrationBuilder.DropTable(
                name: "OutboundAliases");

            migrationBuilder.DropTable(
                name: "MedicationDoseInstances");

            migrationBuilder.DropTable(
                name: "MedicationDoses");

            migrationBuilder.DropTable(
                name: "CodeSystems");

            migrationBuilder.DropTable(
                name: "OutsideUserAddresses");

            migrationBuilder.DropTable(
                name: "PatientContacts");

            migrationBuilder.DropTable(
                name: "PatientLists");

            migrationBuilder.DropTable(
                name: "PermissionModules");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "TaskDocuments");

            migrationBuilder.DropTable(
                name: "Encounters");

            migrationBuilder.DropTable(
                name: "HL7MessageLogs");

            migrationBuilder.DropTable(
                name: "MedicationInstances");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "EpisodeOfCares");

            migrationBuilder.DropTable(
                name: "ChemotherapySchemaInstances");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "PersonnelTeams");

            migrationBuilder.DropTable(
                name: "SmartOncologyPatients");

            migrationBuilder.DropTable(
                name: "ChemotherapySchemas");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "MultipleBirths");

            migrationBuilder.DropTable(
                name: "Codes");

            migrationBuilder.DropTable(
                name: "CodeSets");

            migrationBuilder.DropTable(
                name: "ThesaurusEntries");

            migrationBuilder.DropTable(
                name: "AdministrativeDatas");

            migrationBuilder.DropTable(
                name: "Personnel");

            migrationBuilder.DropTable(
                name: "PersonnelConfigs");

            migrationBuilder.DropTable(
                name: "PersonnelOccupations");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "OrganizationAddresses");

            migrationBuilder.DropTable(
                name: "OrganizationRelations");

            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[PersonnelPositionPermissionViews];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[EncounterViews];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[CodeAliasViews];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[PersonnelViews];");
        }
    }
}
