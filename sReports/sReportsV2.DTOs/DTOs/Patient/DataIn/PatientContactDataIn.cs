using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    public class PatientContactDataIn
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? ContactRoleId { get; set; }
        public int? ContactRelationshipId { get; set; }
        public int? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public List<PatientContactAddressDataIn> Addresses { get; set; }
        public string Gender { get; set; }
        public List<PatientContactTelecomDataIn> Telecoms { get; set; }
        public string RowVersion { get; set; }
    }
}
