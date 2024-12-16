using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using sReportsV2.Common.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace sReportsV2
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                host.Run();
            }
            catch (Exception ex)
            {
                HandleStartupError(ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var buildConfig = config.Build();
                    string keyVaultName = buildConfig["KeyVault:Name"];
                    if (!string.IsNullOrWhiteSpace(keyVaultName))
                    {
                        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
                        var credential = new DefaultAzureCredential();
                        config.AddAzureKeyVault(keyVaultUri, credential);
                    }
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void HandleStartupError(Exception exception)
        {
            LogException(exception);

            var builder = WebHost.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc();
                services.AddRazorPages();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseStaticFiles();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();

                    endpoints.MapGet("/", async context =>
                    {
                        try
                        {
                            await AddRazorViewOnStartupError(context);  
                        }
                        catch (Exception ex)
                        {
                            await AddStaticViewOnStartupError(ex, context, app);
                        }

                    });
                });
            })
            .Build();

            builder.Run();
        }

        private static async Task AddRazorViewOnStartupError(HttpContext context)
        {
            context.Response.StatusCode = 500;
            var actionContext = new ActionContext(
            context,
            context.GetRouteData(),
            new PageActionDescriptor(),
            new ModelStateDictionary()
            );

            var viewResult = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                            {
                                {"ErrorMessage", "Application error on startup. Please contact your administartor."},
                                {"StartupError", true}
                            }
            };

            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>();
            await executor.ExecuteAsync(actionContext, viewResult);
        }

        private static async Task AddStaticViewOnStartupError(Exception exception, HttpContext context, IApplicationBuilder app)
        {
            LogException(exception);
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(Path.Combine(app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().WebRootPath, "startup-error.html"));
        }

        private static void LogException(Exception exception)
        {
            string message = ExceptionHelper.GetExceptionStackMessages(exception);
            string stackTrace = exception.StackTrace;
            Log.Error("Exception occurred on Application startup: {Message} StackTrace: {StackTrace}", message, stackTrace);
            Debug.WriteLine("***************** ERROR ON STARTUP ***********************");
            Debug.WriteLine($"Exception occurred on Application startup: {message}");
            Debug.WriteLine(stackTrace);
            Debug.WriteLine("***************** ERROR ON STARTUP ***********************");
        }
    }
}