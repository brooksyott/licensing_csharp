using Licensing.Common;
using Licensing.Skus;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.Keys
{
    [Route("api/v1/keys")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly ILogger<KeysController> _logger;
        private readonly IKeyService _keyService;

        public KeysController(ILogger<KeysController> logger, IKeyService licenseService)
        {
            this._logger = logger;
            this._keyService = licenseService;
        }

        // GET: api/<LicenseController>
        [HttpGet]
        public async Task<ActionResult<KeyEntity>> Get(BasicQueryFilter queryFilter)
        {
            var result = await _keyService.GetKeysAsync(queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        [HttpGet("{keyId}")]
        public async Task<ActionResult<KeyEntity>> GetById(string keyId)
        {
            var result = await _keyService.GetByIdAsync(keyId);
            return (ActionResult)result.ToActionResult();
        }


        // GET: api/<LicenseController>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<PaginatedResults>> Get([FromQuery] BasicQueryFilter queryFilter, string customerId)
        {
            var result = await _keyService.GetByCustomerIdAsync(customerId, queryFilter);
            return (ActionResult)result.ToActionResult();
        }

        // GET api/<LicenseController>/5
        [HttpGet("{keyId}/download/public")]
        public async Task<ActionResult<byte[]>> DownloadPublicKey(string keyId)
        {
            var result = await _keyService.DownloadPublicKeyAsync(keyId);
            if (result?.Data == null)
            {
                return new ObjectResult("") { StatusCode = (int)HttpStatusCode.NotFound };
            }

            return File(result.Data, "application/x-pem-file", "public_key.pem");
        }

        [HttpGet("{keyId}/download/private")]
        public async Task<ActionResult<byte[]>> DownloadPrivateKey(string keyId)
        {
            var result = await _keyService.DownloadPrivateKeyAsync(keyId);
            if (result?.Data == null)
            {
                return new ObjectResult("") { StatusCode = (int)HttpStatusCode.NotFound };
            }

            return File(result.Data, "application/x-pem-file", "private_key.pem");
        }

        // POST api/<LicenseController>
        [HttpPost]
        public async Task<ActionResult<KeyEntity>> Post([FromBody] KeyGenerationRequestBody keyGenRequest)
        {
            var result = await _keyService.GenerateKeys(keyGenRequest);
            return (ActionResult)result.ToActionResult();
        }

        // PUT api/<LicenseController>/5
        [HttpPut("{keyId}")]
        [HttpPatch("{keyId}")]
        public async Task<IActionResult> Put(string keyId, [FromBody] KeyUpdateRequestBody value)
        {
            var result = await _keyService.UpdateKeyAsync(keyId, value);
            return result.ToActionResult();
        }

        // DELETE api/<LicenseController>/5
        [HttpDelete("{keyId}")]
        public async Task<IActionResult> Delete(string keyId)
        {
            var result = await _keyService.DeleteKeyAsync(keyId);
            return result.ToActionResult();
        }
    }
}
