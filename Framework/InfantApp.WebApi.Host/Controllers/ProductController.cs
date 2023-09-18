using Infant.Core.Models.Domain.Shared;
using Infant.Core.Modularity;
using InfantApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InfantApp.WebApi.Host.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ProductController> _logger;
    private readonly ProductManager _productManager;
    private readonly AppModuleManager _appModuleManager;

    public ProductController(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<ProductController>>();
        _productManager = serviceProvider.GetRequiredService<ProductManager>();
        _appModuleManager = serviceProvider.GetRequiredService<AppModuleManager>();
    }

    [HttpGet("InsertProducts")]
    public async Task<ActionResult<int>> InsertProducts()
    {
        return Ok(await _productManager.WriteLine());
    }

    [HttpDelete("DeleteLast10")]
    public async Task<IActionResult> DeleteLast10Products()
    {
        await _productManager.DeleteLast10();
        return Ok();
    }

    [HttpGet("GetPagedProducts")]
    public async Task<ActionResult<PagedResultDto<Product>>> GetPagedProducts(int size, int num, string sort)
    {
        return Ok(await _productManager.GetPaged(size, num, sort));
    }
}