using Xunit;
using NexusConnect.Core.Providers;

namespace NexusConnect.Core.Tests;

public class FluentApiTests
{
    [Fact]
    public void FullFluentChain_ShouldCompile_AndNotThrowException()
    {
        // Arrange & Act: Tüm akıcı arayüz zincirini çağır.
        var exception = Record.Exception(() =>
        {
            Connect.To<GitHubProvider>()
                   .WithToken("dummy-github-token")
                   .Post("Hello World from NexusConnect!");
        });

        // Assert: Hiçbir hata fırlatılmadığını doğrula.
        Assert.Null(exception);
    }
}