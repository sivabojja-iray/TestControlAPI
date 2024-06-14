using Microsoft.AspNetCore.Http.HttpResults;
using OfficeOpenXml;
using System.IO;

namespace TestControlAPI.Services
{
    public class ExcelProcessor
    {
        // Private field to hold the XmlProcessor dependency.
        private readonly XmlProcessor _xmlProcessor;

        // Constructor to inject the XmlProcessor dependency
        public ExcelProcessor(XmlProcessor xmlProcessor)
        {
            _xmlProcessor = xmlProcessor;
        }

        // Method to process the uploaded Excel file
        public async Task ProcessExcelFile(IFormFile file)
        {
            // Set the license context for EPPlus library
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a memory stream to hold the file data
            using (var stream = new MemoryStream())
            {
                // Copy the uploaded file to the memory stream
                await file.CopyToAsync(stream);

                // Create an ExcelPackage from the memory stream
                using (var package = new ExcelPackage(stream))
                {
                    // Get the workbook from the package
                    var workbook = package.Workbook;

                    // Check if the workbook is not null
                    if (workbook != null)
                    {
                        // Iterate through each worksheet in the workbook
                        foreach (var sheet in workbook.Worksheets)
                        {
                            // Process the "Conditional Tag Expression" worksheet
                            if (sheet.Name == "Conditional Tag Expression")
                            {
                                ProcessConditionalTagExpressionWorksheet(sheet);
                            }
                            // Process the "Variables" worksheet
                            else if (sheet.Name == "Variables")
                            {
                                ProcessVariablesWorksheet(sheet);
                            }
                        }
                    }
                }
            }
        }

        // Method to process the "Conditional Tag Expression" worksheet
        private void ProcessConditionalTagExpressionWorksheet(ExcelWorksheet sheet)
        {
            // Get the number of rows in the worksheet
            int rowCount = sheet.Dimension.Rows;

            // Iterate through each row starting from the second row
            for (int row = 2; row <= rowCount; row++)
            {
                // Retrieve cell values for status, tag, and target file
                string? status = sheet.Cells[row, 1].Value?.ToString();
                string? tag = sheet.Cells[row, 2].Value?.ToString();
                string? targetFile = sheet.Cells[row, 7].Value?.ToString();

                // Skip processing if any of the required cell values are null or empty
                if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(targetFile))
                    continue;

                // Process the tag using the XmlProcessor
                _xmlProcessor.ProcessTag(status, tag, targetFile);
            }
        }

        // Method to process the "Variables" worksheet
        private void ProcessVariablesWorksheet(ExcelWorksheet sheet)
        {
            // Get the number of rows in the worksheet
            int rowCount = sheet.Dimension.Rows;

            // Iterate through each row starting from the second row
            for (int row = 2; row <= rowCount; row++)
            {
                // Retrieve cell values for target value, variable, and target file
                string? targetValue = sheet.Cells[row, 1].Value?.ToString();
                string? variable = sheet.Cells[row, 3].Value?.ToString();
                string? targetFile = sheet.Cells[row, 8].Value?.ToString();

                // Skip processing if any of the required cell values are null or empty
                if (string.IsNullOrEmpty(targetValue) || string.IsNullOrEmpty(variable) || string.IsNullOrEmpty(targetFile))
                    continue;

                // Process the variable using the XmlProcessor
                _xmlProcessor.ProcessVariable(targetValue, variable, targetFile);
            }
        }
    }
}
