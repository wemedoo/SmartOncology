using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    public class EncounterView : Entity
    {
        [Key]
        [Column("EncounterId")]
        public int EncounterId { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public int? GenderCD { get; set; }
        public DateTime? BirthDate { get; set; }
        public int PatientId { get; set; }
        public DateTimeOffset? AdmissionDate { get; set; }
        public DateTimeOffset? DischargeDate { get; set; }
        public int EpisodeOfCareId { get; set; }
        public int? EpisodeOfCareTypeCD { get; set; }
        public int? StatusCD { get; set; }
        public int? TypeCD { get; set; }
    }
}
