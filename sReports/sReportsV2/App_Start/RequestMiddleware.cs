using Microsoft.AspNetCore.Http;
using sReportsV2.App_Start;
using sReportsV2.Common.Extensions;
using System.Threading;
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
                SetMetaTags(httpContext);
                SetLocalization(httpContext);
                await _next(httpContext);
            }
            catch (System.Exception ex)
            {
                await _exceptionHandler.HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private void SetMetaTags(HttpContext httpContext)
        {
            httpContext.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow");
        }

        private void SetLocalization(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue("Language", out string activeLanguage))
            {
                Thread.CurrentThread.UpdateLanguage(activeLanguage);
            }
        }
    }
}
