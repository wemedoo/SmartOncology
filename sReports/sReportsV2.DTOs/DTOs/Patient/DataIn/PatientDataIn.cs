using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Patient
{
    public class PatientDataIn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public int? GenderCD { get; set; }
        public int? MaritalStatusCD { get; set; }
        [Year]
        public DateTime? BirthDate { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public List<PatientContactDataIn> Contacts{ get; set; }
        public List<PatientIdentifierDataIn> Identifiers { get; set; }
        public List<PatientTelecomDataIn> Telecoms { get; set; }
        public List<PatientAddressDataIn> Addresses { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int? CitizenshipCD { get; set; }
        public int? ReligionCD { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }
        public PatientChemotherapyDataDataIn PatientChemotherapyData { get; set; }
        public string RowVersion { get; set; }
    }
}