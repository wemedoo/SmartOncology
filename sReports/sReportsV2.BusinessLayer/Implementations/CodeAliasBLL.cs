using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.CodeAliases.DataIn;
using sReportsV2.DTOs.DTOs.CodeAliases.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CodeAliasBLL : ICodeAliasBLL
    {
        private readonly ICodeAliasViewDAL aliasDAL;
        private readonly IInboundAliasDAL inboundAliasDAL;
        private readonly IOutboundAliasDAL outboundAliasDAL;
        private readonly IMapper mapper;

        public CodeAliasBLL(ICodeAliasViewDAL aliasDAL, IInboundAliasDAL inboundAliasDAL, IOutboundAliasDAL outboundAliasDAL, IMapper mapper)
        {
            this.aliasDAL = aliasDAL;
            this.inboundAliasDAL = inboundAliasDAL;
            this.outboundAliasDAL = outboundAliasDAL;
            this.mapper = mapper;
        }

        public CodeAliasViewDataOut GetById(int aliasId)
        {
            CodeAliasView alias = aliasDAL.GetById(aliasId);
            return mapper.Map<CodeAliasViewDataOut>(alias);
        }

        public PaginationDataOut<CodeAliasViewDataOut, DataIn> GetAllFiltered(CodeAliasFilterDataIn dataIn)
        {
            CodeAliasFilter filter = mapper.Map<CodeAliasFilter>(dataIn);
            PaginationDataOut<CodeAliasViewDataOut, DataIn> result = new PaginationDataOut<CodeAliasViewDataOut, DataIn>()
            {
                Count = this.aliasDAL.GetAllEntriesCount(filter),
                Data = mapper.Map<List<CodeAliasViewDataOut>>(this.aliasDAL.GetAllAlias(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public int InsertAliases(CodeAliasDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            if (dataIn.Outbound != null)
                return InsertInboundAndOutboundAliases(dataIn);
            else 
                return InsertInboundAlias(dataIn, false).AliasId;
        }

        public InboundAlias InsertInboundAlias(CodeAliasDataIn dataIn, bool hasOutboundAlias)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            InboundAlias inboundAlias = mapper.Map<InboundAlias>(dataIn);
            InboundAlias inboundAliasDB = inboundAliasDAL.GetById(inboundAlias.AliasId);

            try 
            {
                inboundAliasDB = InsertOrUpdateInboundAlias(inboundAliasDB, inboundAlias);
            }
            catch (Exception e)
            {
                throw new InsertAliasException("An error occurred while inserting or updating the inbound alias.", e);
            }

            if (!hasOutboundAlias)
                inboundAliasDAL.InsertInbound(inboundAliasDB);

            return inboundAliasDB;
        }

        public void IsSystemDefinedAlready(int codeId, string system, string alias, bool isInbound)
        {
            if (isInbound)
            {
                if (aliasDAL.SystemExist(codeId, system, alias, isInbound))
                    throw new DuplicateAliasException($"Inbound alias ({alias}) is already added under a ({system}) for given code value ({codeId})!");
            }
            else
            {
                if (aliasDAL.SystemExist(codeId, system, alias, isInbound))
                    throw new DuplicateAliasException($"Outbound alias ({alias}) is already added under a ({system}) for given code value ({codeId})!");
            }
        }

        public void DeleteAlias(int inboundAliasId, int outboundAliasId)
        {
            if(outboundAliasId != 0)
                outboundAliasDAL.Delete(outboundAliasId);

            inboundAliasDAL.Delete(inboundAliasId);
        }

        private int InsertInboundAndOutboundAliases(CodeAliasDataIn dataIn) 
        {
            OutboundAlias outboundAlias = mapper.Map<OutboundAlias>(dataIn);
            OutboundAlias outboundAliasDB = outboundAliasDAL.GetById(outboundAlias.AliasId);
            InboundAlias inboundAliasForInsert = InsertInboundAlias(dataIn, true);

            try
            {
                outboundAliasDB = InsertOrUpdateOutboundAlias(outboundAliasDB, outboundAlias);
            }
            catch (Exception e)
            {
                throw new InsertAliasException("An error occurred while inserting or updating the outbound alias.", e);
            }

            inboundAliasForInsert.OutboundAliasId = outboundAliasDAL.InsertOutbound(outboundAliasDB);
            inboundAliasDAL.InsertInbound(inboundAliasForInsert);

            return inboundAliasForInsert.AliasId;
        }

        private InboundAlias InsertOrUpdateInboundAlias(InboundAlias inboundAliasDB, InboundAlias inboundAlias) 
        {
            if (inboundAliasDB == null)
            {
                IsSystemDefinedAlready(inboundAlias.CodeId, inboundAlias.System, inboundAlias.Alias, true);
                inboundAliasDB = inboundAlias;
            }
            else
                inboundAliasDB.Copy(inboundAlias);

            return inboundAliasDB;
        }

        private OutboundAlias InsertOrUpdateOutboundAlias(OutboundAlias outboundAliasDB, OutboundAlias outboundAlias)
        {
            if (outboundAliasDB == null)
            {
                IsSystemDefinedAlready(outboundAlias.CodeId, outboundAlias.System, outboundAlias.Alias, false);
                outboundAliasDB = outboundAlias;
            }
            else
                outboundAliasDB.Copy(outboundAlias);

            return outboundAliasDB;
        }
    }
}
