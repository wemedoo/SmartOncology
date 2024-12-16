using System.Collections.Generic;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    interface IEditChildEntries<T>
    {
        void CopyEntries(List<T> upcomingEntries);
        void DeleteExistingRemovedEntries(List<T> upcomingEntries);
        void AddNewOrUpdateOldEntries(List<T> upcomingEntries);
    }
}
