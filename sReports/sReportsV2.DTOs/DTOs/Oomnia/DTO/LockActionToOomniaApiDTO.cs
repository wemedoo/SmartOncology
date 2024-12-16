namespace sReportsV2.DTOs.DTOs.Oomnia.DTO
{
    public class LockActionToOomniaApiDTO
    {
        public string FormInstanceId { get; set; }
        public string ChapterId { get; set; }
        public string PageId { get; set; }
        public bool IsLocked { get; set; }
    }
}
