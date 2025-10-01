using sReportsV2.Common.Constants;
using System;

namespace sReportsV2.DTOs.Autocomplete
{
    public class AutocompleteDataIn
    {
        public string Term { get; set; }

        public string ExcludeId { get; set; }

        public int Page { get; set; }

        public bool ShouldLoadMore(int numOfFilteredElements, int? pageSize = null)
        {
            pageSize = FixPageSizeIfNecessarry(pageSize);
            int pageNumber = FixPageNumberIfNecessarry();
            return Math.Ceiling(numOfFilteredElements / pageSize.Value * pageNumber * 1.00) > pageNumber;
        }

        public int GetHowManyElementsToSkip(int? pageSize = null)
        {
            pageSize = FixPageSizeIfNecessarry(pageSize);
            int pageNumber = FixPageNumberIfNecessarry();
            return (pageNumber - 1) * pageSize.Value;
        }

        private int FixPageSizeIfNecessarry(int? pageSize = null)
        {
            return pageSize ?? FilterConstants.DefaultPageSize;
        }

        private int FixPageNumberIfNecessarry()
        {
            return this.Page > 0 ? this.Page : 1;
        }
    }
}