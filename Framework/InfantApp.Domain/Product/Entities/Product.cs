using Infant.Core.Models.Ddd;
using Infant.Core.Models.Entity;

namespace InfantApp.Domain;

public class Product: FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
}