using Microsoft.AspNetCore.Http;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICodeSetBLL
    {
        void Insert(CodeSetDataIn dataIn);
        Task InsertAsync(CodeSetDataIn dataIn);
        PaginationDataOut<CodeSetDataOut, DataIn> GetAllFiltered(CodeSetFilterDataIn dataIn);
        int GetCodeSetThesaurusId(int codeSetId);
        bool ExistCodeSet(int codeSetId);
        bool ExistCodeSetByPreferredTerm(string codeSetName);
        int GetByPreferredTerm(string preferredTerm);
        AutocompleteResultDataOut GetAutoCompleteCodes(AutocompleteDataIn dataIn, int codesetId, string activeLanguage);
        bool ImportFileFromCsv(IFormFile file, string codesetName, bool applicableInDesigner);
        CodeSetDataOut GetById(int codeSetId);
        List<CodeSetDataOut> GetAllByPreferredTerm(string preferredTerm);
        void Delete(int codeSetId);
        Task<AutocompleteResultDataOut> GetAutoCompleteNames(AutocompleteDataIn dataIn, bool onlyApplicableInDesigner, string language);
        string GetCodedCodeSetDisplay(int codeSetId);
    }
}
