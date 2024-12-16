using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataIn;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CodeAssociationBLL : ICodeAssociationBLL
    {
        private readonly ICodeAssociationDAL codeAssociationDAL;
        private readonly IMapper Mapper; 
        public CodeAssociationBLL(ICodeAssociationDAL codeAssociationDAL, IMapper mapper)
        {
            this.codeAssociationDAL = codeAssociationDAL;
            Mapper = mapper;
        }
        public PaginationDataOut<CodeAssociationDataOut, DataIn> GetAllFiltered(CodeAssociationFilterDataIn dataIn)
        {
            CodeAssociationFilter filter = Mapper.Map<CodeAssociationFilter>(dataIn);
            PaginationDataOut<CodeAssociationDataOut, DataIn> result = new PaginationDataOut<CodeAssociationDataOut, DataIn>()
            {
                Count = this.codeAssociationDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<CodeAssociationDataOut>>(this.codeAssociationDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public void Insert(List<CodeAssociationDataIn> associations)
        {
            associations = Ensure.IsNotNull(associations, nameof(associations));

            List<CodeAssociation> entries = Mapper.Map<List<CodeAssociation>>(associations);
            codeAssociationDAL.Insert(entries);
        }

        public bool ExistAssociation(int parentId, int childId)
        {
            return codeAssociationDAL.ExistAssociation(parentId, childId);
        }

        public void Delete(int associationId)
        {
            codeAssociationDAL.Delete(associationId);
        }

        public List<int> GetByParentId(int parentId)
        {
            return codeAssociationDAL.GetByParentId(parentId);
        }

        public Dictionary<int, Dictionary<int, string>> InitializeMissingValueList(string language = LanguageConstants.EN)
        {
            return codeAssociationDAL.InitializeMissingValueList(language);
        }
    }
}