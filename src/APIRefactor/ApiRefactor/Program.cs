using ApiRefactor.Data;
using ApiRefactor.Data.Repositories;
using ApiRefactor.Endpoints;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Get logger for Program
var logger = app.Services.GetRequiredService<ILogger<Program>>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints

app.MapWavesEndpoints();


app.Run();
