using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadExcelProcess
    {
        // This method processes an Excel file and returns its content as a list of dictionaries
        public async Task<List<Dictionary<string, object>>> ProcessReadExcelFile(IFormFile file)
        {
            // Initialize the list to hold the data from the Excel file
            var data = new List<Dictionary<string, object>>();

            // Set the license context for the EPPlus library to NonCommercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a memory stream to temporarily hold the uploaded file data
            using (var stream = new MemoryStream())
            {
                // Copy the file data into the memory stream asynchronously
                await file.CopyToAsync(stream);

                // Load the Excel package from the memory stream
                using (var package = new ExcelPackage(stream))
                {
                    // Get the first worksheet in the Excel workbook
                    var worksheet = package.Workbook.Worksheets.First();

                    // Get the total number of rows and columns in the worksheet
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    // Initialize a list to store the column names
                    var columns = new List<string>();

                    // Read the column names from the first row of the worksheet
                    for (int col = 1; col <= colCount; col++)
                    {
                        columns.Add(worksheet.Cells[1, col].Text);
                    }

                    // Determine the maximum number of rows to process, with a cap of 7000 rows
                    int maxRows = Math.Min(7000, rowCount - 1);

                    // Iterate over each row in the worksheet, starting from the second row
                    for (int row = 2; row <= maxRows + 1; row++)
                    {
                        // Create a dictionary to store the data for the current row
                        var rowData = new Dictionary<string, object>();

                        // Iterate over each column in the current row
                        for (int col = 1; col <= colCount; col++)
                        {
                            // Add the cell value to the dictionary, using the column name as the key
                            rowData[columns[col - 1]] = worksheet.Cells[row, col].Text;
                        }
                        // Add the row data dictionary to the main data list
                        data.Add(rowData);
                    }
                }
            }
            // Return the list of dictionaries containing the Excel file data
            return data;
        }
    }
}
