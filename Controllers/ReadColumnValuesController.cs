using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestControlAPI.Services;

namespace TestControlAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReadColumnValuesController : Controller
    {
        private readonly FileValidator _fileValidator;
        private readonly ReadColumnValueProcess _readColumnValue;
        public ReadColumnValuesController(FileValidator fileValidator, ReadColumnValueProcess readColumnValue)
        {
            _fileValidator = fileValidator;
            _readColumnValue = readColumnValue;
        }
        [HttpPost(Name = "compare")]
        public async Task<IActionResult> ReadColumnValue(IFormFile file, string columnvalue)
        {
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                return BadRequest(validationMessage);
            }
            try
            {
                var data = await _readColumnValue.ProcessReadExcelColumnValues(file, columnvalue);
                return StatusCode(200, data);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
