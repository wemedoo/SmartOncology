using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Entities.Form
{
    public class Version
    {
        public string Id { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }

        public bool IsVersionGreater(Version version) 
        {
            Ensure.IsNotNull(version, nameof(version));
            return this.Major > version.Major || this.Major == version.Major && this.Minor > version.Minor;
        }

        public string GetFullVersionString()
        {
            return $"{Major}.{Minor}";
        }
    }
}
