using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity.Custom;
using sReportsV2.Domain.Entities.Form;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Dependency;

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
        typeof(FieldParagraph),
        typeof(FieldLink),
        typeof(FieldAudio))]
    public class Field
    {
        public string Performer { get; set; }
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
        public CustomFHIRClasses.O4CodeableConcept Code { get; set; }
        public O4ResourceReference Subject { get; set; }
        public List<O4ResourceReference> Result { get; set; }
        public bool? AllowSaveWithoutValue { get; set; }
        public virtual List<int> NullFlavors { get; set; } = new List<int>();
        public DependentOnInfo DependentOn {  get; set; }
        [BsonIgnore]
        public virtual string Type { get; set; }
        [BsonIgnore]
        public string FieldSetInstanceRepetitionId { get; set; }
        [BsonIgnore]
        public string FieldSetId { get; set; }
        [BsonIgnore]
        public List<FieldInstanceValue> FieldInstanceValues { get; set; }

        public virtual bool IsFieldRepetitive() => false;
        public virtual bool IsDistributiveField() => false;

        #region virtual methods


        public virtual List<int> GetAllThesaurusIds()
        {
            return new List<int>();
        }

        public virtual void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {            
        }

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

        public virtual void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
        }

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

        public virtual string GetDistributiveSelectedOptionId(string distibutedValue)
        {
            return string.Empty;
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
            if (FieldInstanceValues != null) {
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

        public string GenerateDependentSuffix(Dictionary<string, Field> dictionaryFields)
        {
            string dependebleSuffix = string.Empty;

            if (this.DependentOn != null)
            {
                dependebleSuffix = this.DependentOn.FormatFormula(dictionaryFields);
            }

            return dependebleSuffix;
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
    }
}
