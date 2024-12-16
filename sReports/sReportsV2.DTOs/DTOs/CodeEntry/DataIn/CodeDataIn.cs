using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CodeEntry.DataIn
{
    public class CodeDataIn
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int OrganizationId { get; set; }
        public string RowVersion { get; set; }
        public int ThesaurusEntryId { get; set; }
        public int CodeSetId { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
    }
}