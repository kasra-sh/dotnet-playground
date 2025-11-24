using System;

namespace Boiler.Core.Domain.Entity;

public interface IHasDeletedOn
{
    public DateTime? DeletedOn { get; set; }
}