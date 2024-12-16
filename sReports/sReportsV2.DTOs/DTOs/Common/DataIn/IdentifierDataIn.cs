namespace sReportsV2.DTOs.Common
{
    public class IdentifierDataIn
    {
        public int Id { get; set; }
        public int? IdentifierTypeCD { get; set; }
        public int? IdentifierUseCD { get; set; }
        public string IdentifierValue { get; set; }
        public string RowVersion { get; set; }
    }
}