using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace Chapters.Extensions
{
    public static class FieldSetExtensions
    {
        public static List<FieldSet> GetEffectiveFieldSets(this List<FieldSet> fieldSets)
        {
            return fieldSets.FirstOrDefault().ListOfFieldSets.Any()
                ? fieldSets.First().ListOfFieldSets
                : fieldSets;
        }

        public static List<FormFieldSetDataOut> GetEffectiveFieldSets(this List<FormFieldSetDataOut> fieldSets)
        {
            return fieldSets.FirstOrDefault().ListOfFieldSets.Any()
                ? fieldSets.First().ListOfFieldSets
                : fieldSets;
        }
    }
}
