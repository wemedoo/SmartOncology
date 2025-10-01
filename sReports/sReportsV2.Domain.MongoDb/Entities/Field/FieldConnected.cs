using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    public class FieldConnected : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Connected;
        public List<string> ConnectedFieldIds { get; set; }
    }
}
