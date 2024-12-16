using sReportsV2.DTOs.CodeEntry.DataOut;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class IdentifierDataOut
    {
        public int Id { get; set; }
        public CodeDataOut System { get; set; }
        public string Value { get; set; }
        public CodeDataOut Use { get; set; }
        public int? IdentifierTypeId { get; set; }
        public int? IdentifierUseId { get; set; }
        public string RowVersion { get; set; }
    }
}