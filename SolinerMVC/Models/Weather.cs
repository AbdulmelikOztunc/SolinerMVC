namespace SolinerMVC.Models;

public class Weather
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public int ParameterId { get; set; }
    public DateTime DateTime { get; set; }
    public double? ActualValue { get; set; }
    public double? ForecastValue { get; set; }

    public Region Region { get; set; }
    public Parameter Parameter { get; set; }
}

