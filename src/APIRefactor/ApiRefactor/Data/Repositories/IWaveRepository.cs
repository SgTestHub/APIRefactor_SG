using ApiRefactor.Models;

namespace ApiRefactor.Data.Repositories;

public interface IWaveRepository
{
    /// <summary>
    /// Gets all waves from the database.
    /// </summary>
    Task<List<Wave>> GetAll();

    /// <summary>
    /// Gets a wave by its ID.
    /// </summary>
    Task<Wave?> GetById(Guid id);

    /// <summary>
    /// Saves a wave (insert or update).
    /// </summary>
    Task Save(Wave wave);

    /// <summary>
    /// Deletes a wave by its ID.
    /// </summary>
    Task Delete(Guid id);

    Task<bool> WaveExists(Guid id);
}
