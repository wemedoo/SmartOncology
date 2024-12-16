using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.HL7
{
    public class ErrorMessageLog : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ErrorMessageLogId { get; set; }

        public int HL7MessageLogId { get; set; }
        [ForeignKey("HL7MessageLogId")]
        public HL7MessageLog HL7MessageLog { get; set; }

        public int? ErrorTypeCD { get; set; }

        [ForeignKey("ErrorTypeCD")]
        public Code ErrorType { get; set; }
        public string ErrorText { get; set; } 

        public string HL7EventType { get; set; }
        public int? SourceSystemCD { get; set; }

        [ForeignKey("SourceSystemCD")]
        public Code SourceSystem { get; set; }
        public DateTimeOffset? TransactionDatetime { get; set; } 
    }
}
