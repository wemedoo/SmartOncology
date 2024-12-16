namespace sReportsV2.Common.Helpers
{
    public static class DirectoryHelper
    {
        public static string ProjectBaseDirectory { get; set; }

        public static string AppDataFolder
        {
            get
            {
                return $@"{ProjectBaseDirectory}\App_Data";
            }
        }
    }
}
