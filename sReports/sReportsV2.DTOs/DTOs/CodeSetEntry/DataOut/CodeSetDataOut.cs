using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;

namespace sReportsV2.DTOs.DTOs.CodeSetEntry.DataOut
{
    public class CodeSetDataOut : ICommonPropertiesDTO
    {
        public int CodeSetId { get; set; }
        public int OrganizationId { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public string RowVersion { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
        public bool ApplicableInDesigner { get; set; }

        public bool IsActive()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return this.ActiveFrom <= now && this.ActiveTo >= now;
        }
    }
}
