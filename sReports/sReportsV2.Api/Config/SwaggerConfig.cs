using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using sReportsV2.Common.Helpers;
using System.IO;
using System.Reflection;

namespace sReportsV2.Api.Config
{
    public static class SwaggerConfig
    {
        public static void ConfigureService(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("dfd-v1", new OpenApiInfo { Title = "sReports DfD API", Version = "v1" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "sReports API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });

                c.CustomSchemaIds(x => x.FullName);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(DirectoryHelper.ProjectBaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/dfd-v1/swagger.json", "sReports DfD V1");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "sReports API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
