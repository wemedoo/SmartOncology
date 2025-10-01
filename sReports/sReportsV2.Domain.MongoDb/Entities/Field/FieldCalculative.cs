using MongoDB.Bson.Serialization.Attributes;
using NodaTime;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using System.Data;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Calculative)]
    public partial class FieldCalculative : Field
    {
        public override string Type { get; set; } = FieldTypes.Calculative;
        public string Formula { get; set; }
        public Dictionary<string, string> IdentifiersAndVariables { get; set; }
        public CalculationFormulaType FormulaType { get; set; }
        public CalculationGranularityType? GranularityType { get; set; }
    }
}
