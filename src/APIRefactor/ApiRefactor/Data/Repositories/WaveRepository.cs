using ApiRefactor.Models;
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Data.Repositories;

public class WaveRepository : IWaveRepository
{
    private readonly WavesDbContext _dbContext;
    private readonly ILogger _logger;

    public WaveRepository(WavesDbContext dbContext, ILogger<WaveRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Wave>> GetAll()
    {
        _logger.LogInformation("Getting all waves");
        var waves = new List<Wave>();
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id FROM waves";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var id = Guid.Parse(reader["id"].ToString() ?? string.Empty);
                        var wave = await GetById(id);
                        if (wave != null)
                        {
                            waves.Add(wave);
                        }
                    }
                }
            }
        }
        _logger.LogInformation("Retrieved {Count} waves", waves.Count);
        return waves;
    }

    public async Task<Wave?> GetById(Guid id)
    {
        _logger.LogInformation("Getting WAVE with id {Id}", id);

        if (!await WaveExists(id))
        {
            _logger.LogWarning("Wave with id {Id} not found", id);
            return null;
        }

        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, name, wavedate FROM waves WHERE id = @id";
                command.Parameters.AddWithValue("@id", id.ToString());

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var wave = new Wave
                        {
                            Id = Guid.Parse(reader["id"].ToString() ?? string.Empty),
                            Name = reader["name"].ToString() ?? string.Empty,
                            WaveDate = DateTime.Parse(reader["wavedate"].ToString() ?? DateTime.Now.ToString())
                        };
                        _logger.LogInformation("Successfully retrieved wave {Id}", id);
                        return wave;
                    }
                }
            }
        }

        return null;
    }

    public async Task Save(Wave wave)
    {
        if (wave == null)
            throw new ArgumentNullException(nameof(wave));

        _logger.LogInformation("Saving wave {Id} with name {Name}", wave.Id, wave.Name);

        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            // Check if wave exists
            var existingWave = await WaveExists(wave.Id);

            using (var command = connection.CreateCommand())
            {
                if (!existingWave)
                {
                    // Insert
                    _logger.LogInformation("Inserting new wave {Id}", wave.Id);
                    command.CommandText = @"INSERT INTO waves (id, name, wavedate) 
                                           VALUES (@id, @name, @wavedate)";
                    command.Parameters.AddWithValue("@id", wave.Id.ToString());
                    command.Parameters.AddWithValue("@name", wave.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@wavedate", wave.WaveDate);
                }
                else
                {
                    // Update
                    _logger.LogInformation("Updating existing wave {Id}", wave.Id);
                    command.CommandText = @"UPDATE waves 
                                           SET name = @name, wavedate = @wavedate 
                                           WHERE id = @id";
                    command.Parameters.AddWithValue("@id", wave.Id.ToString());
                    command.Parameters.AddWithValue("@name", wave.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@wavedate", wave.WaveDate);
                }

                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Wave {Id} saved successfully", wave.Id);
            }
        }
    }

    public async Task Delete(Guid id)
    {
       
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM waves WHERE id = @id";
                command.Parameters.AddWithValue("@id", id.ToString());
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Wave {Id} deleted successfully", id);
            }
        }
    }

    public async Task<bool> WaveExists(Guid id)
    {
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT 1 FROM waves WHERE id = @id LIMIT 1";
                command.Parameters.AddWithValue("@id", id.ToString());

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await reader.ReadAsync();
                }
            }
        }
    }

}
