using System;

namespace Boiler.Core.Domain.Entity;

public interface IMayHaveUpdater
{
    public Guid? UpdaterId { get; set; }
}