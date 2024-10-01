using ActionCache.Attributes;
using ActionCache.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

[TestFixture]
public class Test_ActionCacheRehydrator
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
    }
}