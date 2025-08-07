namespace NexusConnect.Core.Fluent;

/// <summary>
/// Represents the authentication stage in a fluent pipeline, allowing a token to be provided
/// and advancing to the action stage.
/// </summary>
public interface IAuthenticationStage
{
    /// <summary>
    /// Specifies the authentication token and advances to the action stage.
    /// </summary>
    /// <param name="token">The authentication token.</param>
    /// <returns>An instance of <see cref="IActionStage"/> representing the next stage.</returns>
    IActionStage WithToken(string token);

    IActionStage WithDefaultToken();
}
