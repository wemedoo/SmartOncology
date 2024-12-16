using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Number)]
    public class FieldNumeric : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Number;
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Step { get; set; }

        public override bool IsDistributiveField() => true;

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
