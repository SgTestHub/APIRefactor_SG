using ApiRefactor.Data;
using ApiRefactor.Data.Repositories;
using ApiRefactor.Endpoints;
using ApiRefactor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

// Register DbContext and Repository
builder.Services.AddScoped<WavesDbContext>();
builder.Services.AddScoped<IWaveRepository, WaveRepository>();

// Register JWT Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (!string.IsNullOrEmpty(secretKey))
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Bearer token support to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Get logger for Program
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Automatically initialize database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WavesDbContext>();

    try
    {
        // Ensure database and tables are created (creates schema if it doesn't exist)
        dbContext.Database.EnsureCreated();
        logger.LogInformation("Database and tables created successfully");

        // Seed initial data if not already present
        SeedDatabase(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization: {Message}", ex.Message);
    }
}

// Seed database with initial data
static void SeedDatabase(WavesDbContext context, ILogger<Program> logger)
{
    // Check if LocalUsers table already has data
    if (context.LocalUsers.Any())
    {
        logger.LogInformation("LocalUsers table already contains data. Skipping seed.");
        return;
    }

    try
    {
        var demoUsers = new List<ApiRefactor.Models.LocalUser>
        {
            new ApiRefactor.Models.LocalUser
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@gmail.com",
                Password = "admin123", // In production, hash passwords!
                Role = "Admin"
            },
            new ApiRefactor.Models.LocalUser
            {
                Id = 2,
                Name = "Test User",
                Email = "test@gmail.com",
                Password = "test123",
                Role = "User"
            },
            new ApiRefactor.Models.LocalUser
            {
                Id = 3,
                Name = "Regular User",
                Email = "user@example.com",
                Password = "user123",
                Role = "User"
            }
        };

        context.LocalUsers.AddRange(demoUsers);
        context.SaveChanges();

        logger.LogInformation("LocalUsers seed data inserted successfully. Added {Count} users.", demoUsers.Count);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding LocalUsers data: {Message}", ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthEndpoints();
app.MapWavesEndpoints();


app.Run();
