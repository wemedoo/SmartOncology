using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldSelectableDataOut : FieldDataOut
    {
        [DataList]
        public List<FormFieldValueDataOut> Values { get; set; } = new List<FormFieldValueDataOut>();
        [DataProp]
        public List<FormFieldDependableDataOut> Dependables { get; set; } = new List<FormFieldDependableDataOut>();

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

        public bool IsOptionChosen(string optionId)
        {
            return GetFirstFieldInstanceValues().Contains(optionId);
        }

        public FormFieldValueDataOut GetOption(string optionId)
        {
            return Values.Find(fV => fV.Id == optionId);
        }
    }
}