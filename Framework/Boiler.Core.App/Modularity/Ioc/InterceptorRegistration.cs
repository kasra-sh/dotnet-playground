using System;

namespace Boiler.Core.Modularity.Ioc;

public class InterceptorRegistration
{
    public Type Interceptor { get; set; }
    public Type[] TargetTypes { get; set; }
}