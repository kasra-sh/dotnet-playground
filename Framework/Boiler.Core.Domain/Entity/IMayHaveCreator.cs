using System;

namespace Boiler.Core.Domain.Entity;

public interface IMayHaveCreator
{
    public Guid? CreatorId { get; set; }
}