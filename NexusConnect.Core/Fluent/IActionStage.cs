namespace NexusConnect.Core.Fluent;

public interface IActionStage
{
    TProviderActions As<TProviderActions>() where TProviderActions : class;
}