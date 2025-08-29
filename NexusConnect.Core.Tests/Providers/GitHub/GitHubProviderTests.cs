using Microsoft.Extensions.Configuration;
using NexusConnect.Core.Exceptions;
using NexusConnect.Core.Providers;
using NexusConnect.Core.Providers.GitHub;
using NexusConnect.Core.Providers.GitHub.Enums;

namespace NexusConnect.Core.Tests.Providers.GitHub;

public class GitHubProviderTests
{
    private readonly string _githubToken;

    public GitHubProviderTests()
    {
        var token = Environment.GetEnvironmentVariable("GH_PAT");
        if (string.IsNullOrEmpty(token))
        {
            var config = new ConfigurationBuilder().AddUserSecrets<GitHubProviderTests>().Build();
            token = config["GitHub:Token"];
        }
        _githubToken = token ?? throw new InvalidOperationException("GitHub token could not be found.");

        NexusConnector.Configure(cfg =>
        {
            cfg.RegisterProvider<GitHubProvider>(() => new GitHubProvider("onurkarakus", "BankingMicroServiceSample"));
        });
    }

    #region Error Handling Tests

    [Fact]
    public async Task WithInvalidToken_ShouldThrowAuthorizationException()
    {
        var invalidToken = "this-is-a-bad-token";
        await Assert.ThrowsAsync<NexusApiAuthorizationException>(async () =>
        {
            await Connect.To<GitHubProvider>()
                .WithToken(invalidToken)
                .As<IGitHubActions>()
                .GetIssues();
        });
    }

    #endregion

    #region Issue Lifecycle Tests

    [Fact]
    public async Task Create_Get_Update_Issue_Lifecycle_ShouldSucceed()
    {
        // ARRANGE: Create a new issue
        string originalTitle = $"Test Lifecycle Issue - {DateTime.Now.Ticks}";
        var createdIssue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateIssue(originalTitle, "This issue is for a full lifecycle test.");

        Assert.NotNull(createdIssue);
        Assert.Equal(originalTitle, createdIssue.Title);

        // ACT 1: Get the issue by its number
        var fetchedIssue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .GetIssueByNumber(createdIssue.Number);

        // ASSERT 1
        Assert.NotNull(fetchedIssue);
        Assert.Equal(createdIssue.Id, fetchedIssue.Id);

        // ACT 2: Update the issue
        string newTitle = "UPDATED - " + originalTitle;
        var updatedIssue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .UpdateIssue(createdIssue.Number, title: newTitle, state: IssueState.Closed);

        // ASSERT 2
        Assert.NotNull(updatedIssue);
        Assert.Equal(newTitle, updatedIssue.Title);
        Assert.Equal("closed", updatedIssue.State);
    }

    [Fact]
    public async Task GetIssues_ShouldReturnIssues()
    {
        var issues = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .GetIssues(IssueState.All);

        Assert.NotNull(issues);
    }

    #endregion

    #region Label & Comment Tests

    [Fact]
    public async Task AddAndRemoveLabels_And_CreateComment_ShouldSucceed()
    {
        // ARRANGE: Create a new issue for the test
        const string labelNameToTest = "bug"; // Assumes "bug" label exists in the repo
        var issue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateIssue($"Test Issue for Labels & Comments - {DateTime.Now.Ticks}", "This issue will be used to test labels and comments.");
        Assert.NotNull(issue);

        // ACT & ASSERT 1: Add Label
        var labelsAfterAdd = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .AddLabelsToIssue(issue.Number, labelNameToTest);
        Assert.Contains(labelsAfterAdd, l => l.Name == labelNameToTest);

        // ACT & ASSERT 2: Create Comment
        string commentText = "This is a test comment from NexusConnect.";
        var createdComment = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateComment(issue.Number, commentText);
        Assert.NotNull(createdComment);
        Assert.Equal(commentText, createdComment.Body);

        // ACT & ASSERT 3: Remove Label
        var labelsAfterRemove = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .RemoveLabelFromIssue(issue.Number, labelNameToTest);
        Assert.DoesNotContain(labelsAfterRemove, l => l.Name == labelNameToTest);
    }

    [Fact]
    public async Task GetLabelsForRepository_ShouldReturnLabels()
    {
        var labels = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .GetLabelsForRepository();
        Assert.NotNull(labels);
    }

    #endregion
}