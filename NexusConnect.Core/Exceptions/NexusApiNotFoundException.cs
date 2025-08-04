namespace NexusConnect.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a requested Nexus API resource is not found.
/// </summary>
public class NexusApiNotFoundException : NexusApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NexusApiNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NexusApiNotFoundException(string message) : base(message) { }
}
