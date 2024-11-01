﻿using CsvHelper.Configuration;
using CsvHelper;
using SolinerMVC.DTOs;
using SolinerMVC.Models;
using System.Globalization;
using SolinerMVC.Data;
using Microsoft.EntityFrameworkCore;
using SolinerMVC.Enums;


namespace SolinerMVC.Services;

public class CsvUploadServices(ApplicationDbContext context)
{
    public void ImportCsvData(string filePath)  // hava tahminleri için kullancım
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null, // Header doğrulamasını kapat
            MissingFieldFound = null // Eksik alan bulunursa hata verme
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<CsvWeatherData>().ToList();
            SaveToDatabase(records);
        }
    }

    public void SaveToDatabase(List<CsvWeatherData> records) // hava tahminlerinde kullandım
    {
        foreach (var record in records)
        {
            try
            {
                // Null or empty value check for 'record'
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(record), "Record is null");
                }

                var region = context.Regions.SingleOrDefault(r => r.Name == record.Plant);
                if (region == null)
                {
                    region = new Region { Name = record.Plant };
                    context.Regions.Add(region);
                    context.SaveChanges(); // Save to generate Region.Id
                }

                var parameter = context.Parameters.SingleOrDefault(p => p.Name == record.Parameter);
                if (parameter == null)
                {
                    parameter = new Parameter { Name = record.Parameter };
                    context.Parameters.Add(parameter);
                    context.SaveChanges(); // Save to generate Parameter.Id
                }

                var weather = context.Weathers
                    .SingleOrDefault(w => w.RegionId == region.Id && w.ParameterId == parameter.Id && w.DateTime == record.DateTime);

                if (weather == null)
                {
                    weather = new Weather
                    {
                        RegionId = region.Id,
                        ParameterId = parameter.Id,
                        DateTime = record.DateTime
                    };
                    context.Weathers.Add(weather);
                }

                // Handle forecast and actual values safely
                if (record.Info == "forecast")
                {
                    weather.ForecastValue = record.Value;
                }
                else if (record.Info == "weather")
                {
                    weather.ActualValue = record.Value;
                }

            }
            catch (Exception ex)
            {
                // Log the error or throw with specific information
                throw new Exception($"Error processing record: {record?.Plant}, {record?.Parameter}, {record?.DateTime}. Details: {ex.Message}", ex);
            }
        }
        context.SaveChanges();
    }
    public void LoadCsvData(string csvFilePath)
    {
        using (var reader = new StreamReader(csvFilePath))
        {
            // İlk satır başlık satırıysa atla
            var header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                try
                {
                        var data = new ConsumptionData
                    {
                        Value = double.Parse(values[0], CultureInfo.InvariantCulture),
                        DateTime = DateTime.Parse(values[1]),
                        Type = Enum.Parse<ConsumptionType>(values[2], true)
                    };

                    context.ConsumptionData.Add(data);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Hatalı veri satırı: {line}. Hata: {ex.Message}");
                }
            }
        }
        context.SaveChanges();
    }
}
