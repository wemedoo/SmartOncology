using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Datetime)]
    public class FieldDatetime : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Datetime;
        public bool PreventFutureDates { get; set; }

        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueDateTime;
        }

        protected override string GetDisplayValue(FieldInstanceValue fieldInstanceValue)
        {
            return base.GetDisplayValue(fieldInstanceValue).RenderDatetime();
        }
    }
}
