namespace NexusConnect.Core.Providers;

public interface IProvider
{
    string Name { get; }

    void Authenticate(string token);

    void Post(string message);
}
