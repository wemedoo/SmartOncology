using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FormInstance
{
    [BsonIgnoreExtraElements]
    public class FieldInstance
    {
        public List<FieldInstanceValue> FieldInstanceValues { get; set; }
        public int ThesaurusId { get; set; }
        public string Type { get; set; }
        public string FieldSetInstanceRepetitionId { get; set; } 
        public string FieldSetId { get; set; }
        public string FieldId { get; set; }

        [BsonIgnore]
        public int FieldSetCounter { get; set; }

        //IMPORTANT! Value, ValueLabel, InstanceId can be deleted when M_202308231355_FieldInstanceValue_Property is executed
        [BsonIgnoreIfNull]
        public List<string> Value { get; set; }
        [BsonIgnoreIfNull]
        public string InstanceId { get; set; }
        [BsonIgnoreIfNull]
        public List<string> ValueLabel { get; set; }
        

        #region Constructors

        public FieldInstance()
        {
        }

        public FieldInstance(Field field)
        {
            this.FieldId = field.Id;
            this.ThesaurusId = field.ThesaurusId;
            this.FieldInstanceValues = field.FieldInstanceValues.GetFieldInstanceValuesOrInitial();
            this.Type = field.Type;
        }

        /// <summary>
        /// Constuctor for form instance values that are not generated from UI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldSetId"></param>
        public FieldInstance(Field field, string fieldSetId, string fieldSetInstanceRepetitionId) : this(field)
        {
            this.FieldSetId = fieldSetId;
            this.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
        }

        /// <summary>
        /// Constuctor for form instance values that are not generated from UI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldSetId"></param>
        /// <param name="fieldSetInstanceRepetitionId"></param>
        /// <param name="fieldInstanceValue"></param>
        public FieldInstance(Field field, string fieldSetId, string fieldSetInstanceRepetitionId, FieldInstanceValue fieldInstanceValue) : this(field, fieldSetId, fieldSetInstanceRepetitionId)
        {
            if (fieldInstanceValue != null)
            {
                this.FieldInstanceValues.Add(fieldInstanceValue);
            }
        }

        #endregion /Constructors

        public void AddValue(string value)
        {
            if (!(string.IsNullOrWhiteSpace(value) || value.All(c => c.Equals(','))))
            {
                FieldInstanceValues = FieldInstanceValues.GetFieldInstanceValuesOrInitial();
                FieldInstanceValues.Add(new FieldInstanceValue(value));
            }
        }

        public void AddValue(FieldInstanceValue fieldInstanceValue)
        {
            if (fieldInstanceValue != null && !string.IsNullOrWhiteSpace(fieldInstanceValue.FieldInstanceRepetitionId))
            {
                FieldInstanceValues = FieldInstanceValues.GetFieldInstanceValuesOrInitial();
                FieldInstanceValues.Add(fieldInstanceValue);
            }
        }
    } 
}
