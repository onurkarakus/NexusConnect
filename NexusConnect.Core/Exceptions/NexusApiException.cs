namespace NexusConnect.Core.Exceptions;

/// <summary>
/// Represents errors that occur when interacting with the Nexus API.
/// </summary>
public class NexusApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NexusApiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NexusApiException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NexusApiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NexusApiException(string message, Exception innerException) : base(message, innerException) { }
}

