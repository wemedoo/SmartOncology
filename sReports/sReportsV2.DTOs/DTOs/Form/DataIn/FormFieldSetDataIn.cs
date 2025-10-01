using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Field.DataIn;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormFieldSetDataIn : IViewModeDataIn
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public List<FieldDataIn> Fields { get; set; } = new List<FieldDataIn>();
        public FormLayoutStyleDataIn LayoutStyle { get; set; }
        public bool IsBold { get; set; }
        public FormHelpDataIn Help { get; set; }
        public string MapAreaId { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public bool IsReadOnlyViewMode { get; set; }
        public string FormId { get; set; }
        public string MatrixId { get; set; }
        public MatrixType? MatrixType { get; set; }
        public List<FormFieldValueDataIn> Options { get; set; } = new List<FormFieldValueDataIn>();
        public List<FormFieldSetDataIn> ListOfFieldSets { get; set; } = new List<FormFieldSetDataIn>();
    }
}