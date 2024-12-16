using sReportsV2.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace sReportsV2.Common.Extensions
{
    public static class DataTableExtension
    {
        public static string ToCsvString(this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            List<string> columnNames = dt.Columns.GetCleanedColumnNames();
            columnNames = columnNames.Select(x => x.SanitizeForCsvExport()).ToList();

            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString().SanitizeForCsvExport());
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }

        public static List<string> GetCleanedColumnNames(this DataColumnCollection columns)
        {
            List<string> columnNames = columns.Cast<DataColumn>().
                                  Select(column => column.ColumnName).ToList();

            for (int i = 0; i < columnNames.Count; i++)
            {
                string id = columnNames[i].SplitAndTake('_', " ", 1);

                if (columnNames.Exists(x => x.Contains(id + "_1"))) // searches for FieldSet Repetitions 
                    columnNames[i] = columnNames[i].FormatCsvHeaderCell(fieldSetInstanceRepetition: true);
                else
                    columnNames[i] = columnNames[i].FormatCsvHeaderCell(fieldSetInstanceRepetition: false);
            }

            return columnNames;
        }

        public static List<string> GetCleanCustomColumnNames(this List<CustomDataColumn> columns, bool repetitiveHeaders = false)
        {
            List<string> result = new List<string>();

            if (repetitiveHeaders)
            {
                foreach (CustomDataColumn column in columns)
                {
                    if (!string.IsNullOrEmpty(column.ColumnLabel))
                        result.Add(column.ColumnLabel + " " + column.RepetitiveFieldSetCounter + "." + column.RepetitiveFieldCounter);
                    else
                        result.Add(column.ColumnName);
                }

            }
            else
            {
                result = columns.Select(x =>  x.ColumnLabel ?? x.ColumnName).ToList();
            }

            return result;
        }

    }
}
