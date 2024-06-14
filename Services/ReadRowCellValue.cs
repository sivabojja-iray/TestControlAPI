using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadRowCellValue
    {
        // Asynchronously processes the uploaded Excel file and reads rows that contain the specified cell value
        public async Task<List<List<string>>> ProcessReadRowCellValue(IFormFile file, string rowCellValue)
        {
            // Set the license context for EPPlus library to non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Use a memory stream to temporarily hold the uploaded file's data
            using (var stream = new MemoryStream())
            {
                // Copy the file's content to the memory stream asynchronously
                await file.CopyToAsync(stream);

                // Create an ExcelPackage from the stream
                using (var package = new ExcelPackage(stream))
                {
                    // Get the first worksheet in the workbook
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    // If no worksheet is found, throw an exception
                    if (worksheet == null)
                        throw new ArgumentException("No worksheet found in the Excel file.");

                    // Initialize a list to hold the row data lists
                    var rowDataList = new List<List<string>>();

                    // Iterate through each row in the worksheet
                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        // Iterate through each column in the current row
                        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                        {
                            // Check if the cell's text matches the specified value
                            if (worksheet.Cells[row, col].Text == rowCellValue)
                            {
                                // Initialize a list to hold the data of the current row
                                var rowData = new List<string>();

                                // Iterate through each column again to collect the entire row's data
                                for (int c = worksheet.Dimension.Start.Column; c <= worksheet.Dimension.End.Column; c++)
                                {
                                    // Add the cell's text to the row data list
                                    rowData.Add(worksheet.Cells[row, c].Text);
                                }

                                // Add the row data list to the main list
                                rowDataList.Add(rowData);
                                break; // Exit the inner loop once the cell value is found in the row
                            }
                        }
                    }

                    // If no matching cell value was found, throw an exception
                    if (rowDataList.Count == 0)
                    {
                        throw new ArgumentException("No matching cell value found.");
                    }

                    // Return the list of row data lists
                    return rowDataList;
                }
            }
        }
    }
}
