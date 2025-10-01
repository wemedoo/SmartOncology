using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.MongoDb.Entities.Base
{
    public interface IFormThesaurusEntity : IReplaceThesaurusEntity
    {
        List<int> GetAllThesaurusIds();

        void GenerateTranslation(List<Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage);
    }
}
