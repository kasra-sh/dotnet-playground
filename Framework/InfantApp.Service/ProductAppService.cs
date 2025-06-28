using Infant.Core.Abstractions;
using Serilog;

namespace InfantApp.Service;

public class ProductAppService : AppService/*, IProductAppService*/
{
    // public ProductAppService(IServiceProvider serviceProvider) /*: base(serviceProvider)*/
    // {
    // }

    public async Task<string> GetDoSome(IServiceProvider serviceProvider)
    {
        Log.Warning(CurrentUser?.Identity?.Name ?? "no user!");
        return CurrentUser?.Identity?.Name;
    }

    public async Task<CreateSomethingDto> CreateSomething()
    {
        Log.Warning(CurrentUser?.Identity?.Name ?? "no user!");
        return new (){
            Id = Guid.NewGuid()
        };
    }
}

public class CreateSomethingDto
{
    public string Name { get; set; }
    public Guid Id { get; set; }
}