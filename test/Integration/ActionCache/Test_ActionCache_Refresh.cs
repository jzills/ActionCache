using ActionCache;
using ActionCache.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class Test_ActionCache_Refresh
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
        var userResponse = await Client.GetAsync("/users");
        userResponse.EnsureSuccessStatusCode();

        // Cache hit
        userResponse = await Client.GetAsync("/users");
        userResponse.EnsureSuccessStatusCode();

        Assert.That(userResponse.Headers.Contains("Cache-Status"));
        Assert.That(userResponse.Headers.GetValues("Cache-Status").First(), Is.EqualTo("HIT"));

        // Cache refresh
        userResponse = await Client.PostAsync("/users", null);
        userResponse.EnsureSuccessStatusCode();

        Assert.That(userResponse.Headers.Contains("Cache-Status"));
        Assert.That(userResponse.Headers.GetValues("Cache-Status").First(), Is.EqualTo("REFRESH"));
    }

    [TearDown]
    public async Task TearDown()
    {
        var cacheFactory = Server.Services.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Users");
        await cache!.RemoveAsync();
    }
}