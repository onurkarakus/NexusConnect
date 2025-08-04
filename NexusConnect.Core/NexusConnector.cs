using NexusConnect.Core.Providers;

namespace NexusConnect.Core;

/// <summary>
/// Provides configuration and management for provider factories used in NexusConnect.
/// </summary>
public class NexusConnector
{
    /// <summary>
    /// Stores registered provider factories mapped by their type.
    /// </summary>
    private static readonly Dictionary<Type, Func<IProvider>> ProviderFactories = new();

    /// <summary>
    /// Configures provider factories using the specified configuration action.
    /// </summary>
    /// <param name="configureAction">An action to configure provider registrations.</param>
    public static void Configure(Action<ConfigurationBuilder> configureAction)
    {
        var builder = new ConfigurationBuilder();
        configureAction(builder);
    }

    /// <summary>
    /// Retrieves the factory function for the specified provider type.
    /// </summary>
    /// <param name="providerType">The type of the provider.</param>
    /// <returns>A factory function that creates an instance of the provider.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no factory is registered for the specified provider type.
    /// </exception>
    internal static Func<IProvider> GetProviderFactory(Type providerType)
    {
        if (!ProviderFactories.TryGetValue(providerType, out var factory))
        {
            throw new InvalidOperationException($"{providerType.Name} için bir provider kaydı bulunamadı. Lütfen NexusConnector.Configure içinde kaydettiğinizden emin olun.");
        }

        return factory;
    }

    /// <summary>
    /// Builder class for registering provider factories.
    /// </summary>
    public class ConfigurationBuilder
    {
        /// <summary>
        /// Registers a provider factory for the specified provider type.
        /// </summary>
        /// <typeparam name="TProvider">The type of the provider to register.</typeparam>
        /// <param name="factory">A factory function that creates an instance of the provider.</param>
        public void RegisterProvider<TProvider>(Func<IProvider> factory) where TProvider : IProvider
        {
            ProviderFactories[typeof(TProvider)] = factory;
        }
    }
}
