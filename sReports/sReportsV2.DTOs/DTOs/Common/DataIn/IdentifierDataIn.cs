using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common
{
    public class IdentifierDataIn
    {
        public int Id { get; set; }
        [Display(Name = "Identifier Name")]
        [Required]
        public int? IdentifierTypeCD { get; set; }
        public int? IdentifierUseCD { get; set; }
        [Display(Name = "Identifier Value")]
        [Required]
        [StringLength(128)]
        public string IdentifierValue { get; set; }
        public string RowVersion { get; set; }
    }
}