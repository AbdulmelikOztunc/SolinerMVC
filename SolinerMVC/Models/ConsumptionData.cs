using SolinerMVC.Enums;

namespace SolinerMVC.Models;

public class ConsumptionData
{
    public int Id { get; set; } // Primary Key
    public double Value { get; set; } // Tüketim veya Tahmin Değeri
    public DateTime DateTime { get; set; } // Tarih ve Saat Bilgisi

    // Enum Türü ile Verinin Tipi
    public ConsumptionType Type { get; set; }
}

