using System.Text.Json.Serialization;

namespace NexusConnect.Core.Providers.GitHub.Models;

/// <summary>
/// Represents a label on a GitHub issue or pull request.
/// </summary>
public class Label
{
    /// <summary>
    /// The unique identifier of the label.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// The name of the label.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The hexadecimal color code for the label, without the leading #.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// A short description of the label.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}