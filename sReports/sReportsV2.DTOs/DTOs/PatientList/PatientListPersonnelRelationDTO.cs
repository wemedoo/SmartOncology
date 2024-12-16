using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PatientList
{
    public class PatientListPersonnelRelationDTO
    {
        public int PatientListPersonnelRelationId{ get; set; }
        [Required]
        public int PatientListId { get; set; }
        [Required]
        public int PersonnelId { get; set; }
    }
}
