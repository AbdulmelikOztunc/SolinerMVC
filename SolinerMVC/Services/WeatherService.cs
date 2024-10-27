using CsvHelper;
using SolinerMVC.Maps;
using SolinerMVC.Models;
using System.Globalization;

namespace SolinerMVC.Services;

public class WeatherService
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
}