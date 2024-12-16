using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    public class EncounterFilter : EntityFilter
    {
        public int? TypeCD { get; set; }
        public int? StatusCD { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public int? EpisodeOfCareTypeCD { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Gender { get; set; }
        public bool ReloadEncounterFromPatient { get; set; }
        public int EncounterId { get; set; }
        public int? PatientId { get; set; }
        public Dictionary<string, Tuple<int, string>> Genders { get; set; } = new Dictionary<string, Tuple<int, string>>();
        public Dictionary<string, Tuple<int, string>> Statuses { get; set; } = new Dictionary<string, Tuple<int, string>>();
    }
}
