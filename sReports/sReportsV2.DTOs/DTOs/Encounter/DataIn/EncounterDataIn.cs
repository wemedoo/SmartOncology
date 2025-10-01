using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Encounter.DataIn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterDataIn
    {
        public string EpisodeOfCareId { get; set; }
        public int Id { get; set; }
        [Display(Name = "Status")]
        [Required]
        public int StatusCD { get; set; }
        [Display(Name = "Class")]
        [Required]
        public int ClassCD { get; set; }
        [Display(Name = "Type")]
        [Required]
        public int TypeCD { get; set; }
        [Display(Name = "Service Type")]
        [Required]
        public int ServiceTypeCD { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int PatientId { get; set; }
        public PeriodOffsetDTO Period { get; set; }
        public List<EncounterPersonnelRelationDataIn> Doctors  { get; set; }
    }
}