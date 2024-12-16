using MongoDB.Bson.Serialization.Attributes;
using System;

namespace sReportsV2.Domain.Entities.Common
{
    public class Period
    {
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Start { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? End { get; set; }
    }
}

