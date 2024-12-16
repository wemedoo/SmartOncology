using NHapi.Base.Model;
using System;

namespace sReportsV2.HL7.DTO
{
    public class IncomingMessageMetadataDTO
    {
        public IMessage ParsedMessage { get; set; }
        public int HL7MessageLogId { get; set; }
        public string TransactionType { get; set; }
        public string FhirResource { get; set; }
        public string HL7EventType { get; set; }
        public int? SourceSystemCD { get; set; }
        public int? TransactionDirectionCD { get; set; }
        public DateTime? TransactionDatetime { get; set; }
        public string ErrorText { get; set; }
        public int? ErrorTypeCD { get; set; }

        public IncomingMessageMetadataDTO()
        {
        }

        public IncomingMessageMetadataDTO(int hL7MessageLogId, string hL7EventType, int? sourceSystemCD, DateTime? transactionDatetime)
        {
            HL7MessageLogId = hL7MessageLogId;
            HL7EventType = hL7EventType;
            TransactionDatetime = transactionDatetime;
            SourceSystemCD = sourceSystemCD;
        }
    }
}