namespace NexusConnect.Core.Exceptions;

public class NexusApiAuthorizationException : NexusApiException
{
    public NexusApiAuthorizationException(string message) : base(message) { }
}