using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldDatetimeDataOut
    {
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            string value = fieldInstanceValue.FirstValue;
            return !string.IsNullOrEmpty(value) ? $"{value.RenderDate()} {value.RenderTime()}" : string.Empty;
        }
    }
}
