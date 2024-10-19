using ActionCache.Memory.Extensions;
using ActionCache.Redis.Extensions;
using Api.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseInMemoryDatabase(nameof(ApplicationDbContext)));

builder.Services.AddActionCacheMemory();
builder.Services.AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();