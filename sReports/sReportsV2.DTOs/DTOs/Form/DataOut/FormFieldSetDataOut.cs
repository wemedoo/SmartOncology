using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataIn;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormFieldSetDataOut
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
        [DataList]
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
        [DataProp]
        public string MatrixId { get; set; }
        [DataProp]
        public MatrixType? MatrixType { get; set; }
        [DataProp]
        public List<FormFieldValueDataOut> Options { get; set; } = new List<FormFieldValueDataOut>();
        [DataProp]
        public List<FormFieldSetDataOut> ListOfFieldSets { get; set; } = new List<FormFieldSetDataOut>();

        public bool IsMatrixFieldSet() 
        {
            if (this.LayoutStyle != null && this.LayoutStyle.LayoutType == sReportsV2.Common.Enums.LayoutType.Matrix)
                return true;
            return false;
        }

        public bool IsFieldSetMatrixType()
        {
            if (this.MatrixType != null && this.MatrixType == sReportsV2.Common.Enums.MatrixType.FieldSetMatrix)
                return true;
            return false;
        }

        public bool IsTextFieldType()
        {
            if (this.ListOfFieldSets.Count > 0 && this.ListOfFieldSets.FirstOrDefault().Fields?.FirstOrDefault()?.Type == FieldTypes.Text)
                return true;
            return false;
        }
    }
}