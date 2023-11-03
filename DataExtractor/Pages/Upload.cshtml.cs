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
            if (!ModelState.IsValid)
            {
                if (File1 == null || File2 == null)
                {
                    TempData["ErrorMessage"] = "Both files must be uploaded.";
                    return RedirectToPage("/Error");
                }
                // Check if column headers are provided
                if (string.IsNullOrEmpty(Header1) || string.IsNullOrEmpty(Header2))
                {
                    TempData["ErrorMessage"] = "Column header is missing. Please enter a column header.";
                    return RedirectToPage("/Error");
                }

                return Page();
            }
            // Check if files are empty
            if (File1.Length == 0 || File2.Length == 0)
            {
                TempData["ErrorMessage"] = "File is empty. Please upload a non-empty file.";
                return RedirectToPage("/Error");
            }
            // Check if files are CSV
            if (Path.GetExtension(File1.FileName).ToLower() != ".csv" || Path.GetExtension(File2.FileName).ToLower() != ".csv")
            {
                TempData["ErrorMessage"] = "Invalid file format. Please upload a CSV file.";
                return RedirectToPage("/Error");
            }
            

            try
            {
                var uprns = _csvHelper.ReadUPRNs(File2, Header2);
                var data = _csvHelper.ExtractRows(File1, uprns, Header1);
                var csvData = _csvHelper.WriteToCSV(data);

                //return File(csvData, "text/csv", "Data.csv");

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
            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Error");
            }
        }

    }
}
