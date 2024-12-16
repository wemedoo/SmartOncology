using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FieldInstance.DataIn
{
    public class FieldInstanceDependencyDataIn : DependentOnInfoDataIn
    {
        public string ChildFieldInstanceRepetitionId { get; set; }
        public string ChildFieldSetInstanceRepetitionId { get; set; }
        public bool IsChildDependentFieldSetRepetitive { get; set; }

        public List<FieldInstanceDTO> FieldInstancesInFormula { get; set; } 
    }
}
