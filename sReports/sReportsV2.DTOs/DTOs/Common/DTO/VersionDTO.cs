using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;

namespace sReportsV2.DTOs.Common.DTO
{
    public class VersionDTO
    {
        public string Id { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }

        public string GetVersion()
        {
            return $"{Major}.{Minor}";
        }

        public bool IsVersionGreater(Version version)
        {
            Ensure.IsNotNull(version, nameof(version));
            return this.Major > version.Major || this.Major == version.Major && this.Minor > version.Minor;
        }
    }
}