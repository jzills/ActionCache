using ActionCache;
using ActionCache.Common.Enums;
using ActionCache.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class Test_ActionCacheFilter
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
        var userResponse = await Client.GetAsync(route);
        userResponse.EnsureSuccessStatusCode();
    }

    [TearDown]
    public async Task TearDown()
    {
        var cacheFactory = Server.Services.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Teams");
        await cache!.RemoveAsync();
    }
}