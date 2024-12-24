using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Licensing.License
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILogger<LicenseController> _logger;
        private readonly ITokenService _tokenService;

        public LicenseController(ILogger<LicenseController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }


        // GET: api/<LicenseController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LicenseController>/5
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var jwt = await _tokenService.GenerateTokenAsync(id, "tbd@email.com");
            return jwt;
        }

        // POST api/<LicenseController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
