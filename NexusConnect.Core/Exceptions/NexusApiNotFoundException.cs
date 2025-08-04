namespace NexusConnect.Core.Exceptions;

public class NexusApiNotFoundException : NexusApiException
{
    public NexusApiNotFoundException(string message) : base(message) { }
}