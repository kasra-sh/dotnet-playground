using System;

namespace Boiler.Core.Domain.Entity;

public interface IMayHaveTenant
{
    public Guid? TenantId { get; set; }
}