using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestControlAPI.Services;

namespace TestControlAPI.Controllers
{
    // Ensures that only authorized users can access this controller
    [Authorize]

    // Defines the route for this controller
    [Route("api/[controller]")]

    // Specifies that this is an API controller
    [ApiController]
    public class SearchValueController : ControllerBase
    {
        // Dependencies injected via constructor
        private readonly FileValidator _fileValidator;
        private readonly ReadRowCellValue _readRowCellValue;

        // Constructor to initialize dependencies
        public SearchValueController(FileValidator fileValidator, ReadRowCellValue readRowCellValue)
        {
            _fileValidator = fileValidator;
            _readRowCellValue = readRowCellValue;
        }

        // POST method to read cell value from a specific row in the file
        [HttpPost(Name = "ReadRowCell")]// Defines the HTTP POST method and its route name
        public async Task<IActionResult> ReadRowCellValue(IFormFile file, string rowcellvalue)
        {
            // Validate the uploaded file
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                // Return a 400 Bad Request if the file is not valid
                return BadRequest(validationMessage);
            }
            try
            {
                // Process the file and read the specified cell value
                var data = await _readRowCellValue.ProcessReadRowCellValue(file, rowcellvalue);

                // Return a 200 OK status with the data
                return StatusCode(200, data);
            }
            catch (ArgumentException ex)
            {
                // Return a 404 Not Found status if an argument exception occurs
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error status for any other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
