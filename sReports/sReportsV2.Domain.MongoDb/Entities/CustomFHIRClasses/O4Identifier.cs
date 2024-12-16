namespace sReportsV2.Domain.Entities.CustomFHIRClasses
{
    public class O4Identifier 
    {
        public IdentifierUse UseElement { get; set; }
        public string Value { get; set; }
        public O4Identifier() { }

        public O4Identifier(string value, IdentifierUse idUse)
        {
            Value = value;
            UseElement = idUse;
        }

        public enum IdentifierUse
        {
            Usual,
            Official,
            Temp,
            Secondary
        }
    }
}
