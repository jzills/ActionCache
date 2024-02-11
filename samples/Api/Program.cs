using ActionCache.Redis.Extensions;
using ActionCache.Memory.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddActionCacheMemory(options => options.SizeLimit = int.MaxValue);
builder.Services.AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var manager = (ApplicationPartManager)builder.Services
    .LastOrDefault(d => d.ServiceType == typeof(ApplicationPartManager))
    .ImplementationInstance;
var feature = new ControllerFeature();
manager.PopulateFeature(feature);
var controllerTypes = feature.Controllers.Select(t => t.AsType());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();