using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldSelectableDataOut
    {
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            IEnumerable<string> checkedLabels = this.Values.Where(formFieldValue => fieldInstanceValue.Values.Contains(formFieldValue.Id)).Select(formFieldValue => formFieldValue.Label);
            return checkedLabels.Any() ? string.Join(valueSeparator, checkedLabels) : string.Empty;
        }
    }
}
