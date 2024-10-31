using Microsoft.AspNetCore.Mvc;
using SolinerMVC.Enums;
using SolinerMVC.Models;
using SolinerMVC.Services;

namespace SolinerMVC.Controllers;
public class ConsumptionController(ConsumptionService consumptionService) : Controller
{
    
    public async Task<IActionResult> IndexAsync()
    {
        var data = await consumptionService.GetInitialDailyDataAsync();
        var model = new ConsumptionViewModel
        {
            Labels = data.Select(d => d.Date.ToString("d")).ToList(),  // Günlük etiketler
            ConsumptionValues = data.Select(d => d.ConsumptionValue).ToList(),
            OfficialValues = data.Select(d => d.OfficialValue).ToList()
        };
        //var data = await consumptionService.GetInitialDataAsync();

        //var model = new ConsumptionViewModel
        //{
        //    Labels = data.Select(d => d.DateTime.ToString("g")).ToList(),  // Tarih etiketleri
        //    ConsumptionValues = data.Where(d => d.Type == Enums.ConsumptionType.Consumption)
        //                             .Select(d => d.Value).ToList(),
        //    OfficialValues = data.Where(d => d.Type == Enums.ConsumptionType.Official)
        //                         .Select(d => d.Value).ToList()
        //};

        return View(model);
    }

    // Verileri filtreleyip JSON formatında döner
    [HttpGet]
    public async Task<IActionResult> GetProductData()
    {
        try
        {
            var data = await consumptionService.GetProductDataAsync();

            // Günlük etiketler ve değerleri oluştur
            var model = new
            {
                Labels = data.Select(d => d.Date.ToString("d")).ToList(),  // Günlük etiketler
                Prod1Values = data.Select(d => d.Prod_1Value).ToList(),
                Prod2Values = data.Select(d => d.Prod_2Value).ToList()
            };

            return Json(model);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Veri getirilemedi: {ex.Message}");
        }
    }




    [HttpGet]
    public async Task<IActionResult> GetMonthlyErrorData(string metric = "MAE")
    {
        try
        {
            var data = await consumptionService.GetLastWeekErrorDataAsync(metric);
            return Json(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata hesaplanamadı: {ex.Message}");
        }
    }

}
