namespace NexusConnect.Core.Exceptions;

public class NexusApiException : Exception
{
    public NexusApiException(string message) : base(message) { }
    public NexusApiException(string message, Exception innerException) : base(message, innerException) { }
}

