using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;

namespace sReportsV2.Common.Extensions
{
    public static class ControllerExtensions
    {
        public static string RenderPartialView(
            this Controller controller,
            IHttpContextAccessor httpContextAccessor,
            string viewName,
            object model,
            bool isChapterReadonly,
            string fieldSetId)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ControllerActionDescriptor());

            using (var sw = new StringWriter())
            {
                var engine = httpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                if (engine == null)
                {
                    throw new InvalidOperationException("ICompositeViewEngine not found in services. Make sure it is registered.");
                }

                var viewResult = engine.GetView(viewName, viewName, false);
                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Could not find view '{viewName}'");
                }

                var tempDataProvider = httpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;
                var tempData = new TempDataDictionary(httpContext, tempDataProvider);

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    controller.ViewData,
                    tempData,
                    sw,
                    new HtmlHelperOptions()
                );

                var renderTask = viewResult.View.RenderAsync(viewContext);
                renderTask.Wait();

                return sw.GetStringBuilder().ToString();
            }
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
