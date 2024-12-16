using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Encounter.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterDataIn
    {
        public string EpisodeOfCareId { get; set; }
        public int Id { get; set; }
        public int StatusCD { get; set; }
        public int ClassCD { get; set; }
        public int TypeCD { get; set; }
        public int ServiceTypeCD { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int PatientId { get; set; }
        public PeriodDTO Period { get; set; }
        public List<EncounterPersonnelRelationDataIn> Doctors  { get; set; }
    }
}