using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestControlAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestControlAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        //    [HttpPost]
        //    [Route("read-excel")]
        //    public async Task<IActionResult> Upload(IFormFile file)
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            return BadRequest("No file uploaded.");
        //        }

        //        var fileExtension = Path.GetExtension(file.FileName);
        //        if (fileExtension != ".xls" && fileExtension != ".xlsx")
        //        {
        //            return BadRequest("Invalid file format. Please upload an Excel file.");
        //        }

        //        try
        //        {
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //            using (var stream = new MemoryStream())
        //            {
        //                await file.CopyToAsync(stream);
        //                using (var package = new ExcelPackage(stream))
        //                {
        //                    var workbook = package.Workbook;
        //                    if (workbook != null)
        //                    {
        //                        foreach (var sheet in workbook.Worksheets)
        //                        {
        //                            if (sheet.Name == "Conditional Tag Expression")
        //                            {
        //                                ProcessConditionalTagExpressionWorksheet(sheet);
        //                            }
        //                            else if (sheet.Name == "Variables")
        //                            {
        //                                ProcessVariablesWorksheet(sheet);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            return Ok("ExcelFile uploaded and processed successfully.");
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"Internal server error: {ex.Message}");
        //        }
        //    }

        //    private void ProcessConditionalTagExpressionWorksheet(ExcelWorksheet sheet)
        //    {
        //        int rowCount = sheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            string status = sheet.Cells[row, 1].Value?.ToString();
        //            string tag = sheet.Cells[row, 2].Value?.ToString();
        //            string targetFile = sheet.Cells[row, 7].Value?.ToString();

        //            if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(targetFile))
        //                continue;

        //            ProcessTag(status, tag, targetFile);
        //        }
        //    }
        //    private void ProcessVariablesWorksheet(ExcelWorksheet sheet)
        //    {
        //        int rowCount = sheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            string targetValue = sheet.Cells[row, 1].Value?.ToString();
        //            string variable = sheet.Cells[row, 3].Value?.ToString();
        //            string targetFile = sheet.Cells[row, 8].Value?.ToString();

        //            if (string.IsNullOrEmpty(targetValue) || string.IsNullOrEmpty(variable) || string.IsNullOrEmpty(targetFile))
        //                continue;

        //            ProcessVariable(targetValue, variable, targetFile);
        //        }
        //    }
        //    private void ProcessVariable(string targetValue, string variable, string targetFile)
        //    {
        //        XDocument doc = XDocument.Load(targetFile);
        //        var xmlAttributeVariables = doc.Descendants("Variable").ToList();

        //        foreach (var variableElement in xmlAttributeVariables)
        //        {
        //            if (variableElement.Attribute("Name")?.Value == variable)
        //            {
        //                variableElement.Remove();
        //            }
        //        }

        //        doc.Root.Element("Variables").Add(new XElement("Variable",
        //                new XAttribute("Name", variable),
        //                targetValue));
        //        doc.Save(targetFile);
        //    }
        //    private void ProcessTag(string status, string tag, string targetFile)
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(targetFile);
        //        XmlNode node = doc.SelectSingleNode("//CatapultTarget");

        //        if (node != null && node.Attributes != null)
        //        {
        //            string conditionTagExpression = node.Attributes["ConditionTagExpression"]?.Value ?? string.Empty;

        //            List<string> excludeValues = ExtractValues(conditionTagExpression, "exclude");
        //            List<string> includeValues = ExtractValues(conditionTagExpression, "include");

        //            switch (status)
        //            {
        //                case "Exclude":
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: true);
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
        //                    break;
        //                case "Include":
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: true);
        //                    break;
        //                case "Not Set":
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
        //                    conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
        //                    break;
        //            }

        //            node.Attributes["ConditionTagExpression"].Value = conditionTagExpression;
        //            doc.Save(targetFile);
        //        }
        //    }
        //    private List<string> ExtractValues(string conditionTagExpression, string section)
        //    {
        //        string pattern = $@"{section}\[(.*?)\]";
        //        Match match = Regex.Match(conditionTagExpression, pattern);
        //        if (match.Success)
        //        {
        //            string content = match.Groups[1].Value;
        //            return Regex.Matches(content, "\"(.*?)\"").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        //        }
        //        return new List<string>();
        //    }
        //    private string UpdateConditionTagExpression(string conditionTagExpression, string section, string tag, List<string> currentValues, List<string> oppositeValues, bool addTag)
        //    {
        //        if (addTag)
        //        {
        //            if (!currentValues.Contains(tag))
        //            {
        //                if (conditionTagExpression.Contains($"{section}["))
        //                {
        //                    int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
        //                    int endIndex = conditionTagExpression.IndexOf("]", startIndex);
        //                    string existingValues = conditionTagExpression.Substring(startIndex, endIndex - startIndex).Trim();
        //                    if (!string.IsNullOrEmpty(existingValues))
        //                    {
        //                        existingValues += " or ";
        //                    }
        //                    existingValues += $"\"{tag}\"";
        //                    conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + existingValues + conditionTagExpression.Substring(endIndex);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (currentValues.Contains(tag))
        //            {
        //                if (conditionTagExpression.Contains($"{section}["))
        //                {
        //                    int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
        //                    int endIndex = conditionTagExpression.IndexOf("]", startIndex);
        //                    string sectionContent = conditionTagExpression.Substring(startIndex, endIndex - startIndex);
        //                    var items = sectionContent.Split(new[] { " or " }, StringSplitOptions.None).Where(x => x.Trim() != $"\"{tag}\"").ToList();
        //                    string newSectionContent = string.Join(" or ", items);
        //                    conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + newSectionContent + conditionTagExpression.Substring(endIndex);
        //                }
        //            }
        //        }
        //        return conditionTagExpression;
        //    }

        private readonly ExcelProcessor _excelProcessor;
        private readonly FileValidator _fileValidator;

        public ExcelUploadController(ExcelProcessor excelProcessor, FileValidator fileValidator)
        {
            _excelProcessor = excelProcessor;
            _fileValidator = fileValidator;
        }

        [HttpPost(Name ="fileupload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!_fileValidator.IsValid(file, out string validationMessage))
            {
                return BadRequest(validationMessage);
            }

            try
            {
                await _excelProcessor.ProcessExcelFile(file);
                return Ok("Excel file uploaded and processed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
