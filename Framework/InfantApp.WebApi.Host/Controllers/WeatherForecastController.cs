using Infant.Core.DI;
using Infant.Core.Modularity;
using InfantApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InfantApp.WebApi.Host.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ProductManager _productManager;
    private readonly ICollection<ITransientDependency> _transientDependencies;
    private readonly ApplicationManager _applicationManager;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ProductManager productManager, ApplicationManager applicationManager, ICollection<ITransientDependency> transientDependencies)
    {
        _logger = logger;
        _productManager = productManager;
        _applicationManager = applicationManager;
        _transientDependencies = transientDependencies;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        await _productManager.WriteLine();
        // Console.WriteLine(_productManager);
        // Console.WriteLine(_applicationManager);
        return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray());
    }
}