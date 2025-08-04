using NexusConnect.Core.Exceptions;
using NexusConnect.Core.Providers.GitHub;
using NexusConnect.Core.Providers.GitHub.Enums;
using NexusConnect.Core.Providers.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NexusConnect.Core.Providers;

public class GitHubProvider : IProvider, IGitHubActions
{
    private static readonly HttpClient httpClient = new HttpClient();
    private readonly string _owner;
    private readonly string _repo;
    public string Name => "GitHub";
    private string? _token;

    public GitHubProvider(string owner, string repo)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentNullException(nameof(owner));
        if (string.IsNullOrWhiteSpace(repo)) throw new ArgumentNullException(nameof(repo));

        _owner = owner;
        _repo = repo;
    }

    public void Authenticate(string token)
    {
        _token = token;
    }    

    public async Task<IEnumerable<Issue>> GetIssues(IssueState state = IssueState.Open)
    {
        var stateString = state.ToString().ToLower();
        var response = await SendRequestAsync(HttpMethod.Get, $"issues?state={stateString}");

        var contentStream = await response.Content.ReadAsStreamAsync();
        var issues = await JsonSerializer.DeserializeAsync<IEnumerable<Issue>>(contentStream);

        return issues ?? [];
    }

    public async Task<Issue> CreateIssue(string title, string body)
    {
        var issueRequest = new { title, body };
        var jsonContent = JsonSerializer.Serialize(issueRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await SendRequestAsync(HttpMethod.Post, "issues", httpContent);

        var contentStream = await response.Content.ReadAsStreamAsync();
        var createdIssue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return createdIssue ?? throw new NexusApiException("GitHub bir issue oluşturdu ancak cevap gövdesi boş veya geçersiz.");
    }

    public async Task<Issue> GetIssueByNumber(int issueNumber)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentException("Issue numarası pozitif bir değer olmalıdır.", nameof(issueNumber));
        }

        var response = await SendRequestAsync(HttpMethod.Get, $"issues/{issueNumber}");

        var contentStream = await response.Content.ReadAsStreamAsync();
        var issue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return issue ?? throw new NexusApiException("API'den gelen issue verisi deserialize edilemedi veya boş.");
    }

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
            System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden => new NexusApiAuthorizationException("GitHub API isteği yetkilendirme sorunu nedeniyle başarısız oldu."),
            System.Net.HttpStatusCode.NotFound => new NexusApiNotFoundException($"İstenen GitHub kaynağı bulunamadı. URI: {requestUri}"),
            _ => new NexusApiException($"GitHub API'sinde beklenmedik bir hata oluştu. Durum Kodu: {response.StatusCode}", new HttpRequestException())
        };
    }

    public async Task<Issue> UpdateIssue(int issueNumber, string? title = null, string? body = null, IssueState? state = null)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentException("Issue numarası pozitif bir değer olmalıdır.", nameof(issueNumber));
        }

        var payload = new Dictionary<string, object>();
        if (title != null) payload["title"] = title;
        if (body != null) payload["body"] = body;
        if (state != null) payload["state"] = state.Value.ToString().ToLower();

        if (!payload.Any())
        {
            throw new ArgumentException("Güncellemek için en az bir alan belirtilmelidir.");
        }

        var jsonContent = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await SendRequestAsync(new HttpMethod("PATCH"), $"issues/{issueNumber}", httpContent);

        var contentStream = await response.Content.ReadAsStreamAsync();
        var updatedIssue = await JsonSerializer.DeserializeAsync<Issue>(contentStream);

        return updatedIssue ?? throw new NexusApiException("GitHub issue'yu güncelledi ancak cevap gövdesi boş veya geçersiz.");
    }
}
