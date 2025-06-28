using System.Text;
using Infant.Core;
using Infant.Core.Abstractions;
using Infant.Core.Events;
using Infant.Core.Modularity;
using Infant.Host;
using Infant.Host.AutoApi;
using InfantApp.Domain.Eto;
using InfantApp.Ef;
using InfantApp.WebApi.Host;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;


var webAppBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
webAppBuilder.Logging.ClearProviders().AddSerilog();

var configuration = webAppBuilder.Configuration;
webAppBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

webAppBuilder.Services.AddTransient<IApiDescriptionProvider, ApiDescProvider>();

await webAppBuilder.AddApplicationAsync<InfantAppWebApiHostModule>(new WebApplicationSettings
{
    Interceptors = new[] { typeof(LogInterceptor) }
});


webAppBuilder.Services.AddControllers()
    .AddConventionalControllers()
    .AddControllersAsServices();

if (webAppBuilder.Environment.IsDevelopment())
{
    webAppBuilder.Services.AddDevelopmentSwagger();
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

app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { });
    app.UseSwaggerUI();
}

await app.InitializeApplicationAsync();


await app.Services.GetService<ILocalEventBus>().Subscribe<ProductCreatedEto>((ev) => Console.WriteLine(ev.Name));


app.Run();