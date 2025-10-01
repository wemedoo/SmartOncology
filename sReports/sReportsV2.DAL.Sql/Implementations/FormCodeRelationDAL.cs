using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class FormCodeRelationDAL : IFormCodeRelationDAL
    {
        private readonly SReportsContext context;
        public FormCodeRelationDAL(SReportsContext context)
        {
            this.context = context;
        }
        public FormCodeRelation GetFormCodeRelationByFormId(string formId, string organizationTimeZone)
        {
            return context.FormCodeRelations
               .WhereCodeRelationsAreActive(organizationTimeZone)
               .Where(x => x.FormId == formId)
               .FirstOrDefault();
        }

        public bool HasFormCodeRelationByFormId(string formId, string organizationTimeZone)
        {
            return context.FormCodeRelations
               .WhereCodeRelationsAreActive(organizationTimeZone)
               .Any(x => x.FormId == formId);
        }

        public int InsertFormCodeRelation(FormCodeRelation formCodeRelation, string organizationTimeZone = null)
        {
            formCodeRelation.SetActiveFromAndToDatetime(organizationTimeZone);
            context.FormCodeRelations.Add(formCodeRelation);
            context.SaveChanges();

            return formCodeRelation.FormCodeRelationId;
        }

        public async Task SetFormCodeRelationAndCodeToInactive(string formId, string organizationTimeZone)
        {
            FormCodeRelation formCodeRelation = GetFormCodeRelationByFormId(formId, organizationTimeZone);
            if (formCodeRelation != null)
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {
                        formCodeRelation.Delete();
                        Code code = context.Codes.FirstOrDefault(x => x.CodeId == formCodeRelation.CodeCD);
                        code?.Delete(setLastUpdateProperty: false);

                        await context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
        }
    }
}
