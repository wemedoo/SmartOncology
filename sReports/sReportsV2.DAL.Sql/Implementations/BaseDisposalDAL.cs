using sReportsV2.DAL.Sql.Sql;
using System;

namespace sReportsV2.SqlDomain.Implementations
{
    public abstract class BaseDisposalDAL : IDisposable
    {
        private bool disposed;
        protected readonly SReportsContext context;

        protected BaseDisposalDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposed = true;
            }
        }
    }
}
