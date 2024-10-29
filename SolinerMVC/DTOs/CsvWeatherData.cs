namespace SolinerMVC.DTOs;
using CsvHelper.Configuration.Attributes;

// bu sınıf csvden aldığım verileri database taşımak için
public class CsvWeatherData
{
    [Name("plant")]
    public string Plant { get; set; }

    [Name("parameter")]
    public string Parameter { get; set; }

    [Name("value")]
    public double Value { get; set; }

    [Name("info")]
    public string Info { get; set; }

    [Name("datetime")]
    public DateTime DateTime { get; set; }
}

