using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataIn;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICodeAssociationBLL
    {
        void Insert(List<CodeAssociationDataIn> associations);
        PaginationDataOut<CodeAssociationDataOut, DataIn> GetAllFiltered(CodeAssociationFilterDataIn dataIn);
        bool ExistAssociation(int parentId, int childId);
        void Delete(int associationId);
        List<int> GetByParentId(int parentId);
        Dictionary<int, Dictionary<int, string>> InitializeMissingValueList(string language = LanguageConstants.EN);
    }
}
