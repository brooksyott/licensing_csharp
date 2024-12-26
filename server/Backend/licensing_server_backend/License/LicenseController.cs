using Licensing.Common;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.License
{
    [Route("api/v1/licenses")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILogger<LicenseController> _logger;
        private readonly ILicenseService _tokenService;

        public LicenseController(ILogger<LicenseController> logger, ILicenseService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromQuery] BasicQueryFilter queryFilter, string id)
        {
            var result = await _tokenService.GetByIdAsync(id);
            return (ActionResult)result.ToActionResult();
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResults>> Get([FromQuery] BasicQueryFilter queryFilter)
        {
            var result = await _tokenService.GetLicensesAsync(queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<PaginatedResults>> GetByCustomerId([FromQuery] BasicQueryFilter queryFilter, string customerId)
        {
            var result = await _tokenService.GetByCustomerIdAsync(customerId, queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        // POST api/<LicenseController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateLicenseRequestBody licenseRequest)
        {
            var licenseResult = await _tokenService.GenerateLicenseAsync(licenseRequest);
            return licenseResult.ToActionResult();
        }

        // PUT api/<LicenseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LicenseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
