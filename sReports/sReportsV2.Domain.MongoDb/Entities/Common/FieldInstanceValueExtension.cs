using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FormInstance;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;
using System.Linq;

namespace sReportsV2.Domain.Entities.Common
{
    public static class FieldInstanceValueExtension
    {
        public static string GetFirstValue(this List<FieldInstanceValue> fieldInstanceValues)
        {
            return fieldInstanceValues?.FirstOrDefault()?.GetFirstValue();
        }

        public static bool HasAnyFieldInstanceValue(this List<FieldInstanceValue> fieldInstanceValues)
        {
            return fieldInstanceValues != null && fieldInstanceValues.Count > 0;
        }

        public static List<FieldInstanceValue> GetFieldInstanceValuesOrInitial(this List<FieldInstanceValue> fieldInstanceValues)
        {
            return fieldInstanceValues ?? new List<FieldInstanceValue>();
        }

        public static int GetRepetitiveFieldCount(this List<FieldInstanceValue> fieldInstanceValues)
        {
            return fieldInstanceValues != null ? fieldInstanceValues.Count : 0;
        }

        public static List<string> GetAllValues(this FieldInstanceValue fieldInstanceValue)
        {
            return fieldInstanceValue != null && fieldInstanceValue.HasAnyValue() ? fieldInstanceValue.Values : new List<string>();
        }

        public static string GenerateGuidIfNotDefined(this string identifier)
        {
            return string.IsNullOrWhiteSpace(identifier) ? GuidExtension.NewGuidStringWithoutDashes() : identifier;
        }
    }
}
