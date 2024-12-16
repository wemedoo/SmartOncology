namespace sReportsV2.Domain.Entities.FieldInstanceHistory
{
    public class FieldInstanceHistoryFilterData
    {
        public string FormInstanceId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldSetLabel { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
        public int? UserId { get; set; }
        public bool IncludeIsDeletedInQuery { get; set; } = true;

        // --- Data In ---
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}
