using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.IO;

namespace sReportsV2.Domain.Sql
{
    public class SReportsContextFactory : IDesignTimeDbContextFactory<SReportsContext>
    {
        public SReportsContext CreateDbContext(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var optionsBuilder = new DbContextOptionsBuilder<SReportsContext>();
            optionsBuilder.UseSqlServer(configuration["Sql"], sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

            return new SReportsContext(optionsBuilder.Options);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production) // without this line, it defaults to "Production", ignoring ASPNETCORE_ENVIRONMENT
            .ConfigureAppConfiguration((context, config) =>
            {
                config.Build();
                var initialConfig = BuildConfiguration();
                config.AddConfiguration(initialConfig);

                string keyVaultName = initialConfig["KeyVault:Name"];
                if (!string.IsNullOrWhiteSpace(keyVaultName))
                {
                    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
                    var credential = new DefaultAzureCredential();
                    config.AddAzureKeyVault(keyVaultUri, credential);
                }
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());


        public static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
