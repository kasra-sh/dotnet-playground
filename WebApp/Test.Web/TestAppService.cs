using Boiler.Core.Modularity;
using Boiler.Core.Modularity.Attributes;

namespace Test.Web;

[ApiVersion(1)]
public class TestAppService: AppService
{

    public TestAppService()
    {
        
    }

    public async Task<string> CreateTest(string s)
    {
        return await Task.FromResult(s);
    }
}