using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStorage.Service.Entities
{
    public class CustomerRequest
    {
        public Guid Id { get; set; }

        public DateTime RecordDate { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public CustomerTypes CustomerType { get; set; }
    }

}
