namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    public class PatientByNameFilterDataIn : Common.DataIn
    {
        public string Name { get; set; }
        public int OrganizationId { get; set; }
    }
}
