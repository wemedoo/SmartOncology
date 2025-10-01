using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Number)]
    public partial class FieldNumeric : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Number;
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Step { get; set; }

        public override bool IsDistributiveField() => true;
    }
}
