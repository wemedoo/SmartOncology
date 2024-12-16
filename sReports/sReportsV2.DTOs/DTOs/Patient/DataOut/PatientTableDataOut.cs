using sReportsV2.Common.Constants;
using System;

namespace sReportsV2.DTOs.Patient.DataOut
{
    public class PatientTableDataOut
    {
        public int Id { get; set; }
        public int? GenderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }

        public string GetPatientShortInfoString(string birthDateFormat=DateConstants.DateFormat)
        {
            string birthDate = BirthDate != null ? BirthDate.Value.ToString(birthDateFormat) : string.Empty;
            return $"{FirstName} {LastName} {birthDate}";
        }
    }
}