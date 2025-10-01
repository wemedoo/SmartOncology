using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.MongoDb.Entities.Base;
using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public partial class FieldSet : IFormThesaurusEntity
    {
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
        public string MatrixId { get; set; }
        public MatrixType? MatrixType { get; set; }
        public List<FormFieldValue> Options { get; set; } = new List<FormFieldValue>();
        public List<FieldSet> ListOfFieldSets { get; set; } = new List<FieldSet>();

        #region Thesaurus Methods
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

        public void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusId = this.ThesaurusId.ReplaceThesaurus(thesaurusMerge);
            foreach (Field field in this.Fields)
            {
                field.ReplaceThesauruses(thesaurusMerge);
            }
        }
        #endregion /Thesaurus Methods

        public Field GetFieldById(string fieldId)
        {
            return Fields.Find(x => x.Id == fieldId);
        }
    }
}
