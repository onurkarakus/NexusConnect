using System.Text.Json.Serialization;

namespace NexusConnect.Core.Providers.GitHub.Models;

/// <summary>
/// Represents a comment on a GitHub issue.
/// </summary>
public class Comment
{
    /// <summary>
    /// The unique identifier of the comment.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// The content of the comment.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The URL to view the comment on GitHub.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string Url { get; set; } = string.Empty;
}