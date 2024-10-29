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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Weather>()
            .HasIndex(w => new { w.RegionId, w.ParameterId, w.DateTime });
            

        base.OnModelCreating(modelBuilder);
    }
}
