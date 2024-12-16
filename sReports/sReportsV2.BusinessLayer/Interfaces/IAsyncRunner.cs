using System;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IAsyncRunner
    {
        Task Run<T>(Action<T> action);
    }
}
