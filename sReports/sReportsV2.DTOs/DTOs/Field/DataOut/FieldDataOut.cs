using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Common.Constants;
using System;
using System.Linq;
using System.Collections.Generic;
using sReportsV2.Common.Enums;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldDataOut
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
        public virtual bool IsFieldRepetitive => false;

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

        public virtual bool CanBeConnectedField()
        {
            return true;
        }


        protected void AddAttributeIfNotNull(List<string> attributes, string attributeName, object value)
        {
            if (value != null)
            {
                attributes.Add($"{attributeName}={value}");
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



        public bool IsNullFlavorChecked(int codeId)
        {
            if (NullFlavors.Count > 0)
                return NullFlavors.Contains(codeId);
            else
                return true;
        }
    }
}