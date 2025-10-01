using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataOut;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface  IThesaurusEntryBLL : IThesaurusEntryMergeBLL
    {
        bool ExistsThesaurusEntry(int id);
        Task<ThesaurusEntryDataOut> GetById(int id, UserCookieData userCookieData = null);
        Task<ThesaurusEntryDataOut> GetDefaultViewModel();
        ThesaurusEntryDataOut GetThesaurusByFilter(CodesFilterDataIn filterDataIn);
        ThesaurusEntryCountDataOut GetEntriesCount();
        ThesaurusEntryDataOut GetThesaurusDataOut(int id);
        PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> ReloadCodes(CodesFilterDataIn filterDataIn);
        PaginationDataOut<ThesaurusEntryViewDataOut, DataIn> ReloadTable(ThesaurusEntryFilterDataIn dataIn);
        PaginationDataOut<ThesaurusEntryDataOut, GlobalThesaurusFilterDataIn> ReloadThesaurus(GlobalThesaurusFilterDataIn filterDataIn);
        PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, ThesaurusEntryDataOut thesaurus, DTOs.User.DTO.UserCookieData userCookieData);
        PaginationDataOut<ThesaurusEntryDataOut, AdministrationFilterDataIn> GetByAdministrationTerm(AdministrationFilterDataIn dataIn);
        int TryInsertOrUpdate(ThesaurusEntryDataIn thesaurusEntry, UserData userData);
        ResourceCreatedDTO TryInsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId);
        O4CodeableConceptDataOut InsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId);
        Task<ResourceCreatedDTO> CreateThesaurus(ThesaurusEntryDataIn thesaurusEntryDTO, UserData userData);
        void DeleteCode(int id);
        Task TryDelete(int id, bool applyUsingThesaurusInCodeCheck = true, string organizationTimeZone = null);
        ThesaurusEntryDataOut UpdateTranslation(ThesaurusEntryTranslationDataIn thesaurusEntryTranslationDataIn, UserData userData);
        ThesaurusGlobalCountDataOut GetThesaurusGlobalChartData();
        int UpdateConnectionWithOntology(ThesaurusEntryDataIn thesaurusDataIn, UserData userData);
        void InsertOrUpdateCodeSystem(CodeSystemDataIn codeSystem);
        Dictionary<string, string> GetAutocompleteData(string preferredTerm, string activeLanguage);
        Task<object> ExportSkos(int thesaurusEntryId);
    }
}
