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

        public Task<List<Customer>> GetCustomersByType(CustomerTypes customerType);

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

            // Add the newly created entity.
            tableClient.AddEntity(customer);


            return customer.Id;
        }


        private async Task TryCreateTable()
        {
            // Create the table if it doesn't already exist to verify we've successfully authenticated.
            var table = await tableClient.CreateIfNotExistsAsync();
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var customerRes = await tableClient.GetEntityAsync<Customer>(partitionKey, customerId.ToString());

            return customerRes.Value;
        }

        public Task<PageableList<Customer>> GetCustomersByPage(int pageNum, int pageSize)
        {
            Pageable<Customer> queryResultsFilter = tableClient.Query<Customer>(filter: $"PartitionKey eq '{partitionKey}'", maxPerPage: pageSize);

            var skip = (pageNum - 1) * pageSize;

            var list = new List<Customer>();
            foreach (Customer customer in queryResultsFilter.OrderByDescending(x => x.Timestamp).Skip(skip).Take(pageSize))
            {
                list.Add(customer);
            }

            return Task.FromResult(new PageableList<Customer>(list, queryResultsFilter.Count()));
        }

        public async Task<List<Customer>> GetCustomersByType(CustomerTypes customerType)
        {
            var value = (int)customerType;
            var queryResultsFilter = tableClient.QueryAsync<Customer>(x => x.CustomerTypeValue == value);

            var list = new List<Customer>();
            await foreach (var customersPage in queryResultsFilter.AsPages())
            {
                foreach (var customer in customersPage.Values)
                {
                    list.Add(customer);
                }
            }

            return list;
        }

        public async Task UpdateCustomer(Customer customer)
        {
            var customerRes = await tableClient.GetEntityAsync<Customer>(partitionKey, customer.Id.ToString());

            if(customerRes == null)
            {
                throw new Exception("Customer not found");
            }

            customerRes.Value.Name = customer.Name;
            customerRes.Value.Address = customer.Address;
            customerRes.Value.CustomerTypeValue = customer.CustomerTypeValue;

            var res = await tableClient.UpsertEntityAsync(customerRes.Value, TableUpdateMode.Replace);
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
