using Microsoft.AspNetCore.Mvc;
using SolinerMVC.Services;

namespace SolinerMVC.Controllers;
[Route("api/[controller]")]
public class WeatherController(WeatherService weatherService) : Controller
{

    [HttpGet("{plant}")]
    public IActionResult GetWeatherData(string plant, [FromQuery] string[]? parameters = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var data = weatherService.GetWeatherDataByLocation(plant);

        if (parameters != null && parameters.Length > 0)
        {
            data = data.Where(d => parameters.Contains(d.Parameter, StringComparer.OrdinalIgnoreCase)).ToList();
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            data = data.Where(d => d.Datetime >= startDate && d.Datetime <= endDate).ToList();
        }

        if (!data.Any())
            return NotFound($"No data found for plant: {plant}");

        return Ok(data);
    }

    [HttpGet("{region}/calculate-error")]
    public IActionResult CalculateErrorMetrics(
        string region,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string parameter = "Temperature",
        [FromQuery] string metric = "MAE") // Seçilen metrik parametresi
    {
        var data = weatherService.GetWeatherDataByLocation(region);
        if (startDate.HasValue && endDate.HasValue)
        {
            data = data.Where(d => d.Datetime >= startDate && d.Datetime <= endDate).ToList();
        }
        var actual = data.Where(d => d.Parameter == parameter && d.Info == "weather")
                         .Select(d => d.Value).ToList();
        var forecast = data.Where(d => d.Parameter == parameter && d.Info == "forecast")
                           .Select(d => d.Value).Take(actual.Count).ToList();
        if (actual.Count == 0 || forecast.Count != actual.Count)
        {
            return BadRequest("The actual and forecast data must have the same length and not be empty.");
        }

        var results = new List<double>();
        for (int i = 0; i < actual.Count; i += 6) // Her 6 veriden birini al
        {
            var actualSegment = actual.Skip(i).Take(6).ToList();
            var forecastSegment = forecast.Skip(i).Take(6).ToList();
            if (actualSegment.Count == 6 && forecastSegment.Count == 6)
            {
                double result;
                switch (metric.ToUpper())
                {
                    case "MAE":
                        result = weatherService.CalculateMAE(actualSegment, forecastSegment);
                        break;
                    case "MAPE":
                        result = weatherService.CalculateMAPE(actualSegment, forecastSegment);
                        break;
                    case "RMSE":
                        result = weatherService.CalculateRMSE(actualSegment, forecastSegment);
                        break;
                    default:
                        return BadRequest("Invalid metric specified.");
                }
                results.Add(result);
            }
        }

        return Ok(new { Metric = metric, Values = results });
    }


    // 2. Veritabanından alınan veriyi işleyen endpoint
    [HttpGet("{region}/calculate-error-db")]
    public IActionResult CalculateErrorFromDb(
        string region,
        [FromQuery] string metric = "MAE",
        [FromQuery] string parameter = "Temperature")
    {
        try
        {
            if (string.IsNullOrEmpty(region) || string.IsNullOrEmpty(metric) || string.IsNullOrEmpty(parameter))
            {
                return BadRequest("Region, metric, and parameter must be provided.");
            }

            var values = weatherService.CalculateErrorFromDb(region,  parameter, metric);

            var result = new
            {
                metric = metric,
                values = values
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

}

