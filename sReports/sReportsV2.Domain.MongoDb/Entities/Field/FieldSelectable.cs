using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class FieldSelectable : Field
    {
        public List<FormFieldValue> Values { get; set; } = new List<FormFieldValue>();
        public List<FormFieldDependable> Dependables { get; set; } = new List<FormFieldDependable>();

        public override bool IsDistributiveField() => true;

        public override string GetDistributiveSelectedOptionId(string distibutedValue)
        {
            return Values.Find(v => v.Value == distibutedValue)?.Id;
        }

        #region Thesaurus Methods

        public override List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (FormFieldValue value in Values)
            {
                var fieldValuehesaurusId = value.ThesaurusId;
                thesaurusList.Add(fieldValuehesaurusId);
            }

            return thesaurusList;
        }

        public override void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormFieldValue value in Values)
            {
                value.Label = entries.Find(x => x.ThesaurusEntryId.Equals(value.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
            }
        }

        public override void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusId = this.ThesaurusId.ReplaceThesaurus(thesaurusMerge);

            foreach (FormFieldValue value in this.Values)
            {
                value.ReplaceThesauruses(thesaurusMerge);
            }
        }

        #endregion /Thesaurus Methods
    }
}
