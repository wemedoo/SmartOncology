using Autofac;
using Autofac.Core.Lifetime;
using sReportsV2.BusinessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class AsyncRunner : IAsyncRunner
    {
        public ILifetimeScope LifetimeScope { get; set; }

        public AsyncRunner(ILifetimeScope lifetimeScope)
        {
            this.LifetimeScope = lifetimeScope;
        }

        public Task Run<T>(Action<T> action)
        {
            return Task.Factory.StartNew(() =>
            {
                using (ILifetimeScope container = LifetimeScope.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
                {
                    var service = container.Resolve<T>();
                    action(service);
                }
            });
        }
    }
}
