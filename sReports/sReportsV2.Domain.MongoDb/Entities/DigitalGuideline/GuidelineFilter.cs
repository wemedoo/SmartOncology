using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    public class GuidelineFilter : EntityFilter
    {
        public string Title { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }
    }
}
