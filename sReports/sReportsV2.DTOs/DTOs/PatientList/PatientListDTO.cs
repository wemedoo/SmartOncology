using sReportsV2.Common.Enums;
using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PatientList
{
    public class PatientListDTO
    {
        public int PatientListId { get; set; }
        [Required]
        public string PatientListName { get; set; }
        public bool ArePatientsSelected { get; set; }
        public DateTimeOffset? ActiveFrom { get; set; }
        public DateTimeOffset? ActiveTo { get; set; }
        public DateTime? AdmissionDateFrom{ get; set; }
        public DateTime? DischargeDateTo{ get; set; }
        public int? EpisodeOfCareTypeCD { get; set; }
        public int? PersonnelTeamId { get; set; }
        public string PersonnelTeamName { get; set; }
        public int? EncounterTypeCD { get; set; }
        public int? EncounterStatusCD { get; set; }
        public int? AttendingDoctorId { get; set; }
        public string AttendingDoctorName { get; set; }
        public bool ExcludeDeceasedPatient { get; set; }
        public bool IncludeDischargedPatient { get; set; }
        public bool ShowOnlyDischargedPatient { get; set; }
        public List<PatientListPersonnelRelationDTO> PatientListPersonnelRelations { get; set; } = new List<PatientListPersonnelRelationDTO>();
    }
}
