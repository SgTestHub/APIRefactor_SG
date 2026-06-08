using ApiRefactor.Data.Repositories;
using ApiRefactor.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace ApiRefactor.Endpoints;

public static class WavesEndpoints
{
    public static void MapWavesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wave")
            .WithTags("Waves")
            .RequireAuthorization();

        group.MapGet("/", GetAllWaves)
            .WithName("GetWaves")
            .WithOpenApi()
            .Produces<APIResponse>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetWaveById)
            .WithName("GetWaveById")
            .WithOpenApi()
            .Produces<APIResponse>(StatusCodes.Status200OK)
            .Produces<APIResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", UpsertWave)
            .WithName("UpsertWave")
            .WithOpenApi()
            .Produces<APIResponse>(StatusCodes.Status201Created)
            .Accepts<Wave>("application/json");
    }

    private static async Task<IResult> GetAllWaves(IWaveRepository repository,ILogger<Program> logger)
    {
        var waves = await repository.GetAll();
        return Results.Ok(new APIResponse
        {
            IsSuccess = true,
            Result = waves,
            StatusCode = HttpStatusCode.OK
        });
    }

    private static async Task<IResult> GetWaveById(Guid id, IWaveRepository repository, ILogger<Program> logger)
    {
        if(!await repository.WaveExists(id))
        {
            logger.LogWarning("Wave with id {Id} not found", id);
            return Results.NotFound(new APIResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { $"Wave with id {id} not found" },
                StatusCode = HttpStatusCode.NotFound
            });
        }
        var wave = await repository.GetById(id);
        return Results.Ok(new APIResponse
        {
            IsSuccess = true,
            Result = wave,
            StatusCode = HttpStatusCode.OK
        });
    }

    private static async Task<IResult> UpsertWave(Wave wave, IWaveRepository repository, ILogger<Program> logger)
    {
        await repository.Save(wave);
        return Results.CreatedAtRoute("GetWaveById", new { id = wave.Id },
                new APIResponse
                {
                    IsSuccess = true,
                    Result = wave,
                    StatusCode = HttpStatusCode.Created
                });
    }
}
