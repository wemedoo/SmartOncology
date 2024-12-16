using MongoDB.Bson.Serialization.Attributes;

namespace sReportsV2.Domain.Entities.CustomFHIRClasses
{
    public class O4CodeableConcept 
    {
        public string System { get; set; }
        public string Version { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? VersionPublishDate { get; set; }

        public O4CodeableConcept() { }
        public O4CodeableConcept(string system, string version, string code, string value, DateTime? versionPublishDate)
        {
            System = system;
            Version = version;
            Code = code;
            Value = value;
            VersionPublishDate = versionPublishDate;
        }
    }
}
