using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infant.Core;
using Infant.Core.Modularity;
using Infant.Host;
using InfantApp.Ef;
using InfantApp.WebApi.Host;
using Microsoft.EntityFrameworkCore;


var webAppBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.

webAppBuilder.Services.AddDbContext<InfantDbContext>(options =>
{
    options.UseSqlServer(webAppBuilder.Configuration.GetConnectionString("InfantDb"));
    options.AddInterceptors();
}, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);


await webAppBuilder.AddApplicationAsync<InfantAppWebApiHostModule>();

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
// app.UseResponseCompression();
await app.InitializeApplicationAsync();

app.Run();
