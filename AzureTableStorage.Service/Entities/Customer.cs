using Azure;
using Microsoft.WindowsAzure.Storage.Table;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace AzureTableStorage.Service.Entities
{
    public class Customer : ITableEntity
    {
        public Guid Id { 
            get => Guid.Parse(RowKey); 
            set => RowKey = value.ToString(); 
        }

        public DateTime RecordDate { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [IgnoreProperty]
        public CustomerTypes CustomerType
        {
            get { return (CustomerTypes)CustomerTypeValue; }
            set { CustomerTypeValue = (int)value; }
        }
        
        public int CustomerTypeValue { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

}
