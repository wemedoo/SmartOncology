using sReportsV2.Cache.Singleton;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICacheRefreshService
    {
        void RefreshCache(int? resourceId = null, ModifiedResourceType? modifiedResourceType = null, bool callAsyncRunner = true);
    }
}
