using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.HL7
{
    [Table("HL7MessageLogs")]
    public class HL7MessageLog : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int HL7MessageLogId { get; set; }
        public string MessageControlId { get; set; }
        public string Message { get; set; }

        public HL7MessageLog() 
        { 
        }

        public HL7MessageLog(string messageControlId, string plainMessage) : this()
        {
            MessageControlId = messageControlId;
            Message = plainMessage;
        }
    }
}
