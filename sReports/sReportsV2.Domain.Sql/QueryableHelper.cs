using sReportsV2.Common.Enums;
using System;
using System.Linq;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Common.Helpers
{
    public static class QueryableHelper
    {
        public static IQueryable<T> WhereEntriesAreActive<T>(this IQueryable<T> query) where T : Domain.Sql.EntitiesBase.Entity
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return WhereEntriesAreActive(query, now);
        }

        public static IQueryable<T> WhereCodeRelationsAreActive<T>(this IQueryable<T> query, string organizationTimeZone) where T : Domain.Sql.EntitiesBase.Entity
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(organizationTimeZone);
            now = TimeZoneInfo.ConvertTime(now, timeZone);
            return WhereEntriesAreActive(query, now);
        }

        public static IQueryable<T> WhereEntriesAreActive<T>(this IQueryable<T> query, DateTimeOffset date) where T : Domain.Sql.EntitiesBase.Entity
        {
            return query.Where(x => x.EntityStateCD != (int)EntityStateCode.Deleted
                && x.ActiveFrom <= date
                && x.ActiveTo >= date
            );
        }

        public static IQueryable<T> WhereEntriesAreInactive<T>(this IQueryable<T> query) where T : Domain.Sql.EntitiesBase.Entity
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            return WhereEntriesAreInactive(query, now);
        }

        public static IQueryable<T> WhereEntriesAreInactive<T>(this IQueryable<T> query, DateTimeOffset date) where T : Domain.Sql.EntitiesBase.Entity
        {
            return query.Where(x => x.EntityStateCD == (int)EntityStateCode.Deleted
                || x.ActiveFrom > date 
                || x.ActiveTo < date
            );
        }
    }
}
