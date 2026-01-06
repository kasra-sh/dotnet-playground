// using System;
// using Boiler.Core.Abstractions;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Boiler.Core.Modularity.Ioc;
//
// public class Lazy<T>
// {
//     private readonly IServiceScope _serviceScope;
//     
//     public Lazy(IServiceScope serviceScope)
//     {
//         _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
//     }
//     
//     public T Value => _serviceScope.ServiceProvider.GetService<T>();
// }