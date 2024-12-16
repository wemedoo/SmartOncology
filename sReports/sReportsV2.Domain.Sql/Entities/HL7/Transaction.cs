using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.HL7
{
    public class Transaction : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TransactionId { get; set; }

        public int HL7MessageLogId { get; set; }
        [ForeignKey("HL7MessageLogId")]
        public HL7MessageLog HL7MessageLog { get; set; }

        public int? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient.Patient Patient { get; set; }
        public int? EncounterId { get; set; }
        [ForeignKey("EncounterId")]
        public Encounter.Encounter Encounter { get; set; }

        public string TransactionType { get; set; }
        public string FhirResource { get; set; }
        public string HL7EventType { get; set; }

        public int? SourceSystemCD { get; set; }

        [ForeignKey("SourceSystemCD")]
        public Code SourceSystem { get; set; }

        public int? TransactionDirectionCD { get; set; }

        [ForeignKey("TransactionDirectionCD")]
        public Code TransactionDirection { get; set; }

        public DateTimeOffset TransactionDatetime { get; set; }
    }
}
