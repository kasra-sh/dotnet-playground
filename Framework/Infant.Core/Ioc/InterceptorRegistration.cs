namespace Infant.Core.Ioc;

public class InterceptorRegistration
{
    public Type Interceptor { get; set; }
    public Type[] TargetTypes { get; set; }
}