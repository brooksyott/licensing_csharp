﻿using Licensing.Auth;
using Licensing.Common;
using Licensing.Customers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.auth
{
    [Route("api/v1/internal/auth/keys")]
    [ApiController]
    public class InternalAuthKeyController : ControllerBase
    {
        private readonly IInternalAuthKeyService _authKeyService;
        private readonly ILogger<InternalAuthKeyController> _logger;

        public InternalAuthKeyController(ILogger<InternalAuthKeyController> logger, IInternalAuthKeyService authKeyService)
        {
            _logger = logger;
            _authKeyService = authKeyService;
        }

        // GET: api/v1/internal/auth/keys
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] BasicQueryFilter basicQueryFilter)
        {
            var result = await _authKeyService.GetInternalAuthKeysAsync(basicQueryFilter);
            return result.ToActionResult();
        }

        // GET api/v1/internal/auth/keys/<guid>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var result = await _authKeyService.GetByIdAsync(id);
            return result.ToActionResult();
        }

        // GET api/v1/internal/auth/keys/key/<key-name>
        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetKeysByKey(string key)
        {
            var result = await _authKeyService.GetByKeyAsync(key);
            return result.ToActionResult();
        }


        // POST api/v1/internal/auth/keys
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InternalAuthKeyEntity authKey)
        {
            var result = await _authKeyService.AddAuthKeyAsync(authKey);
            return result.ToActionResult();
        }

        // PUT api/v1/internal/auth/keys/<guid>
        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateInternalAuthKeyRequestBody authKeyUpdate)
        {
            var result = await _authKeyService.UpdateAuthKeyAsync(id, authKeyUpdate);
            return result.ToActionResult();
        }

        // DELETE api/<InternalAuthKeyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByName(string id)
        {
            var result = await _authKeyService.DeleteByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpDelete("key/{key}")]
        public async Task<IActionResult> DeleteByAuthKey(string key)
        {
            var result = await _authKeyService.DeleteByAuthKeyAsync(key);
            return result.ToActionResult();
        }
    }
}
