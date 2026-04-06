using Microsoft.EntityFrameworkCore;
using naesangi.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Database connection string 구성
var host = builder.Configuration["DATABASE:HOST"]
    ?? throw new InvalidOperationException("DATABASE:HOST is missing.");
var port = builder.Configuration["DATABASE:PORT"]
    ?? throw new InvalidOperationException("DATABASE:PORT is missing.");
var db = builder.Configuration["DATABASE:NAME"]
    ?? throw new InvalidOperationException("DATABASE:NAME is missing.");
var user = builder.Configuration["DATABASE:USER"]
    ?? throw new InvalidOperationException("DATABASE:USER is missing.");
var password = builder.Configuration["DATABASE:PASSWORD"]
    ?? throw new InvalidOperationException("DATABASE:PASSWORD is missing.");

var connStr = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

// CORS 정책 추가
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // 모든 출처 허용
              .AllowAnyMethod()      // 모든 HTTP 메서드 허용 (GET, POST 등)
              .AllowAnyHeader();     // 모든 헤더 허용
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();  // Swagger 생성기 추가
builder.Services.AddHealthChecks()
    .AddNpgSql(connStr, name: "postgres");
builder.Services.AddOpenApi();
builder.Services.AddDbContext<NaesangiDbContext>(opt =>
    opt.UseNpgsql(connStr));

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // /swagger 경로에서 접근 가능
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapHealthChecks("/health");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
