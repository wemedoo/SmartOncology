using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using sReportsV2.Api.Security;

namespace sReportsV2.Api.Config
{
    public static class OAuthConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddMicrosoftIdentityWebApi(options =>
                 {
                     configuration.Bind("AzureAd", options);
                     options.Authority += "/v2.0";
                     options.TokenValidationParameters.ValidAudiences = new[]
                     {
                        options.Audience,
                        $"api://{options.Audience}"
                     };
                     options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.GetIssuerValidator(options.Authority).Validate;
                 }, options => configuration.Bind("AzureAd", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessAsApplication", policy => policy.Requirements.Add(new HasAnyAcceptedScopeRequirement("access_as_application")));
                options.AddPolicy("AccessAsDfDApplication", policy => policy.Requirements.Add(new HasAnyAcceptedScopeRequirement("access_as_dfd_application")));
            });

            services.AddSingleton<IAuthorizationHandler, HasAnyAcceptedScopeHandler>();
        }
    }
}
