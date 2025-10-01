namespace sReportsV2.Domain.Entities.Common
{
    public class AuditLog
    {
        public string Action { get; set; }
        public string RequestType { get; set; }
        public string Controller { get; set; }
        public string Username { get; set; }
        public DateTime Time { get; set; }
        public string Json { get; set; }
    }
}
