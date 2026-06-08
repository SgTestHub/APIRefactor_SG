using ApiRefactor.Models;
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Data.Repositories;

public class WaveRepository : IWaveRepository
{
    private readonly WavesDbContext _dbContext;

    public WaveRepository(WavesDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public List<Wave> GetAll()
    {
        var waves = new List<Wave>();
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id FROM waves";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = Guid.Parse(reader["id"].ToString() ?? string.Empty);
                        var wave = GetById(id);
                        if (wave != null)
                        {
                            waves.Add(wave);
                        }
                    }
                }
            }
        }

        return waves;
    }

    public Wave? GetById(Guid id)
    {
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, name, wavedate FROM waves WHERE id = @id";
                command.Parameters.AddWithValue("@id", id.ToString());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var wave = new Wave
                        {
                            Id = Guid.Parse(reader["id"].ToString() ?? string.Empty),
                            Name = reader["name"].ToString() ?? string.Empty,
                            WaveDate = DateTime.Parse(reader["wavedate"].ToString() ?? DateTime.Now.ToString())
                        };

                        return wave;
                    }
                }
            }
        }

        return null;
    }

    public void Save(Wave wave)
    {
        if (wave == null)
            throw new ArgumentNullException(nameof(wave));

        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            // Check if wave exists
            var existingWave = GetById(wave.Id);

            using (var command = connection.CreateCommand())
            {
                if (existingWave == null)
                {
                    // Insert
                    command.CommandText = @"INSERT INTO waves (id, name, wavedate) 
                                           VALUES (@id, @name, @wavedate)";
                    command.Parameters.AddWithValue("@id", wave.Id.ToString());
                    command.Parameters.AddWithValue("@name", wave.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@wavedate", wave.WaveDate);
                }
                else
                {
                    // Update
                    command.CommandText = @"UPDATE waves 
                                           SET name = @name, wavedate = @wavedate 
                                           WHERE id = @id";
                    command.Parameters.AddWithValue("@id", wave.Id.ToString());
                    command.Parameters.AddWithValue("@name", wave.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@wavedate", wave.WaveDate);
                }

                command.ExecuteNonQuery();
            }
        }
    }

    public void Delete(Guid id)
    {
        using (var connection = _dbContext.GetConnection())
        {
            _dbContext.EnsureConnectionOpen(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM waves WHERE id = @id";
                command.Parameters.AddWithValue("@id", id.ToString());
                command.ExecuteNonQuery();
            }
        }
    }
}
