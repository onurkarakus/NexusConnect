using Microsoft.Extensions.Configuration;
using NexusConnect.Core.Providers;
using NexusConnect.Core.Providers.GitHub;
using NexusConnect.Core.Providers.GitHub.Enums;
using NexusConnect.Core.Providers.GitHub.Models;

namespace NexusConnect.Core.Tests;

public class FluentApiTests
{
    private readonly string _githubToken;

    public FluentApiTests()
    {
        _githubToken = Environment.GetEnvironmentVariable("GH_PAT");

        if (string.IsNullOrEmpty(_githubToken))
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddUserSecrets<FluentApiTests>()
                .Build();

            _githubToken = configurationBuilder["GitHub:Token"];
        }

        if (string.IsNullOrEmpty(_githubToken))
        {
            throw new InvalidOperationException("GitHub token could not be found in environment variables (GH_PAT) or User Secrets (GitHub:Token).");
        }
        
        NexusConnector.Configure(cfg =>
        {
            cfg.RegisterProvider<GitHubProvider>(() => new GitHubProvider("onurkarakus", "BankingMicroServiceSample"));
        });
    }

    [Fact]
    public async Task GitHub_CreateIssue_Chain_ShouldSucceed()
    {
        var exception = await Record.ExceptionAsync(async () =>
        {
            await Connect.To<GitHubProvider>()
                   .WithToken(_githubToken)
                   .As<IGitHubActions>()
                   .CreateIssue($"Test Issue from NexusConnect - {DateTime.Now}", "This issue was created successfully by an automated test.");
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task GitHub_GetIssues_ShouldReturnIssues()
    {
        // Arrange
        IEnumerable<Issue>? issues = null;

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            issues = await Connect.To<GitHubProvider>()
                .WithToken(_githubToken)
                .As<IGitHubActions>()
                .GetIssues(IssueState.Closed);
        });

        // Assert
        Assert.Null(exception);
        Assert.NotNull(issues);
    }

    [Fact]
    public async Task GitHub_GetIssueByNumber_ShouldReturnCorrectIssue()
    {
        // Arrange
        const int EXISTING_ISSUE_NUMBER = 1;

        Issue? issue = null;

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            issue = await Connect.To<GitHubProvider>()
                .WithToken(_githubToken)
                .As<IGitHubActions>()
                .GetIssueByNumber(EXISTING_ISSUE_NUMBER);
        });

        // Assert
        Assert.Null(exception);
        Assert.NotNull(issue);
        Assert.Equal(EXISTING_ISSUE_NUMBER, issue.Number);
    }

    [Fact]
    public async Task GitHub_UpdateIssue_ShouldChangeStateAndTitle()
    {
        // Arrange
        string originalTitle = $"Test Issue to be Updated - {DateTime.Now.Ticks}";
        var issueToUpdate = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateIssue(originalTitle, "This issue will be updated and closed.");

        Assert.NotNull(issueToUpdate);
        Assert.Equal("open", issueToUpdate.State);

        // Act
        string newTitle = "UPDATED - " + originalTitle;
        Issue? updatedIssue = null;

        var exception = await Record.ExceptionAsync(async () =>
        {
            updatedIssue = await Connect.To<GitHubProvider>()
                .WithToken(_githubToken)
                .As<IGitHubActions>()
                .UpdateIssue(issueToUpdate.Number, title: newTitle, state: IssueState.Closed);
        });

        // Assert
        Assert.Null(exception);
        Assert.NotNull(updatedIssue);
        Assert.Equal(newTitle, updatedIssue.Title); 
        Assert.Equal("closed", updatedIssue.State); 
    }

    [Fact]
    public async Task Connect_WithDefaultToken_ShouldSucceed()
    {
        // ARRANGE
        // Testin başında, varsayılan token olarak geçerli token'ımızı ayarlıyoruz.
        NexusConnector.SetDefaultToken(_githubToken);

        // ACT
        var exception = await Record.ExceptionAsync(async () =>
        {
            // Artık .WithDefaultToken() kullanıyoruz
            var issues = await Connect.To<GitHubProvider>()
                .WithDefaultToken()
                .As<IGitHubActions>()
                .GetIssues();

            Assert.NotNull(issues);
        });

        // ASSERT
        Assert.Null(exception);
    }

    [Fact]
    public async Task GitHub_CreateComment_ShouldSucceedAndReturnComment()
    {
        // 1. ARRANGE (Kurulum):
        // Yorum eklemek için önce üzerine yorum yapabileceğimiz bir issue oluşturuyoruz.
        // Bu, testimizi bağımsız ve tekrar edilebilir kılar.
        string issueTitle = $"Test Issue for Commenting - {DateTime.Now.Ticks}";
        var issue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateIssue(issueTitle, "This issue is for testing comment creation.");

        Assert.NotNull(issue);

        // 2. ACT (Eylem):
        // Şimdi, oluşturduğumuz bu issue'nun numarasını kullanarak yeni yorumumuzu ekliyoruz.
        string commentText = "This is a test comment from NexusConnect.";
        Comment? createdComment = null;

        var exception = await Record.ExceptionAsync(async () =>
        {
            createdComment = await Connect.To<GitHubProvider>()
                .WithToken(_githubToken)
                .As<IGitHubActions>()
                .CreateComment(issue.Number, commentText);
        });

        // 3. ASSERT (Doğrulama):
        Assert.Null(exception); // Hata fırlatılmamalı.
        Assert.NotNull(createdComment); // Dönen yorum nesnesi null olmamalı.
        Assert.Equal(commentText, createdComment.Body); // Yorumun içeriğinin doğru olduğunu doğrula.
        Assert.NotEmpty(createdComment.Url); // Dönen URL'nin boş olmadığını kontrol et.
    }
}