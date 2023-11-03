using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataExtractor.Pages
{
    public class SuccessModel : PageModel
    {
        [HttpGet("Download")]
        public IActionResult OnGetDownload()
        {
            if (TempData["FilePath"] is string filePath && System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);  // Delete the file after reading its content
                return File(fileBytes, "text/csv", "Data.csv");
            }
            else

            return NotFound();
        }
    }
}
