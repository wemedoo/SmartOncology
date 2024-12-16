using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Linq;

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

        public void SetFormCodeRelationAndCodeToInactive(string formId, string organizationTimeZone)
        {
            FormCodeRelation formCodeRelation = GetFormCodeRelationByFormId(formId, organizationTimeZone);
            if (formCodeRelation != null)
            {
                using (var dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        formCodeRelation.Delete();
                        Code code = context.Codes.FirstOrDefault(x => x.CodeId == formCodeRelation.CodeCD);
                        if (code != null)
                        {
                            code.Delete(setLastUpdateProperty: false);
                        }

                        context.SaveChanges();
                        dbTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        throw;
                    }
                }
                
            }
        }
    }
}
