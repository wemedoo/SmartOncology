namespace sReportsV2.DTOs.DTOs.PatientQuery.DataIn
{
    public class PatientQueryFilterDataIn : Common.DataIn
    {
        public int DiagnoseId { get; set; }
        public bool InitialLoad { get; set; } 
        public bool IsEmptyQuery()
        {
            return DiagnoseId == 0;
        }
    }
}
