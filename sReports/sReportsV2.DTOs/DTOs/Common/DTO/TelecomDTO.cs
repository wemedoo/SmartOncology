using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common
{
    public class TelecomDTO
    {
        public int Id { get; set; }
        [Required]
        public string Value { get; set; }
        public int? SystemCD { get; set; }
        public int? UseCD { get; set; }
        public string RowVersion { get; set; }
    }
}