namespace TestControlAPI.Services
{
    public class FileValidator
    {
        // Method to validate the uploaded file
        public bool IsValid(IFormFile file, out string validationMessage)
        {
            // Check if no file is uploaded
            if (file == null || file.Length == 0)
            {
                validationMessage = "No file uploaded.";// Set validation message
                return false;// Return false indicating invalid file
            }

            // Get the file extension
            var fileExtension = Path.GetExtension(file.FileName);

            // Check if the file extension is not .xls or .xlsx
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                validationMessage = "Invalid file format. Please upload an Excel file.";// Set validation message
                return false;// Return false indicating invalid file
            }

            validationMessage = string.Empty;// Clear validation message as the file is valid
            return true;// Return true indicating valid file
        }
    }
}
