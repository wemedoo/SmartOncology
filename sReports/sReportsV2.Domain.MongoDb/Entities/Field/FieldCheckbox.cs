using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Checkbox)]
    public class FieldCheckbox : FieldSelectable
    {
        public override string Type { get; set; } = FieldTypes.Checkbox;
        public override bool IsDistributiveField() => true;
    }
}
