using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Communication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CommunicationId")]
        public int CommunicationId { get; set; }
        public bool Preferred { get; set; }
        [Column("PatientId")]
        public int? PatientId { get; set; }
        public int? LanguageCD { get; set; }
        [ForeignKey("LanguageCD")]
        public Code LanguageCode { get; set; }
        public Communication() { }
        public Communication(int? languageCD, bool preferred)
        {
            this.LanguageCD = languageCD;
            this.Preferred = preferred;
        }

        public void Copy(Communication communication)
        {
            this.Preferred = communication.Preferred;
        }
    }
}
