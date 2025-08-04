namespace NexusConnect.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when API authorization fails in NexusConnect.
/// </summary>
public class NexusApiAuthorizationException : NexusApiException
{
    public NexusApiAuthorizationException(string message) : base(message) { }
}
