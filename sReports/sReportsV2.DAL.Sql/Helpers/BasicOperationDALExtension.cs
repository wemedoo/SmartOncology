using Microsoft.EntityFrameworkCore;
using sReportsV2.Domain.Sql.EntitiesBase;

namespace sReportsV2.SqlDomain.Helpers
{
    public static class BasicOperationDALExtension
    {
        public static void UpdateEntryMetadata<T>(this DbContext context, T entry, bool setRowVersion = true, bool setEntityState = true) where T : Entity
        {
            if (setRowVersion)
            {
                context.Entry(entry).OriginalValues["RowVersion"] = entry.RowVersion;
            }
            if (setEntityState)
            {
                context.Entry(entry).State = EntityState.Modified;
            }
            entry.SetLastUpdate();
        }

        public static void UpdateEntryMetadataOnDelete<T>(this DbContext context, T entry, byte[] rowVersion) where T : Entity
        {
            context.Entry(entry).OriginalValues["RowVersion"] = rowVersion;
            entry.Delete();
        }
    }
}