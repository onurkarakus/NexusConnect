using NexusConnect.Core.Fluent;
using NexusConnect.Core.Providers;

namespace NexusConnect.Core;

public static class Connect
{
    public static IAuthenticationStage To<TProvider>() where TProvider : IProvider, new()
    {
        // 1. Belirtilen tipte bir uzman (strategy) oluştur.
        var provider = new TProvider();

        // 2. Bu uzmanı General'e (Orchestrator) vererek onu yarat.
        return new FluentOrchestrator(provider);
    }

    private class FluentOrchestrator : IAuthenticationStage, IActionStage
    {
        private readonly IProvider _provider;

        public FluentOrchestrator(IProvider provider)
        {
            _provider = provider;
            Console.WriteLine($"Orchestrator, '{_provider.Name}' provider'ı ile çalışmaya hazırlandı.");
        }

        public IActionStage WithToken(string token)
        {
            // İşi kendi yapmak yerine uzmana devrediyor!
            _provider.Authenticate(token);
            return this;
        }

        public void Post(string message)
        {
            // İşi kendi yapmak yerine uzmana devrediyor!
            _provider.Post(message);
        }
    }
}

