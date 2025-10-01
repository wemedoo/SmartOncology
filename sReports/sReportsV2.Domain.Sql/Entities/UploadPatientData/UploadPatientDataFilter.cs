using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Sql.Entities.UploadPatientData
{
    public class UploadPatientDataFilter : EntityFilter
    {
        public DateTimeOffset? DateTimeFrom { get; set; }
        public DateTimeOffset? DateTimeTo { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
    }
}
