using NexusConnect.Core.Providers.GitHub.Enums;
using NexusConnect.Core.Providers.GitHub.Models;

namespace NexusConnect.Core.Providers.GitHub;

public interface IGitHubActions
{
    Task<Issue> CreateIssue(string title, string body);

    Task<IEnumerable<Issue>> GetIssues(IssueState state = IssueState.Open);

    Task<Issue> GetIssueByNumber(int issueNumber);

    Task<Issue> UpdateIssue(int issueNumber, string? title = null, string? body = null, IssueState? state = null);
}
