using NexusConnect.Core.Providers.GitHub.Enums;
using NexusConnect.Core.Providers.GitHub.Models;

namespace NexusConnect.Core.Providers.GitHub;

/// <summary>
/// Provides methods for interacting with GitHub Issues via the GitHub API.
/// </summary>
public interface IGitHubActions
{
    /// <summary>
    /// Creates a new issue with the specified title and body.
    /// </summary>
    /// <param name="title">The title of the issue.</param>
    /// <param name="body">The body content of the issue.</param>
    /// <returns>The created <see cref="Issue"/>.</returns>
    Task<Issue> CreateIssue(string title, string body);

    /// <summary>
    /// Retrieves a collection of issues filtered by the specified state.
    /// </summary>
    /// <param name="state">The state to filter issues by. Defaults to <see cref="IssueState.Open"/>.</param>
    /// <returns>A collection of <see cref="Issue"/> objects.</returns>
    Task<IEnumerable<Issue>> GetIssues(IssueState state = IssueState.Open);

    /// <summary>
    /// Retrieves a single issue by its number.
    /// </summary>
    /// <param name="issueNumber">The number of the issue.</param>
    /// <returns>The <see cref="Issue"/> with the specified number.</returns>
    Task<Issue> GetIssueByNumber(int issueNumber);

    /// <summary>
    /// Updates an existing issue with new values for title, body, or state.
    /// </summary>
    /// <param name="issueNumber">The number of the issue to update.</param>
    /// <param name="title">The new title for the issue (optional).</param>
    /// <param name="body">The new body content for the issue (optional).</param>
    /// <param name="state">The new state for the issue (optional).</param>
    /// <returns>The updated <see cref="Issue"/>.</returns>
    Task<Issue> UpdateIssue(int issueNumber, string? title = null, string? body = null, IssueState? state = null);
}
