using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class FieldNumeric
    {
        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueNumber;
        }

        public override FieldInstanceValue CreateDistributedFieldInstanceValue(List<string> enteredValues)
        {
            double? roundedValue = RoundNumericValue(enteredValues.FirstOrDefault());
            return roundedValue.HasValue ? new FieldInstanceValue(roundedValue.Value.ToString()) : null;
        }

        private double? RoundNumericValue(string enteredValue)
        {
            double step = this.Step ?? 0.0001;
            int decimalsNumber = NumericHelper.GetDecimalsNumber(step);
            double? numbericValueRounded = null;
            if (double.TryParse(enteredValue, out double numericValue))
            {
                numbericValueRounded = Math.Round(numericValue, decimalsNumber);
            }
            return numbericValueRounded;
        }
    }
}
