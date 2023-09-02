using Infant.Core.Models.Ddd;
using Infant.Data;
using Microsoft.EntityFrameworkCore;

namespace InfantApp.Ef;

public class TestDbContext: OutboxDbContext
{
    public TestDbContext(DbContextOptions options): base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}