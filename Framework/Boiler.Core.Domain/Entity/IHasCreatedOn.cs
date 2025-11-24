using System;

namespace Boiler.Core.Domain.Entity;

public interface IHasCreatedOn
{
    public DateTime CreatedOn { get; set; }
}