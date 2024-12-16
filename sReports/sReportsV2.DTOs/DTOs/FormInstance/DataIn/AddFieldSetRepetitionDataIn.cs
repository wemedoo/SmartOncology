using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class AddFieldSetRepetitionDataIn
    {
        public string FormId { get; set; }  
        public string FieldsetId { get; set; }  
        public bool IsLastFieldsetOnPage { get; set; }  
        public int FsNumsInRepetition { get; set; }
        public List<FieldInstanceDTO> FieldInstances { get; set; }
        public bool HiddenFieldsShown { get; set; }
    }
}
