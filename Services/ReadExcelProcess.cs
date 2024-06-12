using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadExcelProcess
    {
        public async Task<List<Dictionary<string, object>>> ProcessReadExcelFile(IFormFile file)
        {
            var data = new List<Dictionary<string, object>>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    var columns = new List<string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        columns.Add(worksheet.Cells[1, col].Text);
                    }

                    int maxRows = Math.Min(7000, rowCount - 1);

                    for (int row = 2; row <= maxRows + 1; row++)
                    {
                        var rowData = new Dictionary<string, object>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            rowData[columns[col - 1]] = worksheet.Cells[row, col].Text;
                        }
                        data.Add(rowData);
                    }
                }
            }
            return data;
        }
    }
}
