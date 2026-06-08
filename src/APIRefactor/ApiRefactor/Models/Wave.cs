namespace ApiRefactor.Models;

/// <summary>
/// Represents a Wave entity.
/// This model contains only data properties and no data access logic.
/// </summary>
public class Wave
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime WaveDate { get; set; } = DateTime.Now;

}
