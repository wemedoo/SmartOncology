using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.MongoDb.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.FormInstance.DataIn;
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
        public string ConnectedFieldInstanceRepetitionId { get; set; }
        public FieldValidationError ValidationError { get; set; }

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
            ConnectedFieldInstanceRepetitionId = fieldInstanceValue.ConnectedFieldInstanceRepetitionId;
        }

        public FieldInstanceDTO(Domain.Entities.FormInstance.FieldInstance fieldInstance, FieldInstanceValue fieldInstanceRepetition)
        {
            FieldSetId = fieldInstance.FieldSetId;
            FieldSetInstanceRepetitionId = fieldInstance.FieldSetInstanceRepetitionId;
            FieldId = fieldInstance.FieldId;
            ThesaurusId = fieldInstance.ThesaurusId;
            Type = fieldInstance.Type;
            FieldInstanceRepetitionId = fieldInstanceRepetition.FieldInstanceRepetitionId;
            ValidationError = fieldInstanceRepetition.ValidationError;
            IsSpecialValue = fieldInstanceRepetition.IsSpecialValue;
            FlatValueLabel = fieldInstanceRepetition.ValueLabel;
            FlatValues = fieldInstanceRepetition.Values;
            ConnectedFieldInstanceRepetitionId = fieldInstanceRepetition.ConnectedFieldInstanceRepetitionId;
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
                        if (DateTimeOffset.TryParse(value, out DateTimeOffset dateTime) && value.HasOffset())
                        {
                            processedValue = dateTime.ConvertFormInstanceDateTimeToOrganizationTimeZone();
                        }
                        else
                        {
                            string organizationOffsetPrinted = DateTimeExtension.GetOffsetValue();
                            processedValue += organizationOffsetPrinted;
                        }

                        FlatValueLabel = processedValue;
                    }
                    processedValues.Add(processedValue);
                }
            }

            return processedValues;
        }

        public void UpdateCachedData(FieldInstanceDTO incomingFieldInstance)
        {
            this.FlatValueLabel = incomingFieldInstance.FlatValueLabel;
            this.FlatValues = incomingFieldInstance.FlatValues;
            this.ValidationError = incomingFieldInstance.ValidationError;
            this.IsSpecialValue = incomingFieldInstance.IsSpecialValue;
        }
    }
}
