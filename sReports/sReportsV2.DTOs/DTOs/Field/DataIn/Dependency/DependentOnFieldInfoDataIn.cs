using sReportsV2.Common.Constants;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class DependentOnFieldInfoDataIn
    {
        public string FieldId { get; set; }
        public string FieldType { get; set; }
        public string FieldValueId { get; set; }
        public string Variable { get; set; }

        public bool VariableAssignedToOption()
        {
            return !string.IsNullOrEmpty(FieldValueId);
        }

        public bool IsNumeric()
        {
            return FieldType == FieldTypes.Number || FieldType == FieldTypes.Calculative;
        }
    }
}
