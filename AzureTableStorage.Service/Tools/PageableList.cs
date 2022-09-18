using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStorage.Service.Tools
{
    public class PageableList<T>
    {
        public List<T> Data { get; }

        public int TotalCount { get; }

        public PageableList(IEnumerable<T> data, int totalCount)
        {
            Data = data.ToList();
            TotalCount = totalCount;
        }
    }
}
