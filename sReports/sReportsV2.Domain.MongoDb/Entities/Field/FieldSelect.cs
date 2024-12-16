using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Extensions;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Select)]
    public class FieldSelect : FieldSelectable
    {
        public override string Type { get; set; } = FieldTypes.Select;
        public override bool IsDistributiveField() => true;
    }
}
