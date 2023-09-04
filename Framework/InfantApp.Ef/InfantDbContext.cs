using Infant.Core.Models.Ddd;
using Infant.Data;
using InfantApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace InfantApp.Ef;

public class InfantDbContext: OutboxDbContext
{
    // private DbSet<Product> Products;
    public InfantDbContext(DbContextOptions options): base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Product>()
            // .HasKey(p => p.Id);
        base.OnModelCreating(modelBuilder);
    }
    
}