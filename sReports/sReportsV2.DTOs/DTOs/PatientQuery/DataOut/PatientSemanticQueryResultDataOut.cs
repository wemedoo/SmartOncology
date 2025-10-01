using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;

namespace sReportsV2.DTOs.DTOs.PatientQuery.DataOut
{
    public class PatientSemanticQueryResultDataOut
    {
        public PaginationDataOut<FormInstanceTableDataOut, PatientQueryFilterDataIn> FormInstanceData {  get; set; }
        public PatientQueryResultDataOut PatientQueryResultData { get; set; }
    }
}
