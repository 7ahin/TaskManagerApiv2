using Microsoft.EntityFrameworkCore;
using System;
using TaskManagerApi.Models;

const string AllowFrontendOrigin = "AllowFrontendOrigin";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowFrontendOrigin,
        policy =>
        {
            var origins = builder.Configuration["AllowedOrigins"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    policy.SetIsOriginAllowed(_ => false)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            }
        });
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoContext>(options =>
{
    var provider = builder.Configuration["DbProvider"]?.Trim().ToLowerInvariant();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(provider))
    {
        provider = connectionString?.Contains("Host=", StringComparison.OrdinalIgnoreCase) == true ? "postgres" : "sqlserver";
    }

    if (provider == "postgres" || provider == "postgresql")
    {
        options.UseNpgsql(connectionString);
        return;
    }

    options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowFrontendOrigin);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
