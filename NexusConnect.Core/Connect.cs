using NexusConnect.Core.Fluent;
using NexusConnect.Core.Providers;

namespace NexusConnect.Core;

/// <summary>
/// Provides fluent methods to connect to a provider, authenticate, and access provider-specific actions.
/// </summary>
public static class Connect
{
    /// <summary>
    /// Initiates a fluent connection pipeline for the specified provider type.
    /// </summary>
    /// <typeparam name="TProvider">The type of provider to connect to.</typeparam>
    /// <returns>An <see cref="IAuthenticationStage"/> to begin authentication.</returns>
    public static IAuthenticationStage To<TProvider>() where TProvider : IProvider
    {
        var factory = NexusConnector.GetProviderFactory(typeof(TProvider));
        var provider = factory();

        return new FluentOrchestrator(provider);
    }

    /// <summary>
    /// Orchestrates the authentication and action stages for a provider.
    /// Implements <see cref="IAuthenticationStage"/> and <see cref="IActionStage"/>.
    /// </summary>
    private sealed class FluentOrchestrator : IAuthenticationStage, IActionStage
    {
        private readonly IProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentOrchestrator"/> class with the specified provider.
        /// </summary>
        /// <param name="provider">The provider to orchestrate.</param>
        public FluentOrchestrator(IProvider provider)
        {
            _provider = provider;
            Console.WriteLine($"Orchestrator prepare to work with '{_provider.Name}' provider.");
        }

        /// <summary>
        /// Authenticates the provider using the specified token.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <returns>The current <see cref="IActionStage"/> instance.</returns>
        public IActionStage WithToken(string token)
        {
            _provider.Authenticate(token);

            return this;
        }

        /// <summary>
        /// Casts the provider to the specified action set type if supported.
        /// </summary>
        /// <typeparam name="TProviderActions">The type of the provider action set.</typeparam>
        /// <returns>The provider cast to <typeparamref name="TProviderActions"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown if the provider does not support the requested action set type.
        /// </exception>
        public TProviderActions As<TProviderActions>() where TProviderActions : class
        {
            if (_provider is TProviderActions providerActions)
            {
                return providerActions;
            }

            throw new InvalidCastException($"Mevcut provider ('{_provider.GetType().Name}'), istenen eylem setini ('{typeof(TProviderActions).Name}') desteklemiyor.");

        }

        public IActionStage WithDefaultToken()
        {
            var defaultToken = NexusConnector.GetDefaultToken();
            _provider.Authenticate(defaultToken);

            return this;
        }
    }
}

