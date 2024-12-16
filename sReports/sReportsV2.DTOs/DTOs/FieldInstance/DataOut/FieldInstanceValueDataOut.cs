using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataOut
{
    public class FieldInstanceValueDataOut
    {
        public List<string> Values { get; set; }
        public string ValueLabel { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
        public bool IsSpecialValue { get; set; }

        public FieldInstanceValueDataOut()
        {
            Values = new List<string>();
            FieldInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
        }

        [JsonIgnore]
        public string FirstValue
        {
            get
            {
                return Values?.FirstOrDefault();
            }
        }

        public FieldInstanceValueDataOut(string value)
        {
            Values = new List<string>();
            if (!string.IsNullOrEmpty(value))
            {
                Values.Add(value);
            }
            ValueLabel = value;
            FieldInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
        }

        public void ResetValue(bool shouldSetSpecialValue, int? missingCodeValueId)
        {
            this.ValueLabel = string.Empty;
            if (shouldSetSpecialValue && missingCodeValueId.HasValue)
            {
                this.Values = new List<string> { missingCodeValueId.Value.ToString() };
                this.IsSpecialValue = true;
            }
            else
            {
                this.Values = new List<string>();
                this.IsSpecialValue = false;
            }
        }
    }
}
