using SolinerMVC.DTOs;

namespace SolinerMVC.Models;

public class Parameter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Weather> Weather { get; set; } = new List<Weather>();
}

