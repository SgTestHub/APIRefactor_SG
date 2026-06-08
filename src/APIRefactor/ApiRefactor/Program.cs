using ApiRefactor.Data;
using ApiRefactor.Data.Repositories;
using ApiRefactor.Models;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext and Repository
builder.Services.AddScoped<WavesDbContext>();
builder.Services.AddScoped<IWaveRepository, WaveRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/wave", (IWaveRepository repository) => repository.GetAll())
    .WithName("GetWaves")
    .WithOpenApi();

app.MapGet("/api/wave/{id}", (Guid id, IWaveRepository repository) => 
    {
        var wave = repository.GetById(id);
        return wave is not null ? Results.Ok(wave) : Results.NotFound();
    })
    .WithName("GetWaveById")
    .WithOpenApi();

app.MapPost("/api/wave", (Wave wave, IWaveRepository repository) => 
    {
        repository.Save(wave);
        return Results.Ok(wave);
    })
    .WithName("UpsertWave")
    .WithOpenApi();

app.Run();
