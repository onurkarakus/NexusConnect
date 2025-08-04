using NexusConnect.Core.Fluent;
using NexusConnect.Core.Providers;

namespace NexusConnect.Core;

public static class Connect
{
    public static IAuthenticationStage To<TProvider>() where TProvider : IProvider
    {
        var factory = NexusConnector.GetProviderFactory(typeof(TProvider));
        var provider = factory();

        return new FluentOrchestrator(provider);
    }

    private class FluentOrchestrator : IAuthenticationStage, IActionStage
    {
        private readonly IProvider _provider;

        public FluentOrchestrator(IProvider provider)
        {
            _provider = provider;
            Console.WriteLine($"Orchestrator prepare to work with '{_provider.Name}' provider.");
        }

        public IActionStage WithToken(string token)
        {
            _provider.Authenticate(token);

            return this;
        }

        public TProviderActions As<TProviderActions>() where TProviderActions: class
        {
            if (_provider is TProviderActions providerActions)
            {
                return providerActions;
            }

            throw new InvalidCastException($"Mevcut provider ('{_provider.GetType().Name}'), istenen eylem setini ('{typeof(TProviderActions).Name}') desteklemiyor.");

        }
    }
}

