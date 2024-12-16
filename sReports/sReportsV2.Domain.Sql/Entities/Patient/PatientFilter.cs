using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientFilter : EntityFilter
    {
        public DateTime? BirthDate { get; set; }
        public DateTimeOffset? EntryDatetime { get; set; }
        public int? IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public string City { get; set; }
        public int? CountryCD { get; set; }
        public string PostalCode { get; set; }
        public int OrganizationId { get; set; }
        public Dictionary<string, Tuple<int, string>> Genders { get; set; } = new Dictionary<string, Tuple<int, string>>();
        public string ActiveLanguage { get; set; }
        public PatientList.PatientList PatientList { get; set; }
        public int? AttendingDoctorCD { get; set; }
        public bool SimpleNameSearch { get; set; }
        public bool ApplyOrderByAndPagination { get; set; } = true;
    }
}
