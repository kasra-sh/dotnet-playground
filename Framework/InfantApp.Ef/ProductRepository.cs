using Infant.Data;
using Infant.Data.EntityFrameworkCore;
using InfantApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace InfantApp.Ef;

public class ProductRepository : EfCrudRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(InfantDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> InsertProductsAsync()
    {
        for (int i = 0; i < 1000; i++)
        {
            GetDbSet().Add(new Product
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
        var last10 = await GetQueryable()
            .Where(p => p.IsDeleted == false)
            .OrderByDescending(p => p.Id)
            .Take(10).ToListAsync();
        GetDbSet().RemoveRange(last10);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOne()
    {
        var last1 = await GetQueryable()
            .Where(p => p.IsDeleted == false)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        last1.Name = "updatedName" + Random.Shared.Next(10000, 99999);
        return await SaveChangesAsync() > 0;
    }
}