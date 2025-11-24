using System;

namespace Boiler.Core.Domain.Entity;

public interface IHasUpdatedOn
{
    public DateTime? UpdatedOn { get; set; }
}