using Microsoft.AspNetCore.Http;
using sReportsV2.App_Start;
using System.Net;
using System.Threading.Tasks;

namespace sReportsV2
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GlobalExceptionHandler _exceptionHandler;


        public RequestMiddleware(RequestDelegate next, GlobalExceptionHandler exceptionHandler)
        {
            this._next = next;
            this._exceptionHandler = exceptionHandler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception ex)
            {
                await _exceptionHandler.HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }
    }
}
