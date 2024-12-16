namespace sReportsV2.Common.Entities
{
    public class EntityFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}
