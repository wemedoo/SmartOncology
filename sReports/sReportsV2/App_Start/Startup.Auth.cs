using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.Common.Extensions;

namespace sReportsV2
{
    public partial class Startup
    {
        private void ConfigureAuth(IServiceCollection services)
        {
            var authBuilder = services.AddAuthentication("Cookies")
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.LogoutPath = "/User/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(123);
                    options.SlidingExpiration = true; //after each request expireTimeSpan starts counting from zero
                    options.AccessDeniedPath = "/User/Logout";
                });

            if (Configuration.IsGlobalThesaurusRunning())
            {
                authBuilder
                    .AddMicrosoftAccount(microsoftOptions =>
                    {
                        microsoftOptions.ClientId = Configuration["AADAppId"];
                        microsoftOptions.ClientSecret = Configuration["AADAppSecret"];
                        microsoftOptions.SaveTokens = true;
                        microsoftOptions.CallbackPath = new PathString("/signin-microsoft");
                    })
                    .AddGoogle(options =>
                    {
                        options.ClientId = Configuration["GoogleClientId"];
                        options.ClientSecret = Configuration["GoogleClientSecret"];
                    });
            }
        }


    }
}
