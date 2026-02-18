/// <summary>
/// Base interface for all manager systems.
/// Defines the initialization contract for game systems.
/// </summary>
public interface IManager
{
    /// <summary>
    /// Initializes the manager system.
    /// Called after all dependencies are set up.
    /// </summary>
    void Init();
}