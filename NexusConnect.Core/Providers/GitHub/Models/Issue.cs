using System.Text.Json.Serialization;

namespace NexusConnect.Core.Providers.GitHub.Models;

/// <summary>
/// Represents a GitHub issue with basic properties.
/// </summary>
public class Issue
{
    /// <summary>
    /// Gets or sets the unique identifier of the issue.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the issue number in the repository.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the title of the issue.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state of the issue (e.g., open, closed).
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML URL of the issue.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string Url { get; set; } = string.Empty;
}
