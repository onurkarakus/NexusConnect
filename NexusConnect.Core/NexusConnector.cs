using NexusConnect.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusConnect.Core;

public class NexusConnector
{
    private static readonly Dictionary<Type, Func<IProvider>> ProviderFactories = new();

    public static void Configure(Action<ConfigurationBuilder> configureAction)
    {
        var builder = new ConfigurationBuilder();
        configureAction(builder);
    }

    internal static Func<IProvider> GetProviderFactory(Type providerType)
    {
        if (!ProviderFactories.TryGetValue(providerType, out var factory))
        {
            throw new InvalidOperationException($"{providerType.Name} için bir provider kaydı bulunamadı. Lütfen NexusConnector.Configure içinde kaydettiğinizden emin olun.");
        }
        return factory;
    }

    public class ConfigurationBuilder
    {
        public void RegisterProvider<TProvider>(Func<IProvider> factory) where TProvider : IProvider
        {
            ProviderFactories[typeof(TProvider)] = factory;
        }
    }
}
