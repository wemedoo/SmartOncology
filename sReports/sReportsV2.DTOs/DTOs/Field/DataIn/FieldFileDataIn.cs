namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldFileDataIn : FieldStringDataIn
    {
        public string File { get; set; }
        public bool DataExtractionEnabled { get; set; }
    }
}