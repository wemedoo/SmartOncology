using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Common.Constants;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDataOut
    {
        public virtual string NestableView { get; }
        public virtual string PartialView { get; }
        [DataProp]
        public string FhirType { get; set; }
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Type { get; set; }
        [DataProp]
        public string Label { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public string Unit { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public bool IsVisible { get; set; } = true;
        [DataProp]
        public bool IsReadonly { get; set; }
        [DataProp]
        public bool IsRequired { get; set; } = false;
        [DataProp]
        public bool IsBold { get; set; }
        [DataProp]
        public FormHelpDataOut Help { get; set; }
        [DataProp]
        public bool IsHiddenOnPdf { get; set; }
        [DataProp]
        public bool? AllowSaveWithoutValue { get; set; }
        [DataProp]
        public List<int> NullFlavors { get; set; }
        [DataProp]
        public DependentOnInfoDataOut DependentOn { get; set; }

        public bool IsDisabled { get; set; }
        public string FieldSetInstanceRepetitionId { get; set; }
        public string FieldSetId { get; set; }
        public List<FieldInstanceValueDataOut> FieldInstanceValues { get; set; }
        public bool HiddenFieldsShown { get; set; }
        public virtual bool IsFieldRepetitive => false;

        public virtual string GetValue()
        {
            return GetFirstFieldInstanceValue();
        }
        public virtual bool IsSpecialValue()
        {
            return HasAnyFieldInstanceValue && (FieldInstanceValues.FirstOrDefault()?.IsSpecialValue ?? false);
        }
        public virtual string GetValueLabel()
        {
            return HasAnyFieldInstanceValue ? FieldInstanceValues.FirstOrDefault()?.ValueLabel ?? string.Empty : string.Empty;
        }
        public virtual string GetSpecialValueLabel(Dictionary<string, string> specialValues)
        {
            return specialValues.FirstOrDefault(x => x.Key == FieldInstanceValues.FirstOrDefault()?.FirstValue).Value;
        }
        public virtual string GetLabel()
        {
            return this.Label;
        }
        public virtual Tuple<bool, bool, int> GetRepetitiveInfo()
        {
            bool possibleRepetitiveField = false;
            bool isRepetitive = false;
            int numberOfRepetition = 1;
            return new Tuple<bool, bool, int>(possibleRepetitiveField, isRepetitive, numberOfRepetition);
        }

        public virtual bool ShowCodeMissingValue()
        {
            return IsRequired && AllowSaveWithoutValue.HasValue && AllowSaveWithoutValue.Value;
        }

        public virtual bool IfSpecialValueCanBeSet()
        {
            return IsRequired && AllowSaveWithoutValue.HasValue;
        }

        #region HTML Helper Attributes
        /// <summary> Label + * if required </summary>
        [JsonIgnore]
        public string FullLabel
        {
            get
            {
                string retVal = IsBold ? $"<b>{Label}</b>" : Label;
                if (!string.IsNullOrEmpty(Unit))
                    retVal += " (" + Unit + ")";
                if (IsRequired)
                    retVal += " * ";
                //if (!string.IsNullOrEmpty(ThesaurusId))
                //    retVal += " <a target='_blank' href='/ThesaurusEntry/EditByO4MtId?id=" + ThesaurusId + "' title='Thesaurus ID: " + ThesaurusId + "' class='metat-link' ><i class='far fa-question-circle'></i></a> ";
                return retVal;
                //https://uts.nlm.nih.gov/metathesaurus.html?cui=C0238463
                //http://vocabularies.unesco.org/thesaurus/
            }
        }

        [JsonIgnore]
        public virtual string DescriptionLabel
        {
            get
            {
                return string.IsNullOrEmpty(Description) ? "Enter: " + Label : Description;
            }
        }

        [JsonIgnore]
        public virtual string ValidationAttr
        {
            get
            {
                return "";
            }
        }

        [JsonIgnore]
        public virtual string PopulateAdditionalAttr
        {
            get
            {
                return "";
            }
        }

        [JsonIgnore]
        public string Visibility
        {
            get
            {
                string retVal = "";
                if (!IsVisible)
                    retVal = " style='display: none; ' ";
                return retVal;
            }
        }

        [JsonIgnore]
        public string DependentAttributes
        {
            get
            {
                return string.Format("data-dependables=\"{0}\" {1}", IsVisible, IsVisible ? "" : "disabled");
            }
        }

        [JsonIgnore]
        public string ShowHiddenFieldsClass
        {
            get
            {
                return !IsVisible && HiddenFieldsShown ? "show-hidden-fields" : !IsVisible && !HiddenFieldsShown ? "d-none" : string.Empty;
            }
        }
        #endregion HTML Helper Attributes

        public bool AcceptsSpecialValue
        {
            get
            {
                return Type != FieldTypes.CustomButton;
            }
        }

        public virtual bool CanBeInDependencyFormula()
        {
            return false;
        }

        public string GetFieldInstanceDataAttrs(string fieldSetId, int fieldInstanceRepetitionIndex)
        {
            string fieldInstanceRepetitionId = this.GetFieldInstanceRepetitionInfo(fieldInstanceRepetitionIndex - 1).Item1;

            StringBuilder stringBuilder = new StringBuilder($"name=\"{fieldInstanceRepetitionId}\" ")
                .AppendLine($"data-fieldtype=\"{this.Type}\" ")
                .AppendLine($"data-thesaurusid=\"{this.ThesaurusId}\" ")
                .AppendLine($"data-fieldsetid=\"{fieldSetId}\" ")
                .AppendLine($"data-fieldsetinstancerepetitionid=\"{this.FieldSetInstanceRepetitionId}\" ")
                .AppendLine($"data-fieldid=\"{this.Id}\" ")
                .AppendLine($"data-fieldinstancerepetitionid=\"{fieldInstanceRepetitionId}\" ")
                .AppendLine($"data-isrequired=\"{IsRequired}\" ")
                .AppendLine($"data-allowsavewithoutValue=\"{this.AllowSaveWithoutValue}\" ");

            if (NullFlavors != null && NullFlavors.Any())
            {
                string nullFlavorsString = string.Join(",", NullFlavors);
                stringBuilder.AppendLine($"data-nullflavors=\"{nullFlavorsString}\" ");
            }

            return stringBuilder.ToString();
        }

        [JsonIgnore]
        public string FirstValue
        {
            get
            {
                return FieldInstanceValues?.FirstOrDefault()?.FirstValue;
            }
        }

        [JsonIgnore]
        private bool HasAnyFieldInstanceValue
        {
            get
            {
                return FieldInstanceValues != null && FieldInstanceValues.Count > 0;
            }
        }

        public int GetRepetitiveFieldCount()
        {
            return FieldInstanceValues != null ? FieldInstanceValues.Count : 0;
        }

        public string GetFirstFieldInstanceValue()
        {
            return this.HasAnyFieldInstanceValue ? this.FirstValue ?? string.Empty : string.Empty;
        }

        public List<string> GetFirstFieldInstanceValues()
        {
            return this.HasAnyFieldInstanceValue ? this.FieldInstanceValues.FirstOrDefault()?.Values ?? new List<string>() : new List<string>();
        }

        public bool HasValue()
        {
            return HasAnyFieldInstanceValue;
        }

        public string GetSynopticValue(int repetitiveValueIndex, string neTranslated, string valueSeparator)
        {
            FieldInstanceValueDataOut fieldInstanceValue = FieldInstanceValues[repetitiveValueIndex];
            return IsRequired && fieldInstanceValue.Values.Count == 0 ? neTranslated : FormatDisplayValue(fieldInstanceValue, valueSeparator);
        }

        protected virtual string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue ?? string.Empty;
        }

        public virtual bool IsInputDisabled(bool isChapterReadonly, bool isSpecialValue)
        {
            return IsReadonly || isChapterReadonly || isSpecialValue;
        }

        public string GetParentFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"[name=\"{fieldInstanceRepetitionId}\"]:not([spec-value])";
        }

        public virtual string GetChildFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"[data-fieldinstancerepetitionid=\"{fieldInstanceRepetitionId}\"]";
        }

        public string GetValueForTextExport(Dictionary<int, Dictionary<int, string>> missingValues)
        {
            List<string> values = new List<string>(); 
            foreach (FieldInstanceValueDataOut fieldInstanceValue in FieldInstanceValues)
            {
                string value;
                if (fieldInstanceValue.IsSpecialValue)
                {
                    value = GetCodeMissingValue(fieldInstanceValue.FirstValue, missingValues);
                }
                else
                {
                    value = FormatDisplayValue(fieldInstanceValue, ",");
                }
                if (!string.IsNullOrEmpty(value))
                {
                    values.Add(value);
                }
            }

            return string.Join(Environment.NewLine, values);
        }

        public Tuple<string, bool> GetFieldInstanceRepetitionInfo(int index)
        {
            if(!HasAnyFieldInstanceValue)
            {
                FieldInstanceValues = new List<FieldInstanceValueDataOut>()
                {
                    new FieldInstanceValueDataOut(null)
                };
            }
            string repetitionId = FieldInstanceValues?.ElementAtOrDefault(index)?.FieldInstanceRepetitionId ?? String.Empty;
            bool isSpecialValue = FieldInstanceValues?.ElementAtOrDefault(index)?.IsSpecialValue ?? false;

            return Tuple.Create(repetitionId, isSpecialValue);
        }

        public bool HasValue(int index)
        {
            if (!HasAnyFieldInstanceValue)
            {
                FieldInstanceValues = new List<FieldInstanceValueDataOut>()
                {
                    new FieldInstanceValueDataOut(null)
                };
            }
            FieldInstanceValueDataOut fieldInstanceValueData = FieldInstanceValues.ElementAtOrDefault(index);
            return fieldInstanceValueData != null && fieldInstanceValueData.Values.Count > 0;
        }


        public bool IsNullFlavorChecked(int codeId)
        {
            if (NullFlavors.Count > 0)
                return NullFlavors.Contains(codeId);
            else
                return true;
        }

        public void HandleDependency(LogicalExpresionResult logicalExpresionResult, int? missingCodeValueId)
        {
            if (logicalExpresionResult == LogicalExpresionResult.TRUE)
            {
                this.IsVisible = true;
            }
            else
            {
                this.IsVisible = false;
                this.FieldInstanceValues.ForEach(x => x.ResetValue(this.IfSpecialValueCanBeSet(), missingCodeValueId));
            }
        }

        public void AddMissingPropertiesInDependency(FormDataOut form)
        {
            if (this.DependentOn?.DependentOnFieldInfos != null)
            {
                Dictionary<string, string> fields = form.GetAllFields().ToDictionary(x => x.Id, x => x.Type);

                foreach (DependentOnFieldInfoDataOut item in this.DependentOn.DependentOnFieldInfos)
                {
                    if (fields.TryGetValue(item.FieldId, out string fieldType))
                    {
                        item.FieldType = fieldType;
                    }
                }
            }
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
    }
}