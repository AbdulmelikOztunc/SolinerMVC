using CsvHelper.Configuration;
using SolinerMVC.DTOs;

namespace SolinerMVC.Maps;

public class WeatherDataMap : ClassMap<WeatherData>
{
	public WeatherDataMap()
	{
		Map(m => m.Plant).Name("plant");
		Map(m => m.Parameter).Name("parameter");
		Map(m => m.Value).Name("value");
		Map(m => m.Info).Name("info");
		Map(m => m.Datetime).Name("datetime");
	}
}
