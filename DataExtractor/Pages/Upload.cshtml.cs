using DataExtractor.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace DataExtractor.Pages
{

    public class UploadModel : PageModel
    {
        private CSVHelper _csvHelper = new CSVHelper();

        // Declare IFormFile properties
        [BindProperty]
        public IFormFile? File1 { get; set; }

        [BindProperty]
        public IFormFile? File2 { get; set; }
        [BindProperty]
        public string Header1 { get; set; }
        [BindProperty]
        public string Header2 { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check for file upload errors
            if (File1 == null || File2 == null)
            {
                ModelState.AddModelError("", "Both CSV files must be uploaded.");
                return Page();
            }
            if (File1.Length == 0 || File2.Length == 0)
            {
                ModelState.AddModelError("", "Both files must not be empty.");
                return Page();
            }
            if (Path.GetExtension(File1.FileName).ToLower() != ".csv"
                || Path.GetExtension(File2.FileName).ToLower() != ".csv")
            {
                ModelState.AddModelError("", "Please upload CSV files only.");
                return Page();
            }

            // Check for header errors
            if (string.IsNullOrEmpty(Header1) || string.IsNullOrEmpty(Header2))
            {
                ModelState.AddModelError("", "Column headers must not be empty.");
                return Page();
            }

            // Process files
            try
            {
                var uprns = _csvHelper.ReadUPRNs(File2, Header2);
                var data = _csvHelper.ExtractRows(File1, uprns, Header1);
                var csvData = _csvHelper.WriteToCSV(data);

                // Consider streaming the file directly to avoid saving it to disk
                //return File(csvData, "text/csv", "ExtractedData.csv");

                // Specify a unique file path
                string filePath = Path.Combine("temp", Guid.NewGuid().ToString() + ".csv");

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Write the file to disk
                System.IO.File.WriteAllBytes(filePath, csvData);

                // Store the file path in TempData
                TempData["FilePath"] = filePath;

                // Redirect to the success page
                return RedirectToPage("/Success");
            }
            catch (ArgumentException ex)
            {
                // If the exception is due to a missing header, inform the user which header is missing.
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception details here for further investigation
                // Logger.LogError(ex, "An error occurred while processing the files");

                // Add a generic error message or specific ones based on exception types if identifiable
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return Page();
            }
        }

    }
}
