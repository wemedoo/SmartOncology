using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldDateDataOut
    {
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue.RenderDate();
        }
    }
}
