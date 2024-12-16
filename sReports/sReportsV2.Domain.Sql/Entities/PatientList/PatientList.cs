using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.Domain.Sql.Entities.User;

namespace sReportsV2.Domain.Sql.Entities.PatientList
{
    public class PatientList: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PatientListId { get; set; }
        public string PatientListName { get; set; }
        public bool ArePatientsSelected { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public int? EpisodeOfCareTypeCD { get; set; }
        [ForeignKey("EpisodeOfCareTypeCD")]
        public virtual Code EpisodeOfCareType { get; set; }
        public int? PersonnelTeamId { get; set; }
        [ForeignKey("PersonnelTeamId")]
        public virtual PersonnelTeam PersonnelTeam { get; set; }
        public int? EncounterTypeCD { get; set; }
        [ForeignKey("EncounterTypeCD")]
        public virtual Code EncounterType { get; set; }
        public int? AttendingDoctorId { get; set; }
        [ForeignKey("AttendingDoctorId")]
        public virtual Personnel AttendingDoctor { get; set; }
        public int? EncounterStatusCD { get; set; }
        [ForeignKey("EncounterStatusCD")]
        public virtual Code EncounterStatus { get; set; }
        public virtual List<PatientListPersonnelRelation> PatientListPersonnelRelations { get; set; } = new List<PatientListPersonnelRelation>();
        public virtual List<PatientListPatientRelation> PatientListPatientRelations { get; set; } = new List<PatientListPatientRelation>();
        public bool ExcludeDeceasedPatient { get; set; }
        public bool IncludeDischargedPatient { get; set; }
        public bool ShowOnlyDischargedPatient { get; set; }

        public void Copy(PatientList source)
        {
            PatientListName = source.PatientListName;
            EpisodeOfCareTypeCD = source.EpisodeOfCareTypeCD;
            PersonnelTeamId = source.PersonnelTeamId;
            EncounterTypeCD = source.EncounterTypeCD;
            AttendingDoctorId = source.AttendingDoctorId;
            ArePatientsSelected = source.ArePatientsSelected;
            EncounterStatusCD = source.EncounterStatusCD;
            AdmissionDate = source.AdmissionDate;
            DischargeDate = source.DischargeDate;
            ExcludeDeceasedPatient = source.ExcludeDeceasedPatient;
            IncludeDischargedPatient = source.IncludeDischargedPatient;
            ShowOnlyDischargedPatient = source.ShowOnlyDischargedPatient;

            SetLastUpdate();
        }
    }
}
