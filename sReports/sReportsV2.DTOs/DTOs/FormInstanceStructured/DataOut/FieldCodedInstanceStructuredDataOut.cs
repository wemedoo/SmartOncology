using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldCodedDataOut
    {
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            string valueLabel = GetValueLabel();
            string valueLabelOrValue = !string.IsNullOrWhiteSpace(valueLabel) ? valueLabel : GetValue();
            return valueLabelOrValue ?? string.Empty;
        }

    }
}
