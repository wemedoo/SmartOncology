using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FieldInstanceValue
    {
        public List<string> Values { get; set; }
        [BsonIgnoreIfNull]
        public string Value { get; set; } // Should be no longer used. Will be removed after M_202402141255_FieldInstanceValue_Modification is successfully executed
        public string ValueLabel { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
        public bool IsSpecialValue { get; set; }

        public FieldInstanceValue(string value)
        {
            Values = new List<string>();
            if (!string.IsNullOrEmpty(value))
            {
                Values.Add(value);
            }
            ValueLabel = value;
            FieldInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
        }

        /// <summary>
        /// Constructor used for selected field instance values
        /// </summary>
        /// <param name="values"></param>
        /// <param name="valueLabel"></param>
        public FieldInstanceValue(List<string> values, string valueLabel)
        {
            Values = values;
            ValueLabel = valueLabel;
            FieldInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
        }

        public FieldInstanceValue(List<string> values, string valueLabel, string fieldInstanceRepetitionId, bool isSpecialValue)
        {
            Values = values;
            ValueLabel = valueLabel;
            FieldInstanceRepetitionId = !string.IsNullOrWhiteSpace(fieldInstanceRepetitionId) ? fieldInstanceRepetitionId : GuidExtension.NewGuidStringWithoutDashes();
            IsSpecialValue = isSpecialValue;
        }

        public string GetValueLabelOrValue()
        {
            return ValueLabel ?? Values?.FirstOrDefault() ?? string.Empty;
        }

        public bool HasAnyValue()
        {
            return Values != null && Values.Count > 0;
        }

        public string GetFirstValue()
        {
            return Values?.FirstOrDefault();
        }
    }
}
