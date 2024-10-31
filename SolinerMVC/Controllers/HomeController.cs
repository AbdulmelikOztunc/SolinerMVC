using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolinerMVC.Enums;
using SolinerMVC.Models;
using SolinerMVC.Services;
using System.Globalization;


namespace SolinerMVC.Controllers;
public class HomeController(CsvUploadServices csvUploadServices) : Controller
{


	public IActionResult Index()
	{
		return View();
	}

    [HttpPost] // hava tahminleri için
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
    [HttpGet] // tüketim tahminleri için
    public Task<IActionResult> LoadCsv()
    {
        var csvFilePath = "C:\\Users\\asus\\Downloads\\Consumption_Table.csv"; // CSV dosya yolunu belirtin

        try
        {
            csvUploadServices.LoadCsvData(csvFilePath);
            return Task.FromResult<IActionResult>(Ok("Veriler baþarýyla yüklendi."));
        }
        catch (Exception ex)
        {
            return Task.FromResult<IActionResult>(BadRequest($"Veri yükleme hatasý: {ex.Message}"));
        }
    }
}
