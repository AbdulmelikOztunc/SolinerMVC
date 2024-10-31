using Microsoft.EntityFrameworkCore;
using SolinerMVC.Models;

namespace SolinerMVC.Data;

public class ApplicationDbContext : DbContext
{


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Parameter> Parameters { get; set; }
    public DbSet<Weather> Weathers { get; set; }
    public DbSet<ConsumptionData> ConsumptionData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Weather>()
            .HasIndex(w => new { w.RegionId, w.ParameterId, w.DateTime });
        modelBuilder.Entity<ConsumptionData>()
            .Property(e => e.Type)
            .HasConversion<string>();// Enum'u string olarak saklar

        base.OnModelCreating(modelBuilder);
    }
}
