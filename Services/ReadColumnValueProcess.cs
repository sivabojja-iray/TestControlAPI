using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadColumnValueProcess
    {
        // Method to process and read column values from the uploaded Excel file
        public async Task<List<string>> ProcessReadExcelColumnValues(IFormFile file,string columnvalue)
        {
            // List to hold the values from the specified column
            var columnValues = new List<string>();

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
                    // Get the first worksheet in the workbook
                    var worksheet = package.Workbook.Worksheets.First();

                    // Get the number of rows and columns in the worksheet
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    // Initialize columnIndex to -1, indicating column not found
                    int columnIndex = -1;

                    // Find the column index of the specified column value
                    for (int col = 1; col <= colCount; col++)
                    {
                        // Check if the header cell matches the specified column value
                        if (worksheet.Cells[1, col].Value?.ToString() == columnvalue)
                        {
                            columnIndex = col;// Set the column index if found
                            break;
                        }
                    }

                    // If the column index is not found, throw an exception
                    if (columnIndex == -1)
                    {
                        throw new ArgumentException($"Column '{columnvalue}' not found.");
                    }

                    // Read the values from the specified column (starting from row 2 to skip the header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Get the text value of the cell and add it to the list
                        var cellValue = worksheet.Cells[row, columnIndex].Text;
                        columnValues.Add(cellValue);
                    }
                }
            }

            // Return the list of column values
            return columnValues;
        }
    }
}
