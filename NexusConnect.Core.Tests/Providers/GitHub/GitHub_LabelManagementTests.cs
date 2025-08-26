using NexusConnect.Core.Providers;
using NexusConnect.Core.Providers.GitHub;
using Microsoft.Extensions.Configuration;

namespace NexusConnect.Core.Tests.Providers.GitHub;

public class GitHub_LabelManagementTests
{
    private readonly string _githubToken;

    public GitHub_LabelManagementTests()
    {
        // Sırları ve konfigürasyonu okuma (diğer test dosyamızla aynı mantık)
        var token = Environment.GetEnvironmentVariable("GH_PAT");
        if (string.IsNullOrEmpty(token))
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<GitHub_LabelManagementTests>()
                .Build();
            token = config["GitHub:Token"];
        }
        _githubToken = token ?? throw new InvalidOperationException("GitHub token could not be found.");

        NexusConnector.Configure(cfg =>
        {
            cfg.RegisterProvider<GitHubProvider>(() => new GitHubProvider("onurkarakus", "BankingMicroServiceSample"));
        });
    }

    [Fact]
    public async Task GetLabelsForRepository_ShouldReturnLabels()
    {
        // ARRANGE & ACT
        var exception = await Record.ExceptionAsync(async () =>
        {
            var labels = await Connect.To<GitHubProvider>()
                .WithToken(_githubToken)
                .As<IGitHubActions>()
                .GetLabelsForRepository();

            // ASSERT
            Assert.NotNull(labels);
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task AddAndRemoveLabels_OnNewIssue_ShouldSucceed()
    {
        // ÖN HAZIRLIK: Bu testin çalışması için test reponda "bug" adında bir etiket olmalıdır.
        const string labelNameToTest = "bug";

        // 1. ARRANGE: Test için yeni bir issue oluştur
        string issueTitle = $"Test Issue for Labels - {DateTime.Now.Ticks}";
        var issue = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .CreateIssue(issueTitle, "This issue is for testing label management.");

        Assert.NotNull(issue);

        // 2. ACT (Etiket Ekleme) & ASSERT
        var labelsAfterAdd = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .AddLabelsToIssue(issue.Number, labelNameToTest);

        Assert.NotNull(labelsAfterAdd);
        Assert.Contains(labelsAfterAdd, l => l.Name == labelNameToTest);

        // 3. ACT (Etiketi Kaldırma) & ASSERT
        var labelsAfterRemove = await Connect.To<GitHubProvider>()
            .WithToken(_githubToken)
            .As<IGitHubActions>()
            .RemoveLabelFromIssue(issue.Number, labelNameToTest);

        Assert.NotNull(labelsAfterRemove);
        Assert.DoesNotContain(labelsAfterRemove, l => l.Name == labelNameToTest);
    }
}