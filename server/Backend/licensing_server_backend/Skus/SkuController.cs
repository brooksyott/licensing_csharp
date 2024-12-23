using Licensing.Common;
using Licensing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.Skus
{
    [Route("api/v1/sku")]
    [ApiController]
    public class SkuController : ControllerBase
    {
        private readonly ISkuService _skuService;

        public SkuController(ISkuService skuService)
        {
            _skuService = skuService;
        }

        // GET: api/<SkuController>
        [HttpGet]
        public async Task<IActionResult> GetSkus([FromQuery] BasicQueryFilter basicQueryFilter)
        {
            var result = await _skuService.GetSkusAsync(basicQueryFilter);
            return result.ToActionResult();
        }

        // GET api/<SkuController>/5
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetSkuById(int id)
        {
            var result = await _skuService.GetSkusByIdAsync(id);
            return result.ToActionResult();

        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetSkuByName(string name)
        {
            var result = await _skuService.GetSkusByNameAsync(name);
            return result.ToActionResult();
        }

        [HttpGet("{skuCode}")]
        public async Task<IActionResult> GetSku(string skuCode)
        {
            var result = await _skuService.GetSkusByCodeAsync(skuCode);
            return result.ToActionResult();

        }

        // POST api/<SkuController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Sku value)
        {
            var result = await _skuService.AddSkuAsync(value);
            return result.ToActionResult();
        }

        // PUT api/<SkuController>/5
        [HttpPut("id/{id}")]
        [HttpPatch("id/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SkuUpdate value)
        {
            var result = await _skuService.UpdateSkuAsync(id, value);
            return result.ToActionResult();
        }

        // PUT api/<SkuController>/5
        [HttpPut("{skuCode}")]
        [HttpPatch("{skuCode}")]
        public async Task<IActionResult> Put(string skuCode, [FromBody] SkuUpdate value)
        {
            var result = await _skuService.UpdateSkuAsync(skuCode, value);
            return result.ToActionResult();
        }

        // DELETE api/<SkuController>/5
        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _skuService.DeleteSkuAsync(id);
            return result.ToActionResult();
        }

        // DELETE api/<SkuController>/5
        [HttpDelete("{skuCode}")]
        public async Task<IActionResult> Delete(string skuCode)
        {
            var result = await _skuService.DeleteSkuAsync(skuCode);
            return result.ToActionResult();
        }
    }
}
