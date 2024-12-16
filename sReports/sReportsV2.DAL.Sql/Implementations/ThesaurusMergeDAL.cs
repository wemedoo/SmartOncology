using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusMergeDAL : IThesaurusMergeDAL
    {
        private readonly SReportsContext context;

        public ThesaurusMergeDAL(SReportsContext context)
        {
            this.context = context;
        }

        public List<ThesaurusMerge> GetAllByState(int? state)
        {
            return context.ThesaurusMerges.WhereEntriesAreActive().Where(x => x.StateCD != null && x.StateCD == state).ToList();
        }

        public void InsertOrUpdate(ThesaurusMerge thesaurusMerge)
        {
            if (thesaurusMerge.ThesaurusMergeId == 0)
            {
                context.ThesaurusMerges.Add(thesaurusMerge);
            }
            else
            {
                thesaurusMerge.SetLastUpdate();
            }
            context.SaveChanges();
        }
    }
}
