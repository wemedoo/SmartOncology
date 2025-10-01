using System.IO;

namespace sReportsV2.Common.Helpers
{
    public static class DirectoryHelper
    {
        public static string ProjectBaseDirectory { get; set; }

        public static string AppDataFolder
        {
            get
            {
                return Path.Combine(ProjectBaseDirectory, "App_Data");
            }
        }

        public static string AppStartFolder
        {
            get
            {
                return Path.Combine(ProjectBaseDirectory, "App_Start");
            }
        }
    }
}
