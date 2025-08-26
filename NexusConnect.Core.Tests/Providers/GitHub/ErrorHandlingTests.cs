using NexusConnect.Core.Providers;
using NexusConnect.Core.Exceptions;
using NexusConnect.Core.Providers.GitHub;

namespace NexusConnect.Core.Tests.Providers.GitHub;

public class ErrorHandlingTests
{
    public ErrorHandlingTests()
    {
        NexusConnector.Configure(config =>
        {
            config.RegisterProvider<GitHubProvider>(() => new GitHubProvider("onurkarakus", "BankingMicroServiceSample"));
        });
    }

    [Fact]
    public async Task GitHub_WithInvalidToken_ShouldThrowAuthorizationException()
    {
        // Arrange
        var invalidToken = "invalid-token";

        // Act & Assert
        await Assert.ThrowsAsync<NexusApiAuthorizationException>(async () =>
        {
            await Connect.To<GitHubProvider>()
                .WithToken(invalidToken)
                .As<IGitHubActions>()
                .GetIssues();
        });
    }
}