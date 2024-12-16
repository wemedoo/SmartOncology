using sReportsV2.Common.Configurations;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FieldInstanceDTO
    {
        public string FieldInstanceRepetitionId { get; set; }
        public string FieldSetInstanceRepetitionId { get; set; }
        public string FieldSetId { get; set; }
        public string FieldId { get; set; }
        public List<string> FlatValues { get; set; } = new List<string>();
        public string FlatValueLabel { get; set; }
        public string Type { get; set; }
        public int ThesaurusId { get; set; }
        public bool IsSpecialValue { get; set; }

        public FieldInstanceDTO()
        {
        }

        public FieldInstanceDTO(FieldDataOut field, FieldInstanceValueDataOut fieldInstanceValue)
        {
            FieldSetId = field.FieldSetId;
            FieldId = field.Id;
            ThesaurusId = field.ThesaurusId;
            Type = field.Type;
            FieldSetInstanceRepetitionId = field.FieldSetInstanceRepetitionId;
            FieldInstanceRepetitionId = fieldInstanceValue.FieldInstanceRepetitionId;
            IsSpecialValue = fieldInstanceValue.IsSpecialValue;
            FlatValueLabel = fieldInstanceValue.ValueLabel;
            FlatValues = fieldInstanceValue.Values;
        }


        public List<string> GetCleanedValue()
        {
            List<string> processedValues = new List<string>();

            foreach (string value in FlatValues)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string processedValue = value;
                    if (this.Type == FieldTypes.Datetime && !IsSpecialValue)
                    {
                        if (DateTimeOffset.TryParse(value, out DateTimeOffset dateTime) && HasOffset(value))
                            processedValue = dateTime.ConvertFormInstanceDateTimeToOrganizationTimeZone();
                        else
                            processedValue += GlobalConfig.GetUserOffset(isOffsetForFormInstance: true);

                        FlatValueLabel = processedValue;
                    }
                    processedValues.Add(processedValue);
                }
            }

            return processedValues;
        }

        private bool HasOffset(string dateTimeString)
        {
            int tIndex = dateTimeString.IndexOf('T');
            if (tIndex == -1) return false;

            string timePart = dateTimeString.Substring(tIndex + 1);

            return timePart.Contains("+") || timePart.Contains("-") || timePart.EndsWith('Z');
        }
    }
}
