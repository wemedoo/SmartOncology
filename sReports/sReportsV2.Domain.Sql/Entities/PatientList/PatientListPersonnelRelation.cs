using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.PatientList
{
    public class PatientListPersonnelRelation: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PatientListPersonnelRelationId { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public int PatientListId { get; set; }
        [ForeignKey("PatientListId")]
        public virtual PatientList PatientList { get; set; }
    }
}
