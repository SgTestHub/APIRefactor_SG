using ApiRefactor.Models;

namespace ApiRefactor.Data.Repositories;

public interface IWaveRepository
{
    /// <summary>
    /// Gets all waves from the database.
    /// </summary>
    List<Wave> GetAll();

    /// <summary>
    /// Gets a wave by its ID.
    /// </summary>
    Wave? GetById(Guid id);

    /// <summary>
    /// Saves a wave (insert or update).
    /// </summary>
    void Save(Wave wave);

    /// <summary>
    /// Deletes a wave by its ID.
    /// </summary>
    void Delete(Guid id);
}
