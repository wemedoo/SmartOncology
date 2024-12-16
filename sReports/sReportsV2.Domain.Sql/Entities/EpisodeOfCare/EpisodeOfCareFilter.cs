using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCareFilter : EntityFilter
    {
        public int PatientId { get; set; }
        public bool FilterByIdentifier { get; set; }
        public int StatusCD { get; set; }
        public int TypeCD { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
    }
}
