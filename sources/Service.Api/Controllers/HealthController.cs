using Microsoft.AspNetCore.Mvc;

namespace Service.Api.Controllers
{
    public class HealthController : ApiControllerBase
    {
        public HealthController()
        {

        }

        [HttpGet(Name = "CheckHealth")]
        public async Task<string> CheckHealth()
        {
            return await Task.FromResult("Hello World!");
        }
    }
}
