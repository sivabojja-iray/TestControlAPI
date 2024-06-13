using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ReadRowCellValue
    {
        public async Task<List<List<string>>> ProcessReadRowCellValue(IFormFile file, string rowCellValue)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        throw new ArgumentException("No worksheet found in the Excel file.");

                    var rowDataList = new List<List<string>>();

                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                        {
                            if (worksheet.Cells[row, col].Text == rowCellValue)
                            {
                                var rowData = new List<string>();
                                for (int c = worksheet.Dimension.Start.Column; c <= worksheet.Dimension.End.Column; c++)
                                {
                                    rowData.Add(worksheet.Cells[row, c].Text);
                                }
                                rowDataList.Add(rowData);
                                break; // Exit the inner loop once the cell value is found in the row
                            }
                        }
                    }

                    if (rowDataList.Count == 0)
                    {
                        throw new ArgumentException("No matching cell value found.");
                    }

                    return rowDataList;
                }
            }
        }
    }
}
