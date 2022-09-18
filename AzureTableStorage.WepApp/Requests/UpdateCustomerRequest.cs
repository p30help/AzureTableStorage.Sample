using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStorage.Service.Entities
{
    public class UpdateCustomerRequest
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public CustomerTypes CustomerType { get; set; }
    }

}
