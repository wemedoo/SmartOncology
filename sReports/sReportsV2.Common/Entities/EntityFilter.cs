namespace sReportsV2.Common.Entities
{
    public class EntityFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }

        public int GetHowManyElementsToSkip()
        {
            int page = this.Page > 0 ? this.Page : 1;
            return (page - 1) * this.PageSize;
        }
    }
}
