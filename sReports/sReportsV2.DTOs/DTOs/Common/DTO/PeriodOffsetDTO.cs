using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.Common.DTO
{
    public class PeriodOffsetDTO
    {
        [Required]
        public DateTimeOffset StartDate { get; set; }
        [DateRange]
        public DateTimeOffset? EndDate { get; set; }

        public string EndToTimeZonedString(string dateFormat)
        {
            return EndDate != null ? EndDate.Value.ToTimeZoned(dateFormat) : string.Empty;
        }

        public string EndToTimeZonedDateString(string dateFormat)
        {
            return EndDate != null ? EndDate.ToTimeZonedDate(dateFormat) : string.Empty;
        }
        
        public string EndToTimeZonedTimeString(string dateFormat)
        {
            return EndDate != null ? EndDate.ToTimeZonedTime(dateFormat) : string.Empty;
        }
    }
}
