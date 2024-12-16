using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstanceJsonDTO
    {
        public string FormId { get; set; }
        public string MedicalRecordNumber { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
