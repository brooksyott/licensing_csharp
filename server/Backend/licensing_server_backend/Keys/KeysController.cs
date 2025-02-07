using Licensing.Common;
using Licensing.Skus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.Keys
{
    [Route("api/v1/keys")]
    [ApiController]
    [Authorize(Roles = "general, license-admin, admin")]
    public class KeysController : ControllerBase
    {
        private readonly ILogger<KeysController> _logger;
        private readonly IKeyService _keyService;

        public KeysController(ILogger<KeysController> logger, IKeyService licenseService)
        {
            this._logger = logger;
            this._keyService = licenseService;
        }

        // GET: api/v1/keys
        [HttpGet]
        public async Task<ActionResult<KeyEntity>> Get([FromQuery] BasicQueryFilter queryFilter)
        {
            bool redact = true;

            // Used to redact sensitive information from the response, specifically the private key
            if (User.IsInRole("license-admin, admin"))
            {
                redact = false;
            }

            var result = await _keyService.GetKeysAsync(queryFilter, redact);
            return (ActionResult)result.ToActionResult();
        }

        // GET: api/v1/keys/{keyId}
        [HttpGet("{keyId}")]
        public async Task<ActionResult<KeyEntity>> GetById(string keyId)
        {
            bool redact = true;
            // Used to redact sensitive information from the response, specifically the private key
            if (User.IsInRole("license-admin, admin"))
            {
                redact = false;
            }

            var result = await _keyService.GetByIdAsync(keyId, redact);
            return (ActionResult)result.ToActionResult();
        }


        /// <summary>
        /// Downloads the public key for the specified keyId
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        // GET api/v1/keys/{keyId}/download/public
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

        /// <summary>
        /// Downloads the private key for the specified keyId
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        // GET api/v1/keys/{keyId}/download/public
        [HttpGet("{keyId}/download/private")]
        [Authorize(Roles = "license-admin, admin")] // Only allow admins to download private keys
        public async Task<ActionResult<byte[]>> DownloadPrivateKey(string keyId)
        {
            var result = await _keyService.DownloadPrivateKeyAsync(keyId);
            if (result?.Data == null)
            {
                return new ObjectResult("") { StatusCode = (int)HttpStatusCode.NotFound };
            }

            return File(result.Data, "application/x-pem-file", "private_key.pem");
        }

        // POST api/v1/keys
        [HttpPost]
        [Authorize(Roles = "license-admin, admin")] // Only allow admins to generate keys
        public async Task<ActionResult<KeyEntity>> Post([FromBody] KeyGenerationRequestBody keyGenRequest)
        {
            var result = await _keyService.GenerateKeys(keyGenRequest);
            return (ActionResult)result.ToActionResult();
        }

        // PUT api/v1/keys/{keyId}
        [HttpPut("{keyId}")]
        [HttpPatch("{keyId}")]
        public async Task<IActionResult> Put(string keyId, [FromBody] KeyUpdateRequestBody value)
        {
            bool redact = true;
            // Used to redact sensitive information from the response, specifically the private key
            if (User.IsInRole("license-admin, admin"))
            {
                redact = false;
            }

            var result = await _keyService.UpdateKeyAsync(keyId, value, redact);
            return result.ToActionResult();
        }

        // DELETE api/v1/keys/{keyId}
        [HttpDelete("{keyId}")]
        [Authorize(Roles = "license-admin, admin")]
        public async Task<IActionResult> Delete(string keyId)
        {
            var result = await _keyService.DeleteKeyAsync(keyId);
            return result.ToActionResult();
        }
    }
}
