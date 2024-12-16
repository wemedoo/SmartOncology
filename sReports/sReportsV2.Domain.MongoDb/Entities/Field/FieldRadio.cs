using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Radio)]
    public class FieldRadio : FieldSelectable
    {
        public override string Type { get; set; } = FieldTypes.Radio;
        public override bool IsDistributiveField() => true;

        public override string GetDistributiveSelectedOptionId(string distibutedValue)
        {
            int.TryParse(distibutedValue, out int thesaurusId);
            return Values.Find(v => v.ThesaurusId == thesaurusId)?.Id;
        }
    }
}
