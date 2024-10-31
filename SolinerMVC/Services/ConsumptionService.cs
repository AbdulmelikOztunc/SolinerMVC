using Microsoft.EntityFrameworkCore;
using SolinerMVC.Data;
using SolinerMVC.DTOs;
using SolinerMVC.Enums;
using SolinerMVC.Models;

namespace SolinerMVC.Services;

public class ConsumptionService(ApplicationDbContext context)
{
    // İlk yüklemede consumption ve official verilerini getiren metod
    public async Task<List<DailyConsumptionData>> GetInitialDailyDataAsync()
    {
        return await context.ConsumptionData
            .Where(d => d.Type == ConsumptionType.Consumption || d.Type == ConsumptionType.Official)
            .GroupBy(d => d.DateTime.Date) // Tarihe göre gruplama
            .Select(g => new DailyConsumptionData
            {
                Date = g.Key,
                ConsumptionValue = g.Where(x => x.Type == ConsumptionType.Consumption)
                                    .Sum(x => x.Value), // Günlük toplam tüketim
                OfficialValue = g.Where(x => x.Type == ConsumptionType.Official)
                                 .Sum(x => x.Value) // Günlük toplam resmi tahmin
            })
            .OrderBy(d => d.Date)
            .ToListAsync();
    }

    // Kullanıcının seçimine göre verileri getirir
    public async Task<List<DailyConsumptionData>> GetProductDataAsync()
    {
        return await context.ConsumptionData
            .Where(d => d.Type == ConsumptionType.Prod_1 || d.Type == ConsumptionType.Prod_2)
            .GroupBy(d => d.DateTime.Date) // Tarihe göre gruplama
            .Select(g => new DailyConsumptionData
            {
                Date = g.Key,
                Prod_1Value = g.Where(x => x.Type == ConsumptionType.Prod_1)
                                    .Sum(x => x.Value), // Günlük toplam tüketim
                Prod_2Value = g.Where(x => x.Type == ConsumptionType.Prod_2)
                                 .Sum(x => x.Value) // Günlük toplam resmi tahmin
            })
            .OrderBy(d => d.Date)
            .ToListAsync();
    }




    // Son 7 günün günlük hata hesaplaması (MAE, MAPE, RMSE)
    public async Task<List<DailyErrorData>> GetLastWeekErrorDataAsync(string metric)
    {
        var referenceDate = new DateTime(2024, 10, 22);
        var oneWeekAgo = referenceDate.AddDays(-7);

        var groupedData = await context.ConsumptionData
            .Where(d => d.Type == ConsumptionType.Consumption || d.Type == ConsumptionType.Official)
            .Where(d => d.DateTime >= oneWeekAgo) // Sadece son 7 gün
            .Where(d => d.DateTime <= referenceDate) // Sadece son 7 gün
            .GroupBy(d => d.DateTime.Date) // Günlük gruplama
            .Select(g => new
            {
                Date = g.Key,
                Consumption = g.Where(x => x.Type == ConsumptionType.Consumption).Select(x => x.Value).ToList(),
                Official = g.Where(x => x.Type == ConsumptionType.Official).Select(x => x.Value).ToList()
            })
            .ToListAsync();

        var errorData = groupedData.Select(g => new DailyErrorData
        {
            Date = g.Date,
            ErrorValue = CalculateError(g.Consumption, g.Official, metric) // Hata hesabı
        }).ToList();

        return errorData;
    }

    // Hata hesaplama metodu
    private double CalculateError(List<double> consumption, List<double> official, string metric)
    {
        if (!consumption.Any() || !official.Any() || consumption.Count != official.Count)
            return 0; // Eksik veya uyumsuz veri durumunda varsayılan hata

        if (metric == "MAE")
            return consumption.Zip(official, (c, o) => Math.Abs(c - o)).Average();

        if (metric == "MAPE")
        {
            var validData = consumption.Zip(official, (c, o) => c != 0 ? Math.Abs((c - o) / c) : (double?)null)
                                       .Where(x => x.HasValue)
                                       .Select(x => x.Value);
            return validData.Any() ? validData.Average() * 100 : 0;
        }

        if (metric == "RMSE")
            return Math.Sqrt(consumption.Zip(official, (c, o) => Math.Pow(c - o, 2)).Average());

        return 0; // Varsayılan MAE
    }

