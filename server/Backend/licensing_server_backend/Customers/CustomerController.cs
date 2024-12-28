using Licensing.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Licensing.Customers
{
    [Route("api/v1/customers")]
    [ApiController]
    [Authorize(Roles = "general, license-admin, admin")] 
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        // GET: api/v1/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] BasicQueryFilter basicQueryFilter)
        {
            var result = await _customerService.GetCustomersAsync(basicQueryFilter);
            return result.ToActionResult();
        }

        // GET api/v1/customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCustomerByName(string name)
        {
            var result = await _customerService.GetByNameAsync(name);
            return result.ToActionResult();
        }

        // POST api/v1/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerEntity customer)
        {
            var result = await _customerService.AddCustomerAsync(customer);
            return result.ToActionResult();
        }

        // PUT api/v1/customers/5
        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateCustomerRequestBody customerUpdate)
        {
            var result = await _customerService.UpdateCustomerAsync(id, customerUpdate);
            return result.ToActionResult();
        }

        // DELETE api/v1/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            var result = await _customerService.DeleteByIdAsync(id);
            return result.ToActionResult();
        }

        // DELETE api/v1/customers/5
        [HttpDelete("name/{name}")]
        public async Task<IActionResult> DeleteByName(string name)
        {
            var result = await _customerService.DeleteByNameAsync(name);
            return result.ToActionResult();
        }
    }


}