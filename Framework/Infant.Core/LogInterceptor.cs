using Castle.DynamicProxy;
using Serilog;

namespace Infant.Core;

public class LogInterceptor : AsyncInterceptorBase, IInterceptor
{
    public LogInterceptor(IServiceProvider serviceProvider)
    {
    }
    
    public void Intercept(IInvocation invocation)
    {
        this.ToInterceptor().Intercept(invocation);
    }
    
    protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        try
        {
            // Cannot simply return the the task, as any exceptions would not be caught below.
            Log.Information("Before calling method {Method}", invocation.Method.Name);
            await proceed(invocation, proceedInfo).ConfigureAwait(false);
            Log.Information("Called method {Method}", invocation.Method.Name);
        }
        catch (Exception ex)
        {
            // Log.Error($"Error calling {invocation.Method.Name}.", ex);
            throw;
        }
    }

    protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        try
        {
            // Cannot simply return the the task, as any exceptions would not be caught below.
            Log.Information("Before calling method {Method}", proceedInfo);
            var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
            Log.Information("Called method {Method}", invocation.GetConcreteMethod());

            return res;
        }
        catch (Exception ex)
        {
            Log.Error($"Error calling {invocation.Method.Name}.", ex);
            throw;
        }
    }
}