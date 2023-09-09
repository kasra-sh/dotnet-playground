using Infant.Data;
using InfantApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace InfantApp.Ef;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(InfantDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> InsertProductsAsync()
    {

        for (int i = 0; i < 1000; i++)
        {
            DbSet.Add(new Product
            {
                Name = "Abc" + Random.Shared.Next().ToString()
            });
        }
        
        await DbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteLast10Async()
    {
        await UpdateOne();
        var last10 = await DbSet
            .Where(p => p.IsDeleted == false)
            .OrderByDescending(p => p.Id)
            .Take(10).ToListAsync();
        DbSet.RemoveRange(last10);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOne()
    {
        var last1 = await DbSet
            .Where(p => p.IsDeleted == false)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();
        
        // DbSet.Update(last1);
        last1.Name = "ahcna";
        return await SaveChangesAsync() > 0;
    }
}