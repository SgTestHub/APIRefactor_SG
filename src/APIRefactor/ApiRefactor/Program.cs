using ApiRefactor.Data;
using ApiRefactor.Data.Repositories;
using ApiRefactor.Endpoints;

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

// Map endpoints
app.MapWavesEndpoints();

app.Run();
