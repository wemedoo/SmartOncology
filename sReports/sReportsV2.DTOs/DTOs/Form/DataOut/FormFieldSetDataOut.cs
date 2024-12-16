using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldSetDataOut
    {
        [DataProp]
        public string FhirType { get; set; }
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Label { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        public List<FieldDataOut> Fields { get; set; } = new List<FieldDataOut>();
        [DataProp]
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
        [DataProp]
        public bool IsBold { get; set; }
        [DataProp]
        public string MapAreaId { get; set; }
        [DataProp]
        public FormHelpDataOut Help { get; set; }
        [DataProp]
        public bool IsRepetitive { get; set; }
        [DataProp]
        public int NumberOfRepetitions { get; set; }
        public string FieldSetInstanceRepetitionId { get; set; }
        [DataProp]
        public List<FormFieldValueDataOut> Options { get; set; } = new List<FormFieldValueDataOut>();

        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> allParentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> parentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> AllParentFieldInstanceDependencies
        {
            get => allParentFieldInstanceDependencies;
            set => allParentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> ParentFieldInstanceDependencies
        {
            get => parentFieldInstanceDependencies;
            set => parentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        public void SetParentFieldInstanceDependencies(FormDataOut formDataOut)
        {
            IEnumerable<string> fieldInstanceRepetitionIdsINFieldSet = this.Fields.SelectMany(f => f.FieldInstanceValues).Select(fIv => fIv.FieldInstanceRepetitionId);

            this.AllParentFieldInstanceDependencies = formDataOut.ParentFieldInstanceDependencies;
            this.ParentFieldInstanceDependencies = formDataOut
                .ParentFieldInstanceDependencies
                .Where(x => fieldInstanceRepetitionIdsINFieldSet.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IEnumerable<FieldDataOut> GetFieldsForDependencyFormula(string openedFieldId)
        {
            this.Fields = this.Fields ?? new List<FieldDataOut>();
            return this.Fields.Where(f => f.Id != openedFieldId && f.CanBeInDependencyFormula() && !f.IsFieldRepetitive);
        }

        public bool CanFieldSetBeIncludedInDependencyFormula(FieldDataOut openedField)
        {
            return this.Id == openedField.FieldSetId || this.Fields.Exists(f => f.Id == openedField.Id) || !this.IsRepetitive;
        }

        public bool IsMatrixFieldSet() 
        {
            if (this.LayoutStyle != null && this.LayoutStyle.LayoutType == sReportsV2.Common.Enums.LayoutType.Matrix)
                return true;
            return false;
        }
    }
}