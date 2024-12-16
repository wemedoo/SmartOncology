using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeAliases.DataIn;
using sReportsV2.DTOs.DTOs.CodeAliases.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICodeAliasBLL
    {
        CodeAliasViewDataOut GetById(int aliasId);
        PaginationDataOut<CodeAliasViewDataOut, DataIn> GetAllFiltered(CodeAliasFilterDataIn dataIn);
        int InsertAliases(CodeAliasDataIn dataIn);
        InboundAlias InsertInboundAlias(CodeAliasDataIn dataIn, bool hasOutboundAlias);
        void DeleteAlias(int inboundAliasId, int outboundAliasId);
    }
}
