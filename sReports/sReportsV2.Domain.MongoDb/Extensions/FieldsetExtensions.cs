using sReportsV2.Domain.Entities.Form;

namespace sReportsV2.Domain.MongoDb.Extensions
{
    public static class FieldsetExtensions
    {
        public static IEnumerable<FieldSet> GetAllFieldSets(this List<List<FieldSet>> listOfFieldSets)
        {
            IEnumerable<FieldSet> allFieldSets = Enumerable.Empty<FieldSet>();

            foreach (var fieldsets in listOfFieldSets)
            {
                var firstFieldSet = fieldsets.FirstOrDefault();
                if (firstFieldSet != null && firstFieldSet.ListOfFieldSets.Any())
                {
                    allFieldSets = allFieldSets.Concat(fieldsets.SelectMany(fs => fs.ListOfFieldSets));
                }
                else
                {
                    allFieldSets = allFieldSets.Concat(fieldsets);
                }
            }

            return allFieldSets;
        }
    }
}
