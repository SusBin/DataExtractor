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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uprns = _csvHelper.ReadUPRNs(File2);
            var data = _csvHelper.ExtractRows(File1, uprns);
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
        }

    }
}
