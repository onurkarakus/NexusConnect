namespace NexusConnect.Core.Fluent;

public interface IActionStage
{
    void Post(string message);

    TProviderActions As<TProviderActions>() where TProviderActions : class;
}