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
        string NormalizePostgres(string cs)
        {
            if (cs?.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) == true ||
                cs?.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) == true)
            {
                var uri = new Uri(cs);
                var userInfo = Uri.UnescapeDataString(uri.UserInfo ?? "");
                var split = userInfo.Split(':', 2);
                var username = split.Length > 0 ? split[0] : "";
                var password = split.Length > 1 ? split[1] : "";
                var db = uri.AbsolutePath.Trim('/'); 
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var ssl = host.EndsWith(".internal", StringComparison.OrdinalIgnoreCase)
                    ? "SSL Mode=Disable"
                    : "SSL Mode=Require;Trust Server Certificate=true";
                return $"Host={host};Port={port};Database={db};Username={username};Password={password};{ssl}";
            }
            return cs ?? "";
        }

        options.UseNpgsql(NormalizePostgres(connectionString));
        return;
    }

    options.UseSqlServer(connectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();
    var db = services.GetRequiredService<TodoContext>();
    var provider = config["DbProvider"]?.Trim().ToLowerInvariant();
    if (provider == "postgres" || provider == "postgresql")
    {
        db.Database.EnsureCreated();
    }
    else
    {
        db.Database.Migrate();
    }
}

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
