using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using System;

public class CacheRefreshService : ICacheRefreshService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAsyncRunner _asyncRunner;

    public CacheRefreshService(IServiceProvider serviceProvider, IAsyncRunner asyncRunner)
    {
        _serviceProvider = serviceProvider;
        _asyncRunner = asyncRunner;
    }

    public void RefreshCache(int? resourceId = null, ModifiedResourceType? modifiedResourceType = null, bool callAsyncRunner = true)
    {
        if (callAsyncRunner)
        {
            _asyncRunner.Run<CacheRefreshService>(svc => svc.RefreshCache(resourceId, modifiedResourceType, callAsyncRunner: false));
        }
        else
        {
            using var scope = _serviceProvider.CreateScope();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var codeAliasViewDAL = scope.ServiceProvider.GetRequiredService<ICodeAliasViewDAL>();
            var codeDAL = scope.ServiceProvider.GetRequiredService<ICodeDAL>();

            SingletonDataContainer.Instance.RefreshSingleton(mapper, codeAliasViewDAL, codeDAL, resourceId, modifiedResourceType);
        }
    }
}