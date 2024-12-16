using Microsoft.Extensions.Configuration;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using System;
using System.Configuration;

namespace sReportsV2.HL7.DTOs
{
    public class OutgoingMessageMetadataDTO
    {
        public Patient Patient { get; set; }
        public Encounter Encounter { get; set; }
        public FormInstance FormInstance { get; set; }
        public PdfDocumentDataOut PdfDocument { get; set; }
        public string OrganizationAlias { get; set; }
        public string FormAlias { get; set; }
        public string HL7EventType { get; set; }
        public string TransactionType { get; set; }
        public int? SourceSystemCD { get; set; }
        public int? TransactionDirectionCD { get; set; }
        public int HL7MessageLogId { get; set; }
        public DateTime TransactionDatetime { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}