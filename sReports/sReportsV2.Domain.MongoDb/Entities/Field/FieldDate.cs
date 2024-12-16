using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Date)]
    public class FieldDate : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Date;
        public bool PreventFutureDates { get; set; }

        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueDate;
        }

        protected override string GetDisplayValue(FieldInstanceValue fieldInstanceValue)
        {
            return base.GetDisplayValue(fieldInstanceValue).RenderDate();
        }
    }
}
