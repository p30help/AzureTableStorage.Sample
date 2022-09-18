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
        public Task<Guid> Create(InsertCustomerRequest request)
        {
            var customer = new Customer()
            {
                Id = request.Id,
                Name = request.Name,
                Address = request.Address,
                CustomerType = request.CustomerType,
            };

            return _customerService.AddCustomer(customer);
        }

        [HttpGet("{id}")]
        public async Task<CustomerResponse?> Get(Guid id)
        {
            var res = await _customerService.GetCustomer(id);

            if (res == null) return null;

            return convertToCustomerRequest(res);
        }

        [HttpGet("List")]
        public async Task<PageableList<CustomerResponse>> List(int pageNum)
        {
            var list = await _customerService.GetCustomersByPage(pageNum, 5);

            return new PageableList<CustomerResponse> (list.Data.Select(x => convertToCustomerRequest(x)), list.TotalCount);
        }


        [HttpGet("ListByType/{type}")]
        public async Task<List<CustomerResponse>> ListByType(CustomerTypes type)
        {
            var list = await _customerService.GetCustomersByType(type);

            return list.Select(x => convertToCustomerRequest(x)).ToList();
        }


        [HttpPut("{id}")]
        public Task Update(Guid id, UpdateCustomerRequest request)
        {
            var customer = new Customer()
            {
                Id = id,
                Name = request.Name,
                Address = request.Address,
                CustomerType = request.CustomerType
            };

            return _customerService.UpdateCustomer(customer);
        }

        private CustomerResponse convertToCustomerRequest(Customer entity)
        {
            return new CustomerResponse()
            {
                Address = entity.Address,
                CustomerType = (CustomerTypes) entity.CustomerTypeValue,
                Id = entity.Id,
                Name = entity.Name,
                RecordDate = entity.RecordDate,
            };
        }
    }
}