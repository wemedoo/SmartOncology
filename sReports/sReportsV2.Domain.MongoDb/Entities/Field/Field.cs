using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Dependency;
using sReportsV2.Domain.Entities.FieldEntity.Custom;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.MongoDb.Entities.Base;
using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
        typeof(FieldText),
        typeof(FieldRadio),
        typeof(FieldCheckbox),
        typeof(FieldEmail),
        typeof(FieldDate),
        typeof(FieldCalculative),
        typeof(FieldRegex),
        typeof(FieldNumeric),
        typeof(FieldFile),
        typeof(FieldSelect),
        typeof(FieldTextArea),
        typeof(FieldDatetime),
        typeof(CustomFieldButton),
        typeof(FieldCoded),
        typeof(FieldConnected),
        typeof(FieldParagraph),
        typeof(FieldLink),
        typeof(FieldAudio),
        typeof(FieldRichTextParagraph))]
    public partial class Field : IFormThesaurusEntity
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsRequired { get; set; }
        public bool IsBold { get; set; }
        public bool IsHiddenOnPdf { get; set; }
        public Help Help { get; set; }
        public bool? AllowSaveWithoutValue { get; set; }
        public virtual List<int> NullFlavors { get; set; } = new List<int>();
        public DependentOnInfo DependentOn {  get; set; }
        [BsonIgnore]
        public virtual string Type { get; set; }

        public virtual bool IsFieldRepetitive() => false;
        public virtual bool IsDistributiveField() => false;

        #region virtual methods

        #region Thesaurus Methods
        public virtual List<int> GetAllThesaurusIds()
        {
            return new List<int>();
        }

        public virtual void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
        }

        public virtual void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusId = this.ThesaurusId.ReplaceThesaurus(thesaurusMerge);
        }

        #endregion /Thesaurus Methods

        public virtual string GetDistributiveSelectedOptionId(string distibutedValue)
        {
            return string.Empty;
        }
        #endregion

        public string GenerateDependentSuffix(Dictionary<string, Field> dictionaryFields)
        {
            string dependebleSuffix = string.Empty;

            if (this.DependentOn != null)
            {
                dependebleSuffix = this.DependentOn.FormatFormula(dictionaryFields);
            }

            return dependebleSuffix;
        }
    }
}
