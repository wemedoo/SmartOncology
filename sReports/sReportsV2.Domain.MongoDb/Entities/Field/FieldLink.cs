using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Link)]
    public class FieldLink : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Link;
        public string Link { get; set; }
    }
}
