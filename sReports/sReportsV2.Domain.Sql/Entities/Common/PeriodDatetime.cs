using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    [NotMapped]
    public class PeriodDatetime
    {
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }
    }
}
