using StackExchange.Redis;
using WeatherApp.Api.Endpoints;
using WeatherApp.Api.Middlewares;
using WeatherApp.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionFilterMiddleware>();

var apiKey = builder.Configuration["WeatherApiKey"]!;

WeatherEndpoints.MapEndpoints(app, apiKey);

app.UseHttpsRedirection();

app.Run();