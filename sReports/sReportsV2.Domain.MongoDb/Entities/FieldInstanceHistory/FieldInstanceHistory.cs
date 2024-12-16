using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FieldInstanceHistory
{
    [BsonIgnoreExtraElements]
    public class FieldInstanceHistory : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormInstanceId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldSetLabel { get; set; }
        public int Revision { get; set; }
        public bool IsSpecialValue { get; set; }
        public List<string> Values { get; set; }
        [BsonIgnoreIfNull]
        public string Value { get; set; } // Should be no longer used. Will be removed after M_202402141255_FieldInstanceValue_Modification is successfully executed
        public string ValueLabel { get; set; }
        public int UserId { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }

        public string FieldSetInstanceRepetitionId { get; set; }
        public string FieldSetId { get; set; }
        public string FieldId { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
        public string Type { get; set; }

        public FieldInstanceHistory() { }

        public FieldInstanceHistory(FormInstance.FormInstance formInstance, string fieldLabel, string fieldSetLabel, FieldInstance fieldInstance, FieldInstanceValue repetitiveFieldInstanceValue)
        {
            FormInstanceId = formInstance.Id;
            FieldLabel = fieldLabel;
            FieldSetLabel = fieldSetLabel;
            FieldSetId = fieldInstance.FieldSetId;
            FieldSetInstanceRepetitionId = fieldInstance.FieldSetInstanceRepetitionId;
            FieldId = fieldInstance.FieldId;
            Type = fieldInstance.Type;
            FieldInstanceRepetitionId = repetitiveFieldInstanceValue.FieldInstanceRepetitionId;
            Values = repetitiveFieldInstanceValue.Values;
            ValueLabel = repetitiveFieldInstanceValue.ValueLabel;
            IsSpecialValue = repetitiveFieldInstanceValue.IsSpecialValue;
            UserId = formInstance.GetUserIdWhoMadeAction();
            Revision = 1;
            EntryDatetime = DateTime.Now;
            ActiveFrom = DateTime.Now;
        }

        public void Copy(FieldInstanceHistory fieldInstanceHistoryToCopy)
        {
            if(fieldInstanceHistoryToCopy != null)
            {
                this.Id = fieldInstanceHistoryToCopy.Id;
                this.FormInstanceId = fieldInstanceHistoryToCopy.FormInstanceId;
                this.FieldLabel = fieldInstanceHistoryToCopy.FieldLabel;
                this.FieldSetLabel = fieldInstanceHistoryToCopy.FieldSetLabel;
                this.FieldSetId = fieldInstanceHistoryToCopy.FieldSetId;
                this.FieldSetInstanceRepetitionId = fieldInstanceHistoryToCopy.FieldSetInstanceRepetitionId;
                this.FieldId = fieldInstanceHistoryToCopy.FieldId;
                this.FieldInstanceRepetitionId = fieldInstanceHistoryToCopy.FieldInstanceRepetitionId;
                this.IsSpecialValue = fieldInstanceHistoryToCopy.IsSpecialValue;
                this.Values = fieldInstanceHistoryToCopy.Values;
                this.ValueLabel = fieldInstanceHistoryToCopy.ValueLabel;
                this.Type = fieldInstanceHistoryToCopy.Type;
                base.Copy(fieldInstanceHistoryToCopy);
            }
        }

        public bool LastValueChanged(List<string> newValues)
        {
            bool returnVal = false;

            if (Values != null && Values.Count > 0)
            {
                if (newValues.Count == 0 || !ValuesAreSame(newValues))
                    returnVal = true;
            }
            else
            {
                if (newValues.Count > 0)
                    returnVal = true;
            }
            return returnVal;
        }

        public FieldInstanceHistory CreateNewHistoryLog(int userId, bool isDeleted = false, List<string> values = null, string valueLabel = null, bool isSpecialValue = false)
        {
            return new FieldInstanceHistory()
            {
                FormInstanceId = this.FormInstanceId,
                FieldLabel = this.FieldLabel,
                FieldSetLabel = this.FieldSetLabel,
                FieldSetInstanceRepetitionId = this.FieldSetInstanceRepetitionId,
                FieldSetId = this.FieldSetId,
                FieldId = this.FieldId,
                FieldInstanceRepetitionId = this.FieldInstanceRepetitionId,
                Type = this.Type,
                Values = values,
                IsSpecialValue = isSpecialValue,
                ValueLabel = valueLabel,
                UserId = userId,
                Revision = this.Revision + 1,
                IsDeleted = isDeleted,
                EntryDatetime = DateTime.Now,
                ActiveFrom = DateTime.Now
            };
        }

        private bool ValuesAreSame(List<string> newValues)
        {
            return Values.Count == newValues.Count && Values.TrueForAll(newValues.Contains);
        }
    }
}
