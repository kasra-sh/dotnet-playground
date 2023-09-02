using Castle.DynamicProxy;

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
            Console.WriteLine("before::"+invocation);
            await proceed(invocation, proceedInfo).ConfigureAwait(false);
            Console.WriteLine("after::"+invocation);
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
            Console.WriteLine("before::"+invocation);

            var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
            Console.WriteLine("after::"+invocation);

            return res;
        }
        catch (Exception ex)
        {
            // Log.Error($"Error calling {invocation.Method.Name}.", ex);
            throw;
        }
    }
}