using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common.DTO
{
    public class PeriodOffsetDTO
    {
        [Required]
        public DateTimeOffset StartDate { get; set; }
        [DateRange]
        public DateTimeOffset? EndDate { get; set; }
    }
}