    // Yıllık hata hesaplama (MAE, MAPE, RMSE)
    //public async Task<List<YearlyErrorData>> GetYearlyErrorDataAsync(string metric)
    //{
    //    var groupedData = await context.ConsumptionData
    //        .Where(d => d.Type == ConsumptionType.Consumption || d.Type == ConsumptionType.Official)
    //        .GroupBy(d => d.DateTime.Year) // Yıllık gruplama
    //        .Select(g => new
    //        {
    //            Year = g.Key,
    //            Consumption = g.Where(x => x.Type == ConsumptionType.Consumption).Select(x => x.Value).ToList(),
    //            Official = g.Where(x => x.Type == ConsumptionType.Official).Select(x => x.Value).ToList()
    //        })
    //        .ToListAsync();

    //    var errorData = groupedData.Select(g => new YearlyErrorData
    //    {
    //        Year = g.Year,
    //        ErrorValue = CalculateError(g.Consumption, g.Official, metric) // Hata hesabı
    //    }).ToList();

    //    return errorData;
    //}

    // Hata hesaplama metodu
    //private double CalculateError(List<double> consumption, List<double> official, string metric)
    //{
    //    if (!consumption.Any() || !official.Any() || consumption.Count != official.Count)
    //        return 0; // Eksik veya uyumsuz veri durumunda varsayılan hata

    //    if (metric == "MAE")
    //        return consumption.Zip(official, (c, o) => Math.Abs(c - o)).Average();

    //    if (metric == "MAPE")
    //    {
    //        var validData = consumption.Zip(official, (c, o) => c != 0 ? Math.Abs((c - o) / c) : (double?)null)
    //                                   .Where(x => x.HasValue)
    //                                   .Select(x => x.Value);
    //        return validData.Any() ? validData.Average() * 100 : 0;
    //    }

    //    if (metric == "RMSE")
    //        return Math.Sqrt(consumption.Zip(official, (c, o) => Math.Pow(c - o, 2)).Average());

    //    return 0; // Varsayılan MAE
    //}

    //// Aylık hata hesaplama (MAE, MAPE, RMSE)
    //public async Task<List<MonthlyErrorData>> GetMonthlyErrorDataAsync(string metric)
    //{
    //    var groupedData = await context.ConsumptionData
    //        .Where(d => d.Type == ConsumptionType.Consumption || d.Type == ConsumptionType.Official)
    //        .GroupBy(d => new { d.DateTime.Year, d.DateTime.Month }) // Aylık gruplama
    //        .Select(g => new
    //        {
    //            Month = new DateTime(g.Key.Year, g.Key.Month, 1),
    //            Consumption = g.Where(x => x.Type == ConsumptionType.Consumption).Select(x => x.Value).ToList(),
    //            Official = g.Where(x => x.Type == ConsumptionType.Official).Select(x => x.Value).ToList()
    //        })
    //        .ToListAsync();

    //    var errorData = groupedData.Select(g => new MonthlyErrorData
    //    {
    //        Month = g.Month,
    //        ErrorValue = CalculateError(g.Consumption, g.Official, metric) // Hata hesabı
    //    }).ToList();

    //    return errorData;
    //}

    //private double CalculateError(List<double> consumption, List<double> official, string metric)
    //{
    //    if (!consumption.Any() || !official.Any() || consumption.Count != official.Count)
    //        return 0; // Eksik veya uyumsuz veri durumunda varsayılan hata

    //    if (metric == "MAE")
    //        return consumption.Zip(official, (c, o) => Math.Abs(c - o)).Average();

    //    if (metric == "MAPE")
    //    {
    //        var validData = consumption.Zip(official, (c, o) => c != 0 ? Math.Abs((c - o) / c) : (double?)null)
    //                                   .Where(x => x.HasValue)
    //                                   .Select(x => x.Value);
    //        return validData.Any() ? validData.Average() * 100 : 0;
    //    }

    //    if (metric == "RMSE")
    //        return Math.Sqrt(consumption.Zip(official, (c, o) => Math.Pow(c - o, 2)).Average());

    //    return 0; // Varsayılan MAE
    //}


    // İlk yüklemede consumption ve official verilerini getiren metod
    //internal async Task<List<ConsumptionData>> GetInitialDataAsync()
    //{
    //    return await context.ConsumptionData
    //.Where(d => d.Type == ConsumptionType.Consumption ||
    //            d.Type == ConsumptionType.Official)
    //.OrderBy(d => d.DateTime)  // Veriyi tarih sırasına göre sıralıyoruz
    //.ToListAsync();
    //}

}
