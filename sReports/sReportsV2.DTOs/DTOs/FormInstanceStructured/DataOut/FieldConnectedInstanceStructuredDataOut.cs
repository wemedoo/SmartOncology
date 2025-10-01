using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstanceStructured.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldConnectedDataOut
    {
        public List<FieldConnectedOptionDataOut> ConnectedFieldDataSource { get; set; }
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            string valueLabel = GetValueLabel();
            string valueLabelOrValue = !string.IsNullOrWhiteSpace(valueLabel) ? valueLabel : GetValue();
            return valueLabelOrValue ?? string.Empty;
        }

    }
}
