using ExcelImporter.Classes;
using ExcelImporter.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace ExcelImporter.Importers
{
    public class ExtendedPropertiesImporter : ExcelSaxImporter<string>
    {
        private readonly IAdministrativeDataDAL administrativeDataDAL;

        public ExtendedPropertiesImporter(IAdministrativeDataDAL administrativeDataDAL, string fileName, string sheetName) : base(fileName, sheetName)
        {
            this.administrativeDataDAL = administrativeDataDAL;
        }
        public override void ImportDataFromExcelToDatabase()
        {
            List<string> executeExtendedPropertiesCommands = ImportFromExcel();
            InsertDataIntoDatabase(executeExtendedPropertiesCommands);
        }

        protected override List<string> ImportFromExcel()
        {
            List<RowInfo> dataRows = ImportRowsFromExcel();
            List<string> commands = PrepareCommandsForExecution(dataRows);
            return commands;
        }

        protected override void InsertDataIntoDatabase(List<string> executeExtendedPropertiesCommands)
        {
            administrativeDataDAL.ExecuteCustomSqlCommand(string.Join(" ", executeExtendedPropertiesCommands));
        }

        private List<string> PrepareCommandsForExecution(List<RowInfo> dataRows)
        {
            List<string> commands = new List<string>();
            foreach (RowInfo dataRow in dataRows)
            {
                string tableName = dataRow.GetCellValue(GetColumnAddress(ExtendedPropertiesConstants.TableName));
                string columnName = dataRow.GetCellValue(GetColumnAddress(ExtendedPropertiesConstants.ColumnName));
                string description = dataRow.GetCellValue(GetColumnAddress(ExtendedPropertiesConstants.Description));
                if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName))
                {
                    commands.Add(MigrationHelper.AddExtendedProperty(tableName, description.EscapeSqlString(), columnName));
                }
            }
            return commands;
        }
    }
}
