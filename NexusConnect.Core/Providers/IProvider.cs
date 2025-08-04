namespace NexusConnect.Core.Providers;

/// <summary>
/// Represents a provider with authentication capabilities.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Authenticates the provider using the specified token.
    /// </summary>
    /// <param name="token">The authentication token.</param>
    void Authenticate(string token);
}
