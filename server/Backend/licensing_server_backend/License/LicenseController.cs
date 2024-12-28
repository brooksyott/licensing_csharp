using Licensing.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.License
{
    [Route("api/v1/licenses")]
    [ApiController]
    [Authorize(Roles = "general, admin, license-admin")]
    public class LicenseController : ControllerBase
    {
        private readonly ILogger<LicenseController> _logger;
        private readonly ILicenseService _licenseService;

        public LicenseController(ILogger<LicenseController> logger, ILicenseService tokenService)
        {
            _logger = logger;
            _licenseService = tokenService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromQuery] BasicQueryFilter queryFilter, string id)
        {
            var result = await _licenseService.GetByIdAsync(id);
            return (ActionResult)result.ToActionResult();
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResults>> Get([FromQuery] BasicQueryFilter queryFilter)
        {
            var result = await _licenseService.GetLicensesAsync(queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<PaginatedResults>> GetByCustomerId([FromQuery] BasicQueryFilter queryFilter, string customerId)
        {
            var result = await _licenseService.GetByCustomerIdAsync(customerId, queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        // POST api/<LicenseController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateLicenseRequestBody licenseRequest)
        {
            var licenseResult = await _licenseService.GenerateLicenseAsync(licenseRequest);
            return licenseResult.ToActionResult();
        }

        // DELETE api/<LicenseController>/5
        [HttpDelete("{licenseId}")]
        public async Task<IActionResult> Delete(string licenseId)
        {
            var licenseResult = await _licenseService.DeleteLicenseAsync(licenseId);
            return licenseResult.ToActionResult();
        }
    }
}
