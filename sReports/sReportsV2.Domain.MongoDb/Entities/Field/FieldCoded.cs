using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    public class FieldCoded : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Coded;
        public int CodeSetId { get; set; }
    }
}
