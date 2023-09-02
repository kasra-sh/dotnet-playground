using Infant.Core.Events;
using Infant.Core.Models.Ddd;
using Infant.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
        try
        {
            SoftDeleteEntities();
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

    private void SoftDeleteEntities()
    {
        var softDeleteEntries = ChangeTracker.Entries<ISoftDeletable>();
        foreach (var softDeleteEntry in softDeleteEntries)
        {
            if (softDeleteEntry.State == EntityState.Deleted)
            {
                if (!softDeleteEntry.Entity.IsDeleted)
                {
                    softDeleteEntry.Entity.IsDeleted = true;
                    softDeleteEntry.State = EntityState.Modified;
                }
            }
        }
    }

    private async Task SendOutboxEntityEvents()
    {
        var outboxEntries = ChangeTracker.Entries<IOutboxEntity>();
        var allLocalEvents = new List<object>();
        var allDistributedEvents = new List<object>();
        foreach (var entityEntry in outboxEntries)
        {
            allLocalEvents.AddRange(entityEntry.Entity.GetLocalEvents());
            allDistributedEvents.AddRange(entityEntry.Entity.GetDistributedEvents());

            entityEntry.Entity.ClearLocalEvents();
            entityEntry.Entity.ClearDistributedEvents();
        }

        await Task.WhenAll(
            this.GetService<ILocalEventBus>().PublishMessagesAsync(allLocalEvents),
            this.GetService<IDistributedEventBus>().PublishMessagesAsync(allDistributedEvents)
        );
    }
}