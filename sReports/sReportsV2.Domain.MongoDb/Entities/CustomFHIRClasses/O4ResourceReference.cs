namespace sReportsV2.Domain.Entities.CustomFHIRClasses
{
    public class O4ResourceReference
    {
        public O4Identifier Identifier { get; set; }
        public string Reference { get; set; }
        public O4ResourceReference() { }
        public O4ResourceReference(string reference, O4Identifier identifier)
        {
            Identifier = identifier;
            Reference = reference;
        }

    }
}
