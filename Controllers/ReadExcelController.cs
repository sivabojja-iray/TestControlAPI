using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestControlAPI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestControlAPI.Controllers
{
    // Apply authorization to ensure only authenticated users can access the controller
    [Authorize]

    // Set the base route for all actions in this controller
    [Route("api/[controller]")]

    // Designate this class as an API controller
    [ApiController]
    public class ReadExcelController : ControllerBase
    {
        // Dependency injection for FileValidator service
        private readonly FileValidator _fileValidator;

        // Dependency injection for ReadExcelProcess service
        private readonly ReadExcelProcess _readExcelProcess;

        // Constructor to initialize dependencies
        public ReadExcelController(FileValidator fileValidator, ReadExcelProcess readExcelProcess)
        {
            _fileValidator = fileValidator;// Assign the injected FileValidator instance
            _readExcelProcess = readExcelProcess;// Assign the injected ReadExcelProcess instance
        }
        /// <summary>
        /// Uploads an Excel file and processes its content.
        /// </summary>
        /// <param name="file">The Excel file to upload</param>
        /// <returns>A list of dictionaries representing the rows in the Excel file</returns>
        // Define the HTTP POST method and name it "upload"
        [HttpPost(Name = "upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Validate the uploaded file using the FileValidator service
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                // Return a 400 Bad Request response with the validation message if validation fails
                return BadRequest(validationMessage);
            }

            try
            {
                // Process the Excel file and retrieve data
                var data = await _readExcelProcess.ProcessReadExcelFile(file);

                // Return a 200 OK response with the processed data
                return StatusCode(200, data);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error response with the exception message in case of an error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}