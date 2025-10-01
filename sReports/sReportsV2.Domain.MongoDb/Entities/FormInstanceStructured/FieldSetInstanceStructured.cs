using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;

namespace sReportsV2.Domain.Entities.Form
{
    public partial class FieldSet
    {
        [BsonIgnore]
        public string FieldSetInstanceRepetitionId { get; set; }

        public void SetFieldSetInstanceRepetitionIds(string fieldSetInstanceRepetitionId)
        {
            this.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
            this.Fields.ForEach(f => {
                f.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
                f.FieldSetId = this.Id;
            });
        }

        public bool IsReferable(FieldSet targetFieldSet)
        {
            Ensure.IsNotNull(targetFieldSet, nameof(FieldSet));

            bool result = false;
            int matchedFieldCounter = 0;
            if (this.ThesaurusId == targetFieldSet.ThesaurusId && this.Fields.Count == targetFieldSet.Fields.Count)
            {
                foreach (Field field in this.Fields)
                {
                    foreach (Field targetField in this.Fields)
                    {
                        if (targetField.IsReferrable(field))
                        {
                            matchedFieldCounter++;
                            break;
                        }
                    }
                }

                if (matchedFieldCounter == this.Fields.Count)
                {
                    result = true;
                }
            }

            return result;
        }

        public void SetReferralFields(FieldSet referralFieldSet)
        {
            foreach (Field targetField in this.Fields)
            {
                Field referralField = referralFieldSet.Fields.Find(f => f.IsReferrable(targetField));
                if (referralField != null && referralField.FieldInstanceValues.HasAnyFieldInstanceValue())
                {
                    targetField.SetReferral(referralField);
                }
            }
        }
    }
}
