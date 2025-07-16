namespace NexusConnect.Core.Providers.GitHub;

public interface IGitHubActions
{
    void CreateIssue(string title, string body);
}
