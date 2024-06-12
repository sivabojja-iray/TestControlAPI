using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;


namespace TestControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [HttpPost("editing")]
        public IActionResult Index()
        {
            return BadRequest("No file uploaded.");
        }
    }
}
