using Boiler.Core.Modularity.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Test.Web.Controllers;

[Route("api/v1/test/")]
public class TestController: Controller
{
    [HttpPost("create-test")]
    public async Task<IActionResult> CreateTest()
    {
        return await Task.FromResult(Ok());
    }
}