using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechnicalTest.Controllers
{
    public class HealthCheckController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", utcTime = DateTime.UtcNow });
        }
    }
}
