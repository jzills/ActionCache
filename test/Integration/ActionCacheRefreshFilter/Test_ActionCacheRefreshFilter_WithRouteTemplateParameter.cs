using ActionCache;
using ActionCache.Common.Enums;
using ActionCache.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class Test_ActionCacheRefreshFilter_WithRouteTemplateParameter
{
    TestServer Server;
    HttpClient Client;
    Guid AccountId = Guid.NewGuid();

    [SetUp]
    public void Setup()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => 
            {
                services.AddMvc();
                services.AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379");
            })
            .Configure(app =>
            {
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseEndpoints(options => options.MapControllers());
            });

        Server = new TestServer(builder);
        Client = Server.CreateClient();
    }

    [Test]
    public async Task Test()
    {
        var teamIds = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid());
        var teamTasks = teamIds.Select(teamId => Client.GetAsync($"{AccountId}/teams/{teamId}"));
        await Task.WhenAll(teamTasks);

        var cacheFactory = Server.Services.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create($"Teams:{AccountId}");
        var keys = await cache.GetKeysAsync();
        Assert.That(keys.Count(), Is.EqualTo(10));
    }

    [TearDown]
    public async Task TearDown()
    {
        var cacheFactory = Server.Services.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create($"Teams:{AccountId}");
        await cache!.RemoveAsync();
    }
}