using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestControlAPI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestControlAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReadExcelController : ControllerBase
    {
        private readonly FileValidator _fileValidator;
        private readonly ReadExcelProcess _readExcelProcess;
        public ReadExcelController(FileValidator fileValidator, ReadExcelProcess readExcelProcess)
        {
            _fileValidator = fileValidator;
            _readExcelProcess = readExcelProcess;
        }
        /// <summary>
        /// Uploads an Excel file and processes its content.
        /// </summary>
        /// <param name="file">The Excel file to upload</param>
        /// <returns>A list of dictionaries representing the rows in the Excel file</returns>
        [HttpPost(Name = "upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                return BadRequest(validationMessage);
            }

            try
            {
                var data = await _readExcelProcess.ProcessReadExcelFile(file);
                return StatusCode(200, data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            //var data = new List<Dictionary<string, object>>();
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //using (var stream = new MemoryStream())
            //{
            //    await file.CopyToAsync(stream);
            //    using (var package = new ExcelPackage(stream))
            //    {
            //        var worksheet = package.Workbook.Worksheets.First();
            //        var rowCount = worksheet.Dimension.Rows;
            //        var colCount = worksheet.Dimension.Columns;

            //        var columns = new List<string>();
            //        for (int col = 1; col <= colCount; col++)
            //        {
            //            columns.Add(worksheet.Cells[1, col].Text);
            //        }

            //        int maxRows = Math.Min(7000, rowCount - 1);

            //        for (int row = 2; row <= maxRows + 1; row++)
            //        {
            //            var rowData = new Dictionary<string, object>();
            //            for (int col = 1; col <= colCount; col++)
            //            {
            //                rowData[columns[col - 1]] = worksheet.Cells[row, col].Text;
            //            }
            //            data.Add(rowData);
            //        }
            //    }
            //}

            //return StatusCode(200, data);
        }
    }
}