using GeoAsset.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoAsset.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<InspectionLog> InspectionLogs => Set<InspectionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InspectionLog>()
            .HasOne(il => il.Asset)
            .WithMany(a => a.InspectionLogs)
            .HasForeignKey(il => il.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
