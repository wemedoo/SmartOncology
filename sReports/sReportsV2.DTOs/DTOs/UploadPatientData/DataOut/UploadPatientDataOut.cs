using sReportsV2.DTOs.User.DataOut;
using System;

namespace sReportsV2.DTOs.DTOs.UploadPatientData.DataOut
{
    public class UploadPatientDataOut
    {
        public int UploadPatientDataId { get; set; }
        public UserShortInfoDataOut CreatedBy { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public string UploadPath { get; set; }
    }
}
