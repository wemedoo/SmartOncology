using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace sReportsV2.Configs
{
    public static class SerilogConfiguration
    {
        public static void ConfigureWritingToFile(IConfiguration Configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Configuration["SerilogFileLocation"], rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}