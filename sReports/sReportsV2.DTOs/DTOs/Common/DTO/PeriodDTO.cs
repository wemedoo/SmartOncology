using sReportsV2.DTOs.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common.DTO
{
    public class PeriodDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [DateRange]
        public DateTime? EndDate { get; set; }
    }
}