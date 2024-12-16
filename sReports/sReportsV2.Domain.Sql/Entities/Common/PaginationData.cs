using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class PaginationData<T>
    {
        public int Count { get; set; }  
        public List<T> Data { get; set; }

        public PaginationData(int count, List<T> data)
        {
            Count = count;
            Data = data;
        }
    }
}
