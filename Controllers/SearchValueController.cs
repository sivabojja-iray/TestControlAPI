using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestControlAPI.Services;

namespace TestControlAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchValueController : ControllerBase
    {
        private readonly FileValidator _fileValidator;
        private readonly ReadRowCellValue _readRowCellValue;
        public SearchValueController(FileValidator fileValidator, ReadRowCellValue readRowCellValue)
        {
            _fileValidator = fileValidator;
            _readRowCellValue = readRowCellValue;
        }
        [HttpPost(Name = "ReadRowCell")]
        public async Task<IActionResult> ReadRowCellValue(IFormFile file, string rowcellvalue)
        {
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                return BadRequest(validationMessage);
            }
            try
            {
                var data = await _readRowCellValue.ProcessReadRowCellValue(file, rowcellvalue);
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
