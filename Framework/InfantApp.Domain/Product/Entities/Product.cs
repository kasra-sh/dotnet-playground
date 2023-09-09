using Infant.Core.Models.Ddd;
using Infant.Core.Models.Entity;
using InfantApp.Domain.Eto;

namespace InfantApp.Domain;

public class Product: FullAuditedAggregateRoot<Guid>
{
    public Product()
    {
        
    }

    private string _name;
    public virtual string Name
    {
        set
        {
            _name = value;
            AddLocalEvent(new ProductCreatedEto()
            {
                Name = _name
            });
        }
        get
        {
            return _name;
        }
    }
}