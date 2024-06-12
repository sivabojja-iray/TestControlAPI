using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadColumnValueProcess
    {
        public async Task<List<string>> ProcessReadExcelColumnValues(IFormFile file,string columnvalue)
        {
            var columnValues = new List<string>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    // Find the column index of the specified column value
                    int columnIndex = -1;
                    for (int col = 1; col <= colCount; col++)
                    {
                        if (worksheet.Cells[1, col].Value?.ToString() == columnvalue)
                        {
                            columnIndex = col;
                            break;
                        }
                    }
                    if (columnIndex == -1)
                    {
                        throw new ArgumentException($"Column '{columnvalue}' not found.");
                    }

                    // Read the values from the specified column (starting from row 2 to skip the header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var cellValue = worksheet.Cells[row, columnIndex].Text;
                        columnValues.Add(cellValue);
                    }
                }
            }
            return columnValues;
        }
    }
}
