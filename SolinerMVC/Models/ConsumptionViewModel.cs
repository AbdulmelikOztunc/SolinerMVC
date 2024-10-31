namespace SolinerMVC.Models;

public class ConsumptionViewModel
{
    public List<string>? Labels { get; set; } = new();
    public List<double>? ConsumptionValues { get; set; } = new();
    public List<double>? OfficialValues { get; set; } = new();
    public List<double>? Prod1Values { get; set; } = new();
    public List<double>? Prod2Values { get; set; } = new();
}
