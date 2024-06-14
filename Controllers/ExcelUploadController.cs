using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestControlAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestControlAPI.Controllers
{
    // Apply authorization to the controller. Only authenticated users can access the endpoints.
    [Authorize]

    // Define the route for this controller. The "api/[controller]" placeholder will be replaced by the controller name.
    [Route("api/[controller]")]

    // Indicate that this controller responds to API requests.
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        // Private fields to hold the ExcelProcessor dependencies.
        private readonly ExcelProcessor _excelProcessor;

        // Private fields to hold the FileValidator dependencies.
        private readonly FileValidator _fileValidator;

        // Constructor to inject the dependencies.
        public ExcelUploadController(ExcelProcessor excelProcessor, FileValidator fileValidator)
        {
            _excelProcessor = excelProcessor;
            _fileValidator = fileValidator;
        }

        // Define an HTTP POST endpoint for file uploads.
        [HttpPost(Name ="fileupload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Validate the uploaded file using the FileValidator service.
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                // If the file is not valid, return a BadRequest response with the validation message.
                return BadRequest(validationMessage);
            }

            try
            {
                // Process the Excel file using the ExcelProcessor service.
                await _excelProcessor.ProcessExcelFile(file);

                // Return an OK response if the file is processed successfully.
                return Ok("Excel file uploaded and processed successfully.");
            }
            catch (Exception ex)
            {
                // If an exception occurs, return a 500 Internal Server Error response with the exception message.
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
