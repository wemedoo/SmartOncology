using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class CustomHeaderField
    {
        public string FieldId { get; set; }
        public string Label { get; set; }
        public string CustomLabel { get; set; }
        public int Order { get; set; }
        public int? DefaultHeaderCode { get; set; }
    }
}
