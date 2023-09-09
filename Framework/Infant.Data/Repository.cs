using Microsoft.EntityFrameworkCore;

namespace Infant.Data;

public abstract class Repository<T> where T: class
{
    private readonly DbContext _dbContext;
    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected DbContext DbContext => _dbContext;

    protected DbSet<T> DbSet => _dbContext.Set<T>();

    protected async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}