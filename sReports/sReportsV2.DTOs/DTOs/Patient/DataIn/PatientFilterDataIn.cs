using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Patient.DataIn
{
    public class PatientFilterDataIn : Common.DataIn
    {
        public DateTime? BirthDate { get; set; }
        public DateTimeOffset? EntryDatetime { get; set; }
        public int? IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public string City { get; set; }
        public int? CountryCD { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public int OrganizationId { get; set; }
        public string ActiveLanguage { get; set; }
        public Dictionary<string, Tuple<int, string>> Genders { get; set; } = new Dictionary<string, Tuple<int, string>>();
        public int? PatientListId { get; set; }
        public string PatientListName { get; set; }
        public bool? ListWithSelectedPatients { get; set; }
        public int SelectedPatientId { get; set; }
        public bool SimpleNameSearch { get; set; }
    }
}