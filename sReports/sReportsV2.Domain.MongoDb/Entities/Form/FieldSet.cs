using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using MongoDB.Bson;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FieldSet
    {
        public O4CodeableConcept Code { get; set; } 
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set;}
        public List<Field> Fields { get; set; } = new List<Field>();
        public LayoutStyle LayoutStyle { get; set; }
        public Help Help { get; set; }
        public string MapAreaId { get; set; }
        public bool IsBold { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        [BsonIgnore]
        public string FieldSetInstanceRepetitionId { get; set; }
        public List<FormFieldValue> Options { get; set; } = new List<FormFieldValue>();

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (Field field in Fields)
            {
                var fieldhesaurusId = field.ThesaurusId;
                thesaurusList.Add(fieldhesaurusId);
                thesaurusList.AddRange(field.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (Field field in Fields)
            {
                field.Label = entries.Find(x => x.ThesaurusEntryId.Equals(field.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                field.Description = entries.Find(x => x.ThesaurusEntryId.Equals(field.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                field.GenerateTranslation(entries, language, activeLanguage);
            }
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
                        if (field.ThesaurusId == targetField.ThesaurusId && targetField.Type == field.Type) 
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

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (Field field in this.Fields)
            {
                field.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

        public void SetFieldSetInstanceRepetitionIds(string fieldSetInstanceRepetitionId)
        {
            this.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
            this.Fields.ForEach(f => { 
                f.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
                f.FieldSetId = this.Id;
            });
        }

        public Field GetFieldById(string fieldId)
        {
            return Fields.Find(x => x.Id == fieldId);
        }
    }
}
