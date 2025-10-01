using System;

namespace sReportsV2.DTOs.DTOs.UploadPatientData.DataIn
{
    public class UploadPatientFilterDataIn : Common.DataIn
    {
        public DateTimeOffset? DateTimeFrom { get; set; }
        public DateTimeOffset? DateTimeTo { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
    }
}
