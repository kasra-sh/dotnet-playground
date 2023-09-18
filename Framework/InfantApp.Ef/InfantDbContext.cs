using Infant.Data;
using Infant.Data.EntityFrameworkCore;
using InfantApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace InfantApp.Ef;

public class InfantDbContext: OutboxDbContext
{
    // private DbSet<Product> Products;
    public InfantDbContext(DbContextOptions options): base(options)
    {
    }

}