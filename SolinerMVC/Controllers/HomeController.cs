using Microsoft.AspNetCore.Mvc;
using SolinerMVC.Services;


namespace SolinerMVC.Controllers;
public class HomeController(CsvUploadServices csvUploadServices) : Controller
{


	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}
    [HttpPost]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Please upload a valid CSV file.");
        }

        var filePath = Path.GetTempFileName();

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        try
        {
            csvUploadServices.ImportCsvData(filePath);
            return Ok("CSV data has been successfully imported.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
