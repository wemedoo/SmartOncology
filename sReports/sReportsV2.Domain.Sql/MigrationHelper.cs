using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Linq;
using System;

namespace sReportsV2.Domain.Sql
{
    public static class MigrationHelper
    {
        /// <summary>
        /// Adds system versioning to the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void AddSystemVersioningToTables(MigrationBuilder migrationBuilder, string tableName)
        {
            string addStartDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ADD StartDateTime DATETIME2;";
            string addEndDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ADD EndDateTime DATETIME2;";

            string updateStartDateTimeEndDataTime = $@"
                UPDATE {tableName} SET StartDateTime = '19000101 00:00:00.0000000', EndDateTime = '99991231 23:59:59.9999999';";

            string alterStartDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ALTER COLUMN StartDateTime DATETIME2 NOT NULL;";
            string alterEndDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ALTER COLUMN EndDateTime DATETIME2 NOT NULL;";

            string addPeriodForSystemTime = $@"
                ALTER TABLE {tableName}
                ADD PERIOD FOR SYSTEM_TIME (StartDateTime, EndDateTime);";
            string historyTableName = tableName + "History";
            string addSystemVersioningToTheTable = $@"
                ALTER TABLE {tableName}
                SET(SYSTEM_VERSIONING = ON (HISTORY_TABLE = {historyTableName}, DATA_CONSISTENCY_CHECK = ON));";

            migrationBuilder.Sql(addStartDateTimeColumn);
            migrationBuilder.Sql(addEndDateTimeColumn);
            migrationBuilder.Sql(updateStartDateTimeEndDataTime);
            migrationBuilder.Sql(alterStartDateTimeColumn);
            migrationBuilder.Sql(alterEndDateTimeColumn);
            migrationBuilder.Sql(addPeriodForSystemTime);
            migrationBuilder.Sql(addSystemVersioningToTheTable);
        }

        /// <summary>
        /// Removes system versioning from the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void UnsetSystemVersionedTables(MigrationBuilder migrationBuilder, string tableName)
        {
            string removeSystemVersioningFromTheTable = $@"
                ALTER TABLE {tableName} SET ( SYSTEM_VERSIONING = OFF);";
            string dropPeriodForSystemTime = $@"
                ALTER TABLE {tableName} DROP PERIOD FOR SYSTEM_TIME;";

            string historyTableName = tableName + "History";
            string dropHistoryTable = $@"
                DROP TABLE IF EXISTS {historyTableName};";

            string dropStartDateTime = $@"
                ALTER TABLE {tableName} DROP COLUMN StartDateTime;";
            string dropEndDateTime = $@"
                ALTER TABLE {tableName} DROP COLUMN EndDateTime;";

            migrationBuilder.Sql(removeSystemVersioningFromTheTable);
            migrationBuilder.Sql(dropPeriodForSystemTime);
            migrationBuilder.Sql(dropHistoryTable);
            migrationBuilder.Sql(dropStartDateTime);
            migrationBuilder.Sql(dropEndDateTime);
        }

        /// <summary>
        /// Creates indexes on common properties for the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void CreateIndexesOnCommonProperties(MigrationBuilder migrationBuilder, string tableName)
        {
            string createLastUpdateIndex = $@"
                CREATE INDEX IX_LastUpdate
                ON {tableName} (LastUpdate);";

            string createStartDateTimeIndex = $@"
                CREATE INDEX IX_StartDateTime
                ON {tableName} (StartDateTime);";

            string createEndDateTimeIndex = $@"
                CREATE INDEX IX_EndDateTime
                ON {tableName} (EndDateTime);";

            migrationBuilder.Sql(createLastUpdateIndex);
            migrationBuilder.Sql(createStartDateTimeIndex);
            migrationBuilder.Sql(createEndDateTimeIndex);
        }

        /// <summary>
        /// Drops indexes on common properties for the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void DropIndexesOnCommonProperties(MigrationBuilder migrationBuilder, string tableName)
        {
            string dropLastUpdateIndex = $@"
                DROP INDEX IX_LastUpdate ON {tableName};";

            string dropStartDateTimeIndex = $@"
                DROP INDEX IX_StartDateTime ON {tableName};";

            string dropEndDateTimeIndex = $@"
                DROP INDEX IX_EndDateTime ON {tableName};";

            migrationBuilder.Sql(dropLastUpdateIndex);
            migrationBuilder.Sql(dropStartDateTimeIndex);
            migrationBuilder.Sql(dropEndDateTimeIndex);
        }

