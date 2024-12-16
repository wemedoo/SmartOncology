using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public class CustomDataColumn : DataColumn
    {
        public int RepetitiveFieldCounter { get; set; } = 0;
        public string FieldId { get; set; }
        public int RepetitiveFieldSetCounter { get; set; } = 0;
        public string FieldSetId { get; set; }

        public string ColumnLabel { get; set; }

        public static string CreateRepetitiveFieldName(string fieldSetId, int fieldSetRepetition, string fieldId, int fieldRepetition)
        {
            return fieldSetId + "_" + fieldSetRepetition + "_" + fieldId + "_" + fieldRepetition;
        }

        public static string CreateNonRepetitiveFieldName(string fieldId, string fieldLabel)
        {
            return fieldId + "_" + fieldLabel;
        }
    }

}
