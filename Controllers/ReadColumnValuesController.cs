using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestControlAPI.Services;

namespace TestControlAPI.Controllers
{
    // Apply authorization to the controller. Only authenticated users can access the endpoints
    [Authorize]

    // Define the route for this controller. The "api/[controller]" placeholder will be replaced by the controller name
    [Route("api/[controller]")]

    // Indicate that this controller responds to API requests.
    [ApiController]
    public class ReadColumnValuesController : Controller
    {
        // Private fields to hold the dependencies
        private readonly FileValidator _fileValidator;
        private readonly ReadColumnValueProcess _readColumnValue;

        // Constructor to inject the dependencies
        public ReadColumnValuesController(FileValidator fileValidator, ReadColumnValueProcess readColumnValue)
        {
            _fileValidator = fileValidator;
            _readColumnValue = readColumnValue;
        }

        // Define an HTTP POST endpoint for reading column values from the uploaded file
        [HttpPost(Name = "compare")]
        public async Task<IActionResult> ReadColumnValue(IFormFile file, string columnvalue)
        {
            // Validate the uploaded file using the FileValidator service
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                // If the file is not valid, return a BadRequest response with the validation message
                return BadRequest(validationMessage);
            }
            try
            {
                // Process the column values from the Excel file using the ReadColumnValueProcess service
                var data = await _readColumnValue.ProcessReadExcelColumnValues(file, columnvalue);

                // Return an OK (200) response with the processed data
                return StatusCode(200, data);
            }
            catch (ArgumentException ex)
            {
                // If an ArgumentException occurs, return a NotFound (404) response with the exception message
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // If any other exception occurs, return a 500 Internal Server Error response with the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
