using Infant.Core.Events;
using Infant.Core.Models.Ddd;
using Infant.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infant.Data;

public abstract class OutboxDbContext : DbContext
{
    public OutboxDbContext() : base()
    {
    }

    public OutboxDbContext(DbContextOptions dbContextOptions) : base(options: dbContextOptions)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var utcNow = DateTime.UtcNow;

        try
        {
            ConfigureEntries<ISoftDeletable>(
                e =>
                {
                    e.State = EntityState.Unchanged;
                    e.Entity.IsDeleted = true;
                    if (e.Entity is IHasDeletedOn entity)
                    {
                        entity.DeletedOn = utcNow;
                    }
                },
                EntityState.Deleted
            );

            ConfigureEntries<IHasCreatedOn>(
                e => e.Entity.CreatedOn = utcNow,
                EntityState.Added
            );
            ConfigureEntries<IHasUpdatedOn>(
                e => e.Entity.UpdatedOn = utcNow,
                EntityState.Modified
            );
            
            var result = await base.SaveChangesAsync(cancellationToken);
            await SendOutboxEntityEvents();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }
    
    private void ConfigureEntries<T>(Action<EntityEntry<T>> action, params EntityState[] states) where T : class
    {
        var auditEntries = ChangeTracker.Entries<T>();
        foreach (var auditEntry in auditEntries)
        {
            if (states.Length == 0 || states.Contains(auditEntry.State))
            {
                action.Invoke(auditEntry);
            }
        }
    }

    private async Task SendOutboxEntityEvents()
    {
        var outboxEntries = ChangeTracker.Entries<IOutBoxObject>();
        var allLocalEvents = new List<object>();
        var allDistributedEvents = new List<object>();
        foreach (var entityEntry in outboxEntries)
        {
            allLocalEvents.AddRange(entityEntry.Entity.GetLocalEvents());
            allDistributedEvents.AddRange(entityEntry.Entity.GetDistributedEvents());

            entityEntry.Entity.ClearLocalEvents();
            entityEntry.Entity.ClearDistributedEvents();
        }

        var tasks = new List<Task>(2);

        // var localEventBus = this.GetService<ILocalEventBus>();
        // var distributedEventBus = this.GetService<IDistributedEventBus>();

        ILocalEventBus? localEventBus = null;
        IDistributedEventBus? distributedEventBus = null;

        try
        {
            localEventBus = this.GetService<ILocalEventBus>();
        }
        catch
        {
            // ignored
        }

        try
        {
            distributedEventBus = this.GetService<IDistributedEventBus>();
        }
        catch
        {
            // ignored
        }

        if (localEventBus is not null)
        {
            tasks.Add(localEventBus.PublishMessagesAsync(allLocalEvents));
        }

        if (distributedEventBus is not null)
        {
            tasks.Add(distributedEventBus.PublishMessagesAsync(allDistributedEvents));
        }

        await Task.WhenAll(tasks);
    }
}