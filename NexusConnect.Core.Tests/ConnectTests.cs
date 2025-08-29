using NexusConnect.Core.Providers;

namespace NexusConnect.Core.Tests;

public class ConnectTests
{
    public ConnectTests()
    {
        // Testin çalışması için en az bir provider'ın kayıtlı olması gerekir.
        NexusConnector.Configure(cfg =>
        {
            cfg.RegisterProvider<GitHubProvider>(() => new GitHubProvider("test", "test"));
        });
    }

    [Fact]
    public void ConnectTo_ShouldReturnAuthenticationStage_AndNotThrow()
    {
        // Sadece temel Connect.To<T> zincirinin bir hata fırlatmadığını test ediyoruz.
        var exception = Record.Exception(() => Connect.To<GitHubProvider>());
        Assert.Null(exception);
    }
}