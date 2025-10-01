using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class Field
    {
        [BsonIgnore]
        public string FieldSetInstanceRepetitionId { get; set; }
        [BsonIgnore]
        public string FieldSetId { get; set; }
        [BsonIgnore]
        public List<FieldInstanceValue> FieldInstanceValues { get; set; }

        #region Referrable Logic

        public string GetReferrableValue(Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            string result = string.Empty;

            if (this.FieldInstanceValues.HasAnyFieldInstanceValue())
            {
                IEnumerable<string> referrableValues = this.FieldInstanceValues
                    .Where(fiV => fiV.HasAnyValue())
                    .Select(fiV => this.GetDisplayValue(fiV, missingValuesDict));
                result = string.Join(ResourceTypes.HTML_BR, referrableValues);
            }

            return result;
        }

        public bool IsReferrable(Field field)
        {
            return this.ThesaurusId == field.ThesaurusId && this.Type == field.Type;
        }

        public void SetReferral(Field referralField)
        {
            if (referralField != null && referralField.FieldInstanceValues.HasAnyFieldInstanceValue())
            {
                List<string> fieldInstanceRepetitionIds = this.SetMissingFieldInstanceRepetitionIds(referralField);
                this.FieldInstanceValues = referralField.FieldInstanceValues;
                this.UpdateFieldInstanceRepetitionIds(fieldInstanceRepetitionIds);
            }
        }

        private List<string> SetMissingFieldInstanceRepetitionIds(Field referralField)
        {
            List<string> fieldInstanceRepetitionIds = this
                    .FieldInstanceValues
                    .GetFieldInstanceValuesOrInitial()
                    .Select(fiv => fiv.FieldInstanceRepetitionId)
                    .ToList();
            for (int i = fieldInstanceRepetitionIds.Count; i < referralField.FieldInstanceValues.Count; i++)
            {
                fieldInstanceRepetitionIds.Add(GuidExtension.NewGuidStringWithoutDashes());
            }

            return fieldInstanceRepetitionIds;
        }

        private void UpdateFieldInstanceRepetitionIds(List<string> fieldInstanceRepetitionIds)
        {
            for (int i = 0; i < this.FieldInstanceValues.Count; i++)
            {
                this.FieldInstanceValues[i].FieldInstanceRepetitionId = fieldInstanceRepetitionIds[i];
            }
        }

        #endregion /Referrable Logic

        #region virtual methods

        public virtual bool HasValue()
        {
            return !string.IsNullOrWhiteSpace(GetFirstFieldInstanceValue())
             && !string.IsNullOrWhiteSpace(FieldInstanceValues.GetFirstValue().Replace(",", " "));
        }

        public virtual string GetFirstFieldInstanceValue()
        {
            return FieldInstanceValues.HasAnyFieldInstanceValue() ? FieldInstanceValues.GetFirstValue() ?? string.Empty : string.Empty;
        }

        public string GetTextValueForOomniaApi(FieldInstanceValue fieldInstanceValue)
        {
            return fieldInstanceValue.IsSpecialValue ? "-2147483645" : GetSimpleValueForOomniaApi(fieldInstanceValue.GetFirstValue());
        }

        public virtual string GetSimpleValueForOomniaApi(string enteredValue)
        {
            return enteredValue;
        }

        public virtual IList<string> GetSelectedValuesForOomniaApi(List<string> enteredValues, IDictionary<int, ThesaurusEntry> thesaurusesFromFormDefinition, int? oomniaCodeSystemId)
        {
            return new List<string>();
        }

        public virtual FieldInstanceValue CreateDistributedFieldInstanceValue(List<string> enteredValues)
        {
            return null;
        }

        protected virtual string FormatPatholinkValue(string selectedOptionId)
        {
            return this.FieldInstanceValues.FirstOrDefault()?.GetFirstValue();
        }

        protected virtual int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.NullFlavor;
        }

        private string GetCodeMissingValue(string codeIdValue, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            int.TryParse(codeIdValue, out int codeId);
            return missingValues
                        .Where(x => x.Key == GetMissingValueCodeSetId())
                        .SelectMany(c => c.Value)
                        .Where(v => v.Key == codeId)
                        .Select(v => v.Value)
                        .FirstOrDefault();
        }

        protected virtual string GetDisplayValue(FieldInstanceValue fieldInstanceValue)
        {
            return fieldInstanceValue.GetValueLabelOrValue();
        }
        #endregion

        public string GetDisplayValue(FieldInstanceValue fieldInstanceValue, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            string displayValue = string.Empty;
            if (fieldInstanceValue != null)
            {
                if (fieldInstanceValue.IsSpecialValue)
                {
                    displayValue = GetCodeMissingValue(fieldInstanceValue.GetFirstValue(), missingValues);
                }
                else
                {
                    displayValue = GetDisplayValue(fieldInstanceValue);
                }
            }
            return displayValue;
        }

        public string GetValueForPatholinkExport(Dictionary<int, Dictionary<int, string>> missingValues, string selectedOptionId = "")
        {
            string patholinkValue = string.Empty;
            if (FieldInstanceValues != null)
            {
                foreach (FieldInstanceValue fieldInstanceValue in FieldInstanceValues)
                {
                    if (fieldInstanceValue.IsSpecialValue)
                    {
                        patholinkValue = GetCodeMissingValue(fieldInstanceValue.GetFirstValue(), missingValues);
                    }
                    else
                    {
                        patholinkValue = FormatPatholinkValue(selectedOptionId);
                    }
                    break; //Patholink does not handle repetitive field instances
                }
            }

            return patholinkValue;
        }
    }
}
