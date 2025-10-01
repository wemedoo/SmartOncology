using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientByNameSearchFilter : EntityFilter
    {
        public string Name { get; set; }
        public int OrganizationId { get; set; }
    }
}
