using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    [Table("EpisodeOfCareWorkflows")]
    public class EpisodeOfCareWorkflow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("EpisodeOfCareWorkflowId")]
        public int EpisodeOfCareWorkflowId { get; set; }
        public string DiagnosisCondition { get; set; }
        public int DiagnosisRole { get; set; }
        public int PersonnelId { get; set; }
        public DateTimeOffset Submited { get; set; }

        [Column("StatusCD")]
        public int StatusCD { get; set; }
        [ForeignKey("EpisodeOfCareId")]
        public EpisodeOfCare EpisodeOfCare { get; set; }
        public int EpisodeOfCareId { get; set; }
    }
}
