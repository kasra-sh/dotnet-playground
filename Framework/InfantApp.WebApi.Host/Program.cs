using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infant.Core;
using Infant.Core.Events;
using Infant.Core.Modularity;
using Infant.Host;
using InfantApp.Domain.Eto;
using InfantApp.Ef;
using InfantApp.WebApi.Host;
using Microsoft.EntityFrameworkCore;


var webAppBuilder = WebApplication.CreateBuilder(args);
await webAppBuilder.AddApplicationAsync<InfantAppWebApiHostModule>();

// Add services to the container.

webAppBuilder.Services.AddControllers();

if (webAppBuilder.Environment.IsDevelopment())
{
    webAppBuilder.Services.AddEndpointsApiExplorer();
    webAppBuilder.Services.AddSwaggerGen();
}


var app = webAppBuilder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<InfantDbContext>();

    // Here is the migration executed
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.InitializeApplicationAsync();


await app.Services.GetService<ILocalEventBus>().Subscribe<ProductCreatedEto>((ev) => Console.WriteLine(ev.Name));
app.Run();
