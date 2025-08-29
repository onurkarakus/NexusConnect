using NexusConnect.Core.Providers.Twitter.Models;
using System.Threading.Tasks;

namespace NexusConnect.Core.Providers.Twitter;

/// <summary>
/// Defines actions specific to the Twitter provider.
/// </summary>
public interface ITwitterActions
{
    /// <summary>
    /// Posts a new Tweet.
    /// </summary>
    /// <param name="text">The text content of the Tweet. Must be 280 characters or less.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the created <see cref="Tweet"/> data.</returns>
    Task<Tweet> PostTweet(string text);
}
