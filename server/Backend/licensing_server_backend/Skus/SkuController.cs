using Licensing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.Skus
{
    [Route("api/sku")]
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
        public async Task<ActionResult<IEnumerable<object>>> GetSkus()
        {
            var skus = await _skuService.GetSkusAsync();
            if (skus == null)
            {
                return NotFound();
            }

            return Ok(skus);
        }

        // GET api/<SkuController>/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<object>> GetSkuById(int id)
        {
            var sku = await _skuService.GetSkusByIdAsync(id);
            if (sku == null)
            {
                return NotFound();
            }

            return Ok(sku);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<object>> GetSkuByName(string name)
        {
            var sku = await _skuService.GetSkusByNameAsync(name);
            if (sku == null)
            {
                return NotFound("");
            }

            return Ok(sku);
        }

        [HttpGet("{skuCode}")]
        public async Task<ActionResult<object>> GetSku(string skuCode)
        {
            var sku = await _skuService.GetSkusByCodeAsync(skuCode);
            if (sku == null)
            {
                return NotFound();
            }

            return Ok(sku);
        }

        // POST api/<SkuController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SkuController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SkuController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
