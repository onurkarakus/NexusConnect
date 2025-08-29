using System.Text.Json.Serialization;

namespace NexusConnect.Core.Providers.Twitter.Models;

/// <summary>
/// Represents the data of a Tweet returned by the Twitter API v2.
/// </summary>
public class Tweet
{
    /// <summary>
    /// The unique identifier of the created Tweet.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The actual UTF-8 text of the Tweet.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
