using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.DiagnosticReport.DataIn
{
    public class CreateFromPatientDataIn
    {
        public int PatientId { get; set; }
        public int EncounterId { get; set; }
        public int EpisodeOfCareId { get; set; }
        public string FormId { get; set; }
        public List<string> Referrals { get; set; }
    }
}
