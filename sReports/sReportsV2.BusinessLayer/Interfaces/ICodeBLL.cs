using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.CodeEntry.DataIn;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using sReportsV2.Common.Constants;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICodeBLL
    {
        Dictionary<string, List<EnumData>> GetDocumentPropertiesEnums();
        int Insert(CodeDataIn codeDataIn);
        int InsertWithPreferredTerm(CodeDataIn codeDataIn, string preferredTerm);
        void Delete(int codeId);
        PaginationDataOut<CodeDataOut, DataIn> GetAllFiltered(CodeFilterDataIn dataIn);
        Code GetById(int codeId);
        List<CodeDataOut> GetByCodeSetId(int codeSetId);
        PaginationDataOut<CodeDataOut, DataIn> GetAllAssociationsFiltered(CodeFilterDataIn dataIn);
        int GetIdByPreferredTerm(string preferredTerm);
        int GetByCodeSetIdAndPreferredTerm(int codeSetId, string preferredTerm);
        List<CodeDataOut> GetAssociatedDocuments(int completeQuestionnaireId);
        List<CodeDataOut> GetAssociatedCodes(List<int> associationChildIds);
    }
}
