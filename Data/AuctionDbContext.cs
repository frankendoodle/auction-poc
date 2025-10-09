using Microsoft.EntityFrameworkCore;
using AuctionPoc.Models;

namespace AuctionPoc.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuctionProperty> AuctionProperties { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuctionProperty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(2);
            entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.PropertyType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StartingBid).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Zoning).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });
    }
}
