using OfficeOpenXml;

namespace TestControlAPI.Services
{
    public class ExcelProcessor
    {
        private readonly XmlProcessor _xmlProcessor;

        public ExcelProcessor(XmlProcessor xmlProcessor)
        {
            _xmlProcessor = xmlProcessor;
        }

        public async Task ProcessExcelFile(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var workbook = package.Workbook;
                    if (workbook != null)
                    {
                        foreach (var sheet in workbook.Worksheets)
                        {
                            if (sheet.Name == "Conditional Tag Expression")
                            {
                                ProcessConditionalTagExpressionWorksheet(sheet);
                            }
                            else if (sheet.Name == "Variables")
                            {
                                ProcessVariablesWorksheet(sheet);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessConditionalTagExpressionWorksheet(ExcelWorksheet sheet)
        {
            int rowCount = sheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string? status = sheet.Cells[row, 1].Value?.ToString();
                string? tag = sheet.Cells[row, 2].Value?.ToString();
                string? targetFile = sheet.Cells[row, 7].Value?.ToString();

                if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(targetFile))
                    continue;

                _xmlProcessor.ProcessTag(status, tag, targetFile);
            }
        }

        private void ProcessVariablesWorksheet(ExcelWorksheet sheet)
        {
            int rowCount = sheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string? targetValue = sheet.Cells[row, 1].Value?.ToString();
                string? variable = sheet.Cells[row, 3].Value?.ToString();
                string? targetFile = sheet.Cells[row, 8].Value?.ToString();

                if (string.IsNullOrEmpty(targetValue) || string.IsNullOrEmpty(variable) || string.IsNullOrEmpty(targetFile))
                    continue;

                _xmlProcessor.ProcessVariable(targetValue, variable, targetFile);
            }
        }
    }
}
