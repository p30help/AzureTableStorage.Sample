using Azure;
using Azure.Data.Tables;
using AzureTableStorage.Service.Entities;
using AzureTableStorage.Service.Tools;
using Microsoft.Extensions.Configuration;

namespace AzureTableStorage.Service
{
    public interface ICustomerService
    {
        public Task<Customer> GetCustomer(Guid customerId);

        public Task<PageableList<Customer>> GetCustomersByPage(int pageNum, int pageSize);

        public Task<Guid> AddCustomer(Customer customer);

        public Task UpdateCustomer(Customer customer);

        public Task DeleteCustomer(Guid customerId);

        public Task DeleteEveything();
    }

    public class CustomerService : ICustomerService
    {
        private readonly TableClient tableClient;
        private const string tableName = "customers";
        private const string partitionKey = "1";

        public CustomerService(IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("StorageConnectionString");
            // Construct a new TableClient using a connection string.
            tableClient = new TableClient(cs, tableName);
        }

        public async Task<Guid> AddCustomer(Customer customer)
        {
            await TryCreateTable();

            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }

            customer.RecordDate = DateTime.UtcNow;
            customer.PartitionKey = partitionKey;

            // Make a dictionary entity by defining a <see cref="TableEntity">.
            //var entity = new TableEntity(partitionKey, customer.Id.ToString())
            //{
            //    { nameof(Customer.RecordDate), customer.RecordDate },
            //    { nameof(Customer.Name), customer.Name },
            //    { nameof(Customer.Address), customer.Address },
            //    { nameof(Customer.CustomerType), (int)customer.CustomerType }
            //};

            // Add the newly created entity.
            tableClient.AddEntity(customer);


            return customer.Id;
        }


        private async Task TryCreateTable()
        {
            // Create the table if it doesn't already exist to verify we've successfully authenticated.
            var table = await tableClient.CreateIfNotExistsAsync();

            //// Construct a new <see cref="TableServiceClient" /> using a <see cref="TableSharedKeyCredential" />.
            //var storageUri = "https://AccountName.table.core.windows.net";
            //var accountName = "AccountName";
            //var accountKey = "[ACCESS_KEY]";
            //var credential = new TableSharedKeyCredential(accountName, accountKey);
            //
            //
            //var serviceClient = new TableServiceClient(
            //    new Uri(storageUri),
            //    credential);
            //
            //string tableName = "customers";
            //TableItem table = serviceClient.CreateTableIfNotExists(tableName);
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var customerRes = await tableClient.GetEntityAsync<Customer>(partitionKey, customerId.ToString());

            return customerRes?.Value;
        }

        public Task<PageableList<Customer>> GetCustomersByPage(int pageNum, int pageSize)
        {
            Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'", maxPerPage: pageSize);

            var skip = (pageNum - 1) * pageSize;

            var list = new List<Customer>();
            foreach (TableEntity qEntity in queryResultsFilter.OrderByDescending(x => x.Timestamp).Skip(skip).Take(pageSize))
            {
                list.Add(convertToCustomer(qEntity));
            }

            return Task.FromResult(new PageableList<Customer>(list, queryResultsFilter.Count()));
        }

        private Customer convertToCustomer(TableEntity entity)
        {
            return new Customer()
            {
                RowKey = entity.RowKey,
                ETag = entity.ETag,
                PartitionKey = entity.PartitionKey,
                Timestamp = entity.Timestamp,
                Address = entity.GetString(nameof(Customer.Address)),
                CustomerTypeValue = entity.GetInt32(nameof(Customer.CustomerTypeValue)) ?? 0,
                Id = Guid.Parse(entity.RowKey),
                Name = entity.GetString(nameof(Customer.Name)),
                RecordDate = entity.GetDateTime(nameof(Customer.RecordDate)) ?? DateTime.MinValue,
            };
        }

        public Task UpdateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteCustomer(Guid customerId)
        {
            // Delete the entity given the partition and row key.
            var res = await tableClient.DeleteEntityAsync(partitionKey, customerId.ToString());
        }

        public async Task DeleteEveything()
        {
            // Deletes the table made previously.
            await tableClient.DeleteAsync();
        }
    }
}
