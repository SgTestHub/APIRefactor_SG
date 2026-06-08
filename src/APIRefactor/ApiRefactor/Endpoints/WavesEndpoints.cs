using ApiRefactor.Data.Repositories;
using ApiRefactor.Models;

namespace ApiRefactor.Endpoints;

public static class WavesEndpoints
{
    public static void MapWavesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wave")
            .WithTags("Waves");

        group.MapGet("/", GetAllWaves)
            .WithName("GetWaves")
            .WithOpenApi()
            .Produces<List<Wave>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetWaveById)
            .WithName("GetWaveById")
            .WithOpenApi()
            .Produces<Wave>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", UpsertWave)
            .WithName("UpsertWave")
            .WithOpenApi()
            .Produces<Wave>(StatusCodes.Status200OK)
            .Accepts<Wave>("application/json");
    }

    private static List<Wave> GetAllWaves(IWaveRepository repository)
    {
        return repository.GetAll();
    }

    private static IResult GetWaveById(Guid id, IWaveRepository repository)
    {
        var wave = repository.GetById(id);
        return wave is not null ? Results.Ok(wave) : Results.NotFound();
    }

    private static IResult UpsertWave(Wave wave, IWaveRepository repository)
    {
        repository.Save(wave);
        return Results.Ok(wave);
    }
}
