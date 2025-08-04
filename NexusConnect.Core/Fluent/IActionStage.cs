namespace NexusConnect.Core.Fluent;

/// <summary>
/// Represents a stage in a fluent action pipeline, allowing conversion to a specific provider actions type.
/// </summary>
public interface IActionStage
{
    /// <summary>
    /// Converts the current stage to the specified provider actions type.
    /// </summary>
    /// <typeparam name="TProviderActions">The type of provider actions to convert to.</typeparam>
    /// <returns>An instance of <typeparamref name="TProviderActions"/>.</returns>
    TProviderActions As<TProviderActions>() where TProviderActions : class;
}