        /// <summary>
        /// Creates a temporary table and saves data from the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void CreateCountryTempTableAndSaveData(MigrationBuilder migrationBuilder, string tableName)
        {
            string createTempTable = $@"
                CREATE TABLE dbo.Country{tableName}TempTable (
                CustomEnumId INT,
                AddressId INT
            );";
            string saveDataInTempTable = $@"
                INSERT INTO dbo.Country{tableName}TempTable (CustomEnumId, AddressId) 
                SELECT customEnum.Id AS CustomEnumId, 
                       a.Id AS AddressId
                FROM dbo.{tableName} a
                INNER JOIN dbo.ThesaurusEntryTranslations translation 
                ON translation.PreferredTerm = a.Country
                INNER JOIN dbo.CustomEnums customEnum 
                ON customEnum.ThesaurusEntryId = translation.ThesaurusEntryId
                WHERE customEnum.Type = 10
                GROUP BY customEnum.Id, a.Id;";

            migrationBuilder.Sql(createTempTable);
            migrationBuilder.Sql(saveDataInTempTable);
        }

        /// <summary>
        /// Updates the main table with data from the temporary table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void SetCountryDataFromTempTable(MigrationBuilder migrationBuilder, string tableName)
        {
            string updateAddressTable = $@"
                UPDATE a
                SET a.CountryId = temp.CustomEnumId
                FROM dbo.Country{tableName}TempTable temp
                INNER JOIN dbo.{tableName} a ON a.Id = temp.AddressId;";
            string dropTempTable = $@"
                DROP TABLE IF EXISTS dbo.Country{tableName}TempTable;";

            migrationBuilder.Sql(updateAddressTable);
            migrationBuilder.Sql(dropTempTable);
        }

        /// <summary>
        /// Renames embedded name columns in the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void RemoveEmbeddedNameEntity(MigrationBuilder migrationBuilder, string tableName)
        {
            string renameEmbeddedNameGiven = $@"
                IF EXISTS(SELECT 1 FROM sys.columns WHERE [name] = N'Name_Given'
                   AND [object_id] = OBJECT_ID(N'{tableName}'))
                BEGIN
                    EXEC sp_RENAME '{tableName}.Name_Given', 'NameGiven';
                END;";
            string renameEmbeddedNameFamily = $@"
                IF EXISTS(SELECT 1 FROM sys.columns WHERE [name] = N'Name_Family'
                   AND [object_id] = OBJECT_ID(N'{tableName}'))
                BEGIN
                    EXEC sp_RENAME '{tableName}.Name_Family', 'NameFamily';
                END;";
            migrationBuilder.Sql(renameEmbeddedNameGiven);
            migrationBuilder.Sql(renameEmbeddedNameFamily);
        }

        /// <summary>
        /// Reverts renamed embedded name columns in the specified table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="tableName">The name of the table to modify.</param>
        public static void RevertEmbeddedNameEntity(MigrationBuilder migrationBuilder, string tableName)
        {
            string revertNameGiven = $@"
                EXEC sp_rename '{tableName}.NameGiven', 'Name_Given';";
            string revertNameFamily = $@"
                EXEC sp_rename '{tableName}.NameFamily', 'Name_Family';";

            migrationBuilder.Sql(revertNameGiven);
            migrationBuilder.Sql(revertNameFamily);
        }

        /// <summary>
        /// Adds an extended property to a table or column.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="description">The description for the extended property.</param>
        /// <param name="propertyName">The name of the column for the extended property. Optional.</param>
        /// <returns>The SQL command to add the extended property.</returns>
        public static string AddExtendedProperty(string tableName, string description, string propertyName = null)
        {
            string propertyNameFormatted = string.IsNullOrEmpty(propertyName) ? "default" : $"'{propertyName}'";

            return $@"EXEC AddExtendedProperty @tableName = '{tableName}', @description = '{description}', @columnName = {propertyNameFormatted};";
        }

        /// <summary>
        /// Renames foreign keys in the database.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <param name="FKsToBeRenamed">The list of foreign key pairs to be renamed.</param>
        /// <param name="isUpMigration">Indicates whether the operation is an up migration or down migration.</param>
        public static void ExecuteRenameFKs(MigrationBuilder migrationBuilder, List<Tuple<string, string>> FKsToBeRenamed, bool isUpMigration)
        {
            string renameFKsBatch = PrepareBatchCommand(FKsToBeRenamed, isUpMigration);
            migrationBuilder.Sql(renameFKsBatch);
        }

        private static string PrepareBatchCommand(List<Tuple<string, string>> FKsToBeRenamed, bool isUpMigration)
        {
            return string.Join(" ", FKsToBeRenamed.Select(x => PrepareSingleCommand(x, isUpMigration)));
        }

        private static string PrepareSingleCommand(Tuple<string, string> FKToBeRenamed, bool isUpMigration)
        {
            string oldFK = FKToBeRenamed.Item1;
            string newFK = FKToBeRenamed.Item2;
            if (isUpMigration)
            {
                return $@"EXEC sp_rename '[{oldFK}]', '{newFK}';";
            }
            else
            {
                return $@"EXEC sp_rename '[{newFK}]', '{oldFK}';";
            }
        }
    }
}
