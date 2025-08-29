using NexusConnect.Core.Providers;
using NexusConnect.Core.Providers.Twitter;
using Microsoft.Extensions.Configuration;
using NexusConnect.Core.Providers.Twitter.Models;

namespace NexusConnect.Core.Tests.Providers.Twitter;

public class TwitterProviderTests
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _accessToken;
    private readonly string _accessTokenSecret;

    public TwitterProviderTests()
    {
        _apiKey = Environment.GetEnvironmentVariable("TWITTER_API_KEY") ?? GetSecret("Twitter:ApiKey");
        _apiSecret = Environment.GetEnvironmentVariable("TWITTER_API_SECRET") ?? GetSecret("Twitter:ApiSecret");
        _accessToken = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN") ?? GetSecret("Twitter:AccessToken");
        _accessTokenSecret = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN_SECRET") ?? GetSecret("Twitter:AccessTokenSecret");

        NexusConnector.Configure(cfg =>
        {
            cfg.RegisterProvider<TwitterProvider>(() => new TwitterProvider(_apiKey, _apiSecret, _accessToken, _accessTokenSecret));
        });
    }

    private string GetSecret(string key)
    {
        var config = new ConfigurationBuilder().AddUserSecrets<TwitterProviderTests>().Build();
        return config[key] ?? throw new InvalidOperationException($"Secret '{key}' could not be found in User Secrets. Please add it using 'dotnet user-secrets set \"{key}\" \"your_value\" --project NexusConnect.Core.Tests'");
    }

    [Fact]
    public async Task PostTweet_ShouldSucceedAndReturnTweetData()
    {
        // ARRANGE
        string tweetText = $"Hello World from NexusConnect! Test run at {DateTime.UtcNow.Ticks}";
        Tweet? postedTweet = null;

        // ACT
        var exception = await Record.ExceptionAsync(async () =>
        {
            postedTweet = await Connect.To<TwitterProvider>()
                .As<ITwitterActions>()
                .PostTweet(tweetText);
        });

        // ASSERT
        Assert.Null(exception);
        Assert.NotNull(postedTweet);
        Assert.Equal(tweetText, postedTweet.Text);
        Assert.NotEmpty(postedTweet.Id);
    }
}