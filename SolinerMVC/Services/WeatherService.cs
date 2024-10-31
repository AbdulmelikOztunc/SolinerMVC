using CsvHelper;
using Microsoft.EntityFrameworkCore;
using SolinerMVC.Data;
using SolinerMVC.DTOs;
using SolinerMVC.Maps;
using System.Globalization;

namespace SolinerMVC.Services;

public class WeatherService(ApplicationDbContext context)
{
	public List<WeatherData> GetWeatherDataByLocation(string plant)
	{
		string filePath = "C:/Users/asus/source/repos/Soliner/Weather_Table.csv"; // CSV dosyasının yolu

		using var reader = new StreamReader(filePath);
		using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

		// ClassMap kaydını yapıyoruz.
		csv.Context.RegisterClassMap<WeatherDataMap>();

		var records = csv.GetRecords<WeatherData>().ToList();

		// Belirli bir yer (plant) için veriyi filtreliyoruz.
		return records.Where(r => r.Plant.Equals(plant, StringComparison.OrdinalIgnoreCase)).ToList();
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

    // Veritabanından hata hesaplaması yapan metod
    public List<double> CalculateErrorFromDb(string region, string parameter, string metric)
    {
        var regionId= context.Regions.Where(r=>r.Name==region).FirstOrDefault().Id;
        var parameterId = context.Parameters.Where(r=>r.Name==parameter).FirstOrDefault().Id;

        // 1. İlgili bölge ve parametre için veriyi çek
        var data = context.Weathers
            .Where(w => w.RegionId == regionId && w.ParameterId == parameterId)
            .OrderBy(w => w.DateTime)
            .ToList();

        // 2. Actual ve Forecast verilerini ayır
        var actual = data.Where(d => d.ActualValue.HasValue)
                         .Select(d => d.ActualValue.Value).ToList();

        var forecast = data.Where(d => d.ForecastValue.HasValue)
                           .Select(d => d.ForecastValue.Value)
                           .Take(actual.Count).ToList();

        if (actual.Count == 0 || forecast.Count != actual.Count)
        {
            throw new Exception("The actual and forecast data must have the same length and not be empty.");
        }

        // 3. Her 6 saatlik dilimde metrik hesaplaması yap
        var results = new List<double>();
        for (int i = 0; i < actual.Count; i += 6)
        {
            var actualSegment = actual.Skip(i).Take(6).ToList();
            var forecastSegment = forecast.Skip(i).Take(6).ToList();

            if (actualSegment.Count == 6 && forecastSegment.Count == 6)
            {
                double result;
                switch (metric.ToUpper())
                {
                    case "MAE":
                        result = CalculateMAE(actualSegment, forecastSegment);
                        break;
                    case "MAPE":
                        result = CalculateMAPE(actualSegment, forecastSegment);
                        break;
                    case "RMSE":
                        result = CalculateRMSE(actualSegment, forecastSegment);
                        break;
                    default:
                        throw new Exception("Invalid metric specified.");
                }
                results.Add(result);
            }
        }

        return results;
    }
}