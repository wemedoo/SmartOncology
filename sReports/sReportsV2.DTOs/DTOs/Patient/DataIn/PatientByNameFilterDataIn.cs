namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    public class PatientByNameFilterDataIn : Common.DataIn
    {
        public string SearchValue { get; set; }
        public bool ComplexSearch { get; set; }
        public int OrganizationId { get; set; }
    }
}
