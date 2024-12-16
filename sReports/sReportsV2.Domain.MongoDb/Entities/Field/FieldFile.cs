using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System.Linq;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.File)]
    public class FieldFile : FieldString
    {
        public override string Type { get; set; } = FieldTypes.File;

        protected override string FormatPatholinkValue(string selectedOptionId)
        {
            return this.FieldInstanceValues.FirstOrDefault()?.GetFirstValue().GetFileNameFromUri();
        }
    }
}
