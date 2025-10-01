using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.RichTextParagraph)]
    public class FieldRichTextParagraph : FieldString
    {
        public override string Type { get; set; } = FieldTypes.RichTextParagraph;
    }
}
