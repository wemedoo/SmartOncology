using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IThesaurusDAL
    {
        void InsertOrUpdate(ThesaurusEntry thesaurus);
        ThesaurusEntry GetById(int id);
        List<ThesaurusEntry> GetAllByIds(IEnumerable<int> ids);
        IQueryable<ThesaurusEntry> GetFilteredQuery(GlobalThesaurusFilter filterDataIn);
        List<ThesaurusEntry> GetFiltered(GlobalThesaurusFilter filterDataIn);
        int GetFilteredCount(GlobalThesaurusFilter filterDataIn);
        void DeleteCode(int id);
        int GetAllCount();
        void InsertMany(List<ThesaurusEntry> thesauruses);
        List<int> GetLastBulkInserted(int size);
        List<int> GetBulkInserted(int size);
        int GetAllEntriesCount(ThesaurusEntryFilterData filterData);
        long GetUmlsEntriesCount();
        List<ThesaurusEntryView> GetAll(ThesaurusEntryFilterData filterData);
        bool ExistsThesaurusEntry(int id);
        void Delete(int id, string organizationTimeZone = null);
        List<ThesaurusEntry> GetAllSimilar(ThesaurusReviewFilterData filter, string preferredTerm, string language, int? productionStateCD);
        long GetAllSimilarCount(ThesaurusReviewFilterData filter, string preferredTerm, string language, int? productionStateCD);
        void UpdateState(int thesaurusId, int? stateCD);
        List<ThesaurusEntry> GetByIdsList(List<int> thesaurusList);
        List<string> GetAll(string language, string searchValue, int page);
        O4CodeableConcept GetCodeById(int id);
        int GetThesaurusIdThatHasCodeableConcept(string codeValue);
        int GetIdByPreferredTerm(string prefferedTerm, string language = LanguageConstants.EN);
        ThesaurusEntry GetByPreferredTerm(string prefferedTerm, string language = LanguageConstants.EN);
    }
}
