using NexusConnect.Core.Providers;
using NexusConnect.Core.Providers.GitHub;

namespace NexusConnect.Core.Tests;

public class FluentApiTests
{
    public FluentApiTests()
    {
        NexusConnector.Configure(config =>
        {
            // GitHubProvider istendiğinde, onu nasıl oluşturacağını sisteme öğretiyoruz.
            config.RegisterProvider<GitHubProvider>(() => new GitHubProvider());
        });
    }

    [Fact]
    public void FullFluentChain_ShouldCompile_AndNotThrowException()
    {
        // Arrange & Act
        var exception = Record.Exception(() =>
        {
            Connect.To<GitHubProvider>()
                   .WithToken("dummy-github-token")
                   .As<IGitHubActions>()
                   .CreateIssue("New Feature: Fluent API", "We need to implement a fluent API.");
        });

        // Assert
        Assert.Null(exception);
    }
}