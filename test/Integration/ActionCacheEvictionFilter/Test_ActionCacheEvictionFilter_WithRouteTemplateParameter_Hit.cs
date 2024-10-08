using ActionCache;
using ActionCache.Common.Enums;
using ActionCache.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class Test_ActionCacheEvictionFilter_WithRouteTemplateParameter
{
    TestServer Server;
    HttpClient Client;

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
        var route = $"/teams/{Guid.NewGuid()}";
        var response = await Client.GetAsync(route);
        response.EnsureSuccessStatusCode();

        response = await Client.DeleteAsync(route);
        response.EnsureSuccessStatusCode();

        Assert.That(response.Headers.Contains(CacheHeaders.CacheStatus));
        Assert.That(response.Headers.GetValues(CacheHeaders.CacheStatus).First(), Is.EqualTo(Enum.GetName(CacheStatus.EVICT)));
    }

    [TearDown]
    public async Task TearDown()
    {
        var cacheFactory = Server.Services.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Teams");
        await cache!.RemoveAsync();
    }
}