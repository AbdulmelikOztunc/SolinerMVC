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

    public double CalculateMAE(List<double> actual, List<double> forecast)
	{
		return actual.Zip(forecast, (a, f) => Math.Abs(a - f)).Average();
	}

	public double CalculateMAPE(List<double> actual, List<double> forecast)
	{
		return actual.Zip(forecast, (a, f) => Math.Abs((a - f) / a)).Average() * 100;
	}

	public double CalculateRMSE(List<double> actual, List<double> forecast)
	{
		return Math.Sqrt(actual.Zip(forecast, (a, f) => Math.Pow(a - f, 2)).Average());
	}

}

