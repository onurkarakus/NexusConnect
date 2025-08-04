using System.Text.Json.Serialization;

namespace NexusConnect.Core.Providers.GitHub.Enums;

/// <summary>
/// Represents the state of a GitHub issue.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IssueState
{
    /// <summary>
    /// The issue is open.
    /// </summary>
    Open,

    /// <summary>
    /// The issue is closed.
    /// </summary>
    Closed,

    /// <summary>
    /// Includes all issues regardless of state.
    /// </summary>
    All
}
