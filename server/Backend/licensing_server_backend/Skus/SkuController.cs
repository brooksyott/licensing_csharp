using Licensing.Common;
using Licensing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.Skus
{
    [Route("api/v1/skus")]
    [ApiController]
    [Authorize(Roles = "general, license-admin, admin")]
    public class SkuController : ControllerBase
    {
        private readonly ISkuService _skuService;

        public SkuController(ISkuService skuService)
        {
            _skuService = skuService;
        }

        // GET: api/v1/skus
        [HttpGet]
        public async Task<IActionResult> GetSkus([FromBody] List<String>? skuList, [FromQuery] BasicQueryFilter basicQueryFilter)
        {
            if( (skuList == null) || (skuList.Count == 0))
            {
                var basicResult = await _skuService.GetSkusAsync(basicQueryFilter);
                return basicResult.ToActionResult();
            }

            var result = await _skuService.GetSkusAsync(skuList);
            return result.ToActionResult();
        }

        // GET: api/v1/skus/bylist
        [HttpPost("bylist")]
        public async Task<IActionResult> GetSkusByPost([FromBody] List<String>? skuList, [FromQuery] BasicQueryFilter basicQueryFilter)
        {
            if ((skuList == null) || (skuList.Count == 0))
            {
                var basicResult = await _skuService.GetSkusAsync(basicQueryFilter);
                return basicResult.ToActionResult();
            }

            var result = await _skuService.GetSkusAsync(skuList);
            return result.ToActionResult();
        }


        // GET: api/v1/skus/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetSkuByName(string name)
        {
            var result = await _skuService.GetSkusByNameAsync(name);
            return result.ToActionResult();
        }

        // GET: api/v1/skus/{skuCode}
        [HttpGet("{skuCode}")]
        public async Task<IActionResult> GetSku(string skuCode)
        {
            var result = await _skuService.GetSkusByCodeAsync(skuCode);
            return result.ToActionResult();

        }

        // POST api/v1/skus
        [HttpPost]
        [Authorize(Roles = "license-admin, admin")]                         // Only admins can add new SKUs
        public async Task<IActionResult> Post([FromBody] SkuEntity value)
        {
            var result = await _skuService.AddSkuAsync(value);
            return result.ToActionResult();
        }


        // PUT api/v1/skus/{skuCode}
        [HttpPut("{skuCode}")]
        [HttpPatch("{skuCode}")]
        [Authorize(Roles = "license-admin, admin")]                        // Only admins can update SKUs
        public async Task<IActionResult> Put(string skuCode, [FromBody] SkuUpdate value)
        {
            var result = await _skuService.UpdateSkuAsync(skuCode, value);
            return result.ToActionResult();
        }


        // DELETE api/v1/skus/{skuCode}
        [HttpDelete("{skuCode}")]
        [Authorize(Roles = "license-admin, admin")]                       // Only admins can delete SKUs
        public async Task<IActionResult> Delete(string skuCode)
        {
            var result = await _skuService.DeleteSkuAsync(skuCode);
            return result.ToActionResult();
        }
    }
}
