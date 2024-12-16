using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public class FieldSelectable : Field
    {
        public List<FormFieldValue> Values { get; set; } = new List<FormFieldValue>();
        public List<FormFieldDependable> Dependables { get; set; } = new List<FormFieldDependable>();

        public FieldInstanceValue CreateFieldInstanceValue(List<string> selectedOptionsIds)
        {
            List<string> values = new List<string>();
            List<string> valueLabels = new List<string>();

            foreach (FormFieldValue value in Values.Where(formFieldValue => selectedOptionsIds.Contains(formFieldValue.Id)))
            {
                values.Add(value.Id);
                valueLabels.Add(value.Label);
            }

            return new FieldInstanceValue(values, string.Join(",", valueLabels));
        }

        public override FieldInstanceValue CreateDistributedFieldInstanceValue(List<string> enteredValues)
        {
            return CreateFieldInstanceValue(
                    Values
                        .Where(v => enteredValues
                            .Contains(v.ThesaurusId.ToString())    
                        )
                        .Select(x => x.Id)
                        .ToList()
                );
        }

        public override string GetDistributiveSelectedOptionId(string distibutedValue)
        {
            return Values.Find(v => v.Value == distibutedValue)?.Id;
        }

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

        public override void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;

            foreach (FormFieldValue value in this.Values)
            {
                value.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

        protected override string FormatPatholinkValue(string selectedOptionId)
        {
            return this.FieldInstanceValues.FirstOrDefault().GetAllValues().Contains(selectedOptionId) ? "true" : string.Empty;
        }

        public override string GetSimpleValueForOomniaApi(string enteredValue)
        {
            return string.Empty;
        }

        public override IList<string> GetSelectedValuesForOomniaApi(List<string> enteredValues, IDictionary<int, ThesaurusEntry> thesaurusesFromFormDefinition, int? oomniaCodeSystemId)
        {
            IList<string> selectedValues = base.GetSelectedValuesForOomniaApi(enteredValues, thesaurusesFromFormDefinition, oomniaCodeSystemId);
            IEnumerable<int> selectedThesaurusIds = enteredValues
                    .Select(enteredValue =>
                              this.Values.Find(x => x.Id == enteredValue)
                    )
                    .Where(v => v != null)
                    .Select(fV => fV.ThesaurusId)
                    ;

            foreach (int selectedThesaurusId in selectedThesaurusIds)
            {
                this.AddSelectedOomniaCodeName(selectedValues, selectedThesaurusId, thesaurusesFromFormDefinition, oomniaCodeSystemId);
            }
            return selectedValues;
        }

        protected void AddSelectedOomniaCodeName(IList<string> selectedValues, int selectedThesaurusId, IDictionary<int, ThesaurusEntry> thesaurusesFromFormDefinition, int? oomniaCodeSystemId)
        {
            if (thesaurusesFromFormDefinition.TryGetValue(selectedThesaurusId, out ThesaurusEntry thesaurus))
            {
                O4CodeableConcept codeEntity = thesaurus.GetCodeByCodeSystem(oomniaCodeSystemId);
                if (codeEntity != null)
                {
                    selectedValues.Add(codeEntity.Code);
                }
            }
        }
    }
}
