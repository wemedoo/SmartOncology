using sReportsV2.Domain.Sql.Entities.CodeEntry;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IFormCodeRelationDAL
    {
        int InsertFormCodeRelation(FormCodeRelation formCodeRelation, string organizationTimeZone = null);
        FormCodeRelation GetFormCodeRelationByFormId(string formId, string organizationTimeZone);
        bool HasFormCodeRelationByFormId(string formId, string organizationTimeZone);
        Task SetFormCodeRelationAndCodeToInactive(string formId, string organizationTimeZone);
    }
}
