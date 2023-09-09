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
    private readonly ApplicationManager _applicationManager;

    public WeatherForecastController(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<WeatherForecastController>>();
        _productManager = serviceProvider.GetRequiredService<ProductManager>();
        _applicationManager = serviceProvider.GetRequiredService<ApplicationManager>();
    }

    [HttpGet("InsertProducts")]
    public async Task<IActionResult> InsertProducts()
    {
        await _productManager.WriteLine();
        return Ok();
    }

    [HttpDelete("DeleteLast10")]
    public async Task<IActionResult> DeleteLast10Products()
    {
        await _productManager.DeleteLast10();
        return Ok();
    }
}