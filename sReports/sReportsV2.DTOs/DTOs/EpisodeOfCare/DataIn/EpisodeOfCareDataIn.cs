using sReportsV2.DTOs.Common.DTO;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareDataIn
    {
        public string Description { get; set; }
        public int PatientId { get; set; }
        public int Id { get; set; }
        [Required]
        public int StatusCD { get; set; }
        public int TypeCD { get; set; }
        public string DiagnosisCondition { get; set; }
        [Required]
        public int DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
        public PeriodDTO Period { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int? PersonnelTeamId { get; set; }    
    }
}