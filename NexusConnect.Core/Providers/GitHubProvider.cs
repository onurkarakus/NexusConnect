using NexusConnect.Core.Exceptions;
using NexusConnect.Core.Providers.GitHub;
using NexusConnect.Core.Providers.GitHub.Enums;
using NexusConnect.Core.Providers.GitHub.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NexusConnect.Core.Providers;

/// <summary>
/// Provides methods to interact with GitHub issues for a specific repository.
/// Supports authentication, issue creation, retrieval, and updates using the GitHub REST API.
/// </summary>
public class GitHubProvider : IProvider, IGitHubActions
{
    private static readonly HttpClient httpClient = new HttpClient();
    private readonly string _owner;
    private readonly string _repo;
    
    public string Name => "GitHub";
    private string? _token;

    /// <summary>
    /// Initializes a new instance of <see cref="GitHubProvider"/> for the specified repository owner and name.
    /// </summary>
    /// <param name="owner">The GitHub repository owner.</param>
    /// <param name="repo">The GitHub repository name.</param>
    /// <exception cref="ArgumentNullException">Thrown if owner or repo is null or whitespace.</exception>
    public GitHubProvider(string owner, string repo)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentNullException(nameof(owner));
        if (string.IsNullOrWhiteSpace(repo)) throw new ArgumentNullException(nameof(repo));

        _owner = owner;
        _repo = repo;
    }

    /// <summary>
    /// Authenticates the provider with a GitHub token.
    /// </summary>
    /// <param name="token">The GitHub personal access token.</param>
    public void Authenticate(string token)
    {
        _token = token;
    }

    /// <summary>
    /// Retrieves issues from the repository filtered by state.
    /// </summary>
    /// <param name="state">The state of issues to retrieve (Open, Closed, All).</param>
    /// <returns>A collection of <see cref="Issue"/> objects.</returns>
    public async Task<IEnumerable<Issue>> GetIssues(IssueState state = IssueState.Open)
    {
        var stateString = state.ToString().ToLower();
        var response = await SendRequestAsync(HttpMethod.Get, $"issues?state={stateString}");

        var contentStream = await response.Content.ReadAsStreamAsync();
        var issues = await JsonSerializer.DeserializeAsync<IEnumerable<Issue>>(contentStream);

        return issues ?? [];
    }

    /// <summary>
    /// Creates a new issue in the repository.
    /// </summary>
    /// <param name="title">The title of the issue.</param>
    /// <param name="body">The body/description of the issue.</param>
    /// <returns>The created <see cref="Issue"/>.</returns>
    /// <exception cref="NexusApiException">Thrown if the response body is empty or invalid.</exception>
    public async Task<Issue> CreateIssue(string title, string body)
    {
        var issueRequest = new { title, body };
        var jsonContent = JsonSerializer.Serialize(issueRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await SendRequestAsync(HttpMethod.Post, "issues", httpContent);

        var contentStream = await response.Content.ReadAsStreamAsync();
        var createdIssue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return createdIssue ?? throw new NexusApiException("GitHub created an issue but the response body was empty or invalid.");
    }

    /// <summary>
    /// Retrieves a single issue by its number.
    /// </summary>
    /// <param name="issueNumber">The issue number.</param>
    /// <returns>The <see cref="Issue"/> with the specified number.</returns>
    /// <exception cref="ArgumentException">Thrown if issueNumber is not positive.</exception>
    /// <exception cref="NexusApiException">Thrown if the issue data cannot be deserialized.</exception>
    public async Task<Issue> GetIssueByNumber(int issueNumber)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentException("Issue number must be a positive value.", nameof(issueNumber));
        }

        var response = await SendRequestAsync(HttpMethod.Get, $"issues/{issueNumber}");

        var contentStream = await response.Content.ReadAsStreamAsync();
        var issue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return issue ?? throw new NexusApiException("The issue data from the API could not be deserialized or was empty.");
    }

    /// <summary>
    /// Sends an HTTP request to the GitHub API for the configured repository.
    /// Handles authentication and error mapping.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="endpoint">The API endpoint relative to the repository.</param>
    /// <param name="content">Optional HTTP content for the request.</param>
    /// <returns>The <see cref="HttpResponseMessage"/> from the API.</returns>
    /// <exception cref="InvalidOperationException">Thrown if authentication is missing.</exception>
    /// <exception cref="NexusApiAuthorizationException">Thrown for authorization errors.</exception>
    /// <exception cref="NexusApiNotFoundException">Thrown if the resource is not found.</exception>
    /// <exception cref="NexusApiException">Thrown for other API errors.</exception>
    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string endpoint, HttpContent? content = null)
    {
        if (string.IsNullOrEmpty(_token))
            throw new InvalidOperationException("Authentication required.");

        var requestUri = $"https://api.github.com/repos/{_owner}/{_repo}/{endpoint}";
        using var request = new HttpRequestMessage(method, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("NexusConnect", "0.1"));

        if (content != null)
        {
            request.Content = content;
        }

        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        throw response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden => new NexusApiAuthorizationException("GitHub API request failed due to an authorization issue. Please check your token and its permissions."),
            System.Net.HttpStatusCode.NotFound => new NexusApiNotFoundException($"The requested GitHub resource was not found. Please check the repository owner and name. URI: {request.RequestUri}"),
            _ => new NexusApiException($"An unexpected error occurred with the GitHub API. Status Code: {response.StatusCode}", new HttpRequestException())
        };
    }

    /// <summary>
    /// Updates an existing issue in the repository.
    /// </summary>
    /// <param name="issueNumber">The issue number to update.</param>
    /// <param name="title">Optional new title.</param>
    /// <param name="body">Optional new body/description.</param>
    /// <param name="state">Optional new state (Open, Closed).</param>
    /// <returns>The updated <see cref="Issue"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if issueNumber is not positive or no fields are specified.</exception>
    /// <exception cref="NexusApiException">Thrown if the response body is empty or invalid.</exception>
    public async Task<Issue> UpdateIssue(int issueNumber, string? title = null, string? body = null, IssueState? state = null)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentException("Issue number must be a positive value.", nameof(issueNumber));
        }

        var payload = new Dictionary<string, object>();
        if (title != null) payload["title"] = title;
        if (body != null) payload["body"] = body;
        if (state != null) payload["state"] = state.Value.ToString().ToLower();

        if (!payload.Any())
        {
            throw new ArgumentException("At least one field must be specified for an update.");
        }

        var jsonContent = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await SendRequestAsync(new HttpMethod("PATCH"), $"issues/{issueNumber}", httpContent);

        var contentStream = await response.Content.ReadAsStreamAsync();
        var updatedIssue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return updatedIssue ?? throw new NexusApiException("GitHub updated the issue but the response body was empty or invalid.");
    }
}
