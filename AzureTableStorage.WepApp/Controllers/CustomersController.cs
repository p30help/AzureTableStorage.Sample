using Azure.Data.Tables;
using AzureTableStorage.Service;
using AzureTableStorage.Service.Entities;
using AzureTableStorage.Service.Tools;
using Microsoft.AspNetCore.Mvc;

namespace AzureTableStorage.WepApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        
        private readonly ILogger<CustomersController> _logger;
        private readonly ICustomerService _customerService;

        public CustomersController(ILogger<CustomersController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpPost]
        public Task<Guid> Create(CustomerRequest customer)
        {
            return _customerService.AddCustomer(new Customer()
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                CustomerType = customer.CustomerType,
            });
        }

        [HttpGet("{id}")]
        public async Task<CustomerRequest> Get(Guid id)
        {
            var res = await _customerService.GetCustomer(id);

            if (res == null) return null;

            return convertToCustomerRequest(res);
        }

        [HttpGet("List")]
        public async Task<PageableList<CustomerRequest>> List(int pageNum)
        {
            var list = await _customerService.GetCustomersByPage(pageNum, 5);

            return new PageableList<CustomerRequest> (list.Data.Select(x => convertToCustomerRequest(x)), list.TotalCount);
        }

        private CustomerRequest convertToCustomerRequest(Customer entity)
        {
            return new CustomerRequest()
            {
                Address = entity.Address,
                CustomerType = entity.CustomerType,
                Id = entity.Id,
                Name = entity.Name,
                RecordDate = entity.RecordDate,
            };
        }
    }
}