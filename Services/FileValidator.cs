namespace TestControlAPI.Services
{
    public class FileValidator
    {
        public bool IsValid(IFormFile file, out string validationMessage)
        {
            if (file == null || file.Length == 0)
            {
                validationMessage = "No file uploaded.";
                return false;
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                validationMessage = "Invalid file format. Please upload an Excel file.";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }
    }
}
